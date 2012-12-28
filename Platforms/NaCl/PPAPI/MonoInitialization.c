#include "MonoInitialization.h"
#include "ppapi.h"

char* libNames[256];
int libNamesLength;

PP_Instance g_instance;
PPB_Graphics3D* g_graphics;
PPB_Instance* g_pbbInstance;
struct PPB_OpenGLES2* g_gles;
PPB_Core* g_core;

int dllsLoadedCount;
bool loadingDlls;
void loadLib(int success, const char* buffer, size_t size, const void* data)
{
	assert(success);
	char assembly_name[256];
	snprintf(assembly_name, 256, "/%s", (const char*)data);
	
	MonoImageOpenStatus imgstatus = MONO_IMAGE_ERROR_ERRNO;
	MonoImage* mi = mono_image_open_from_data_with_name((char*)buffer, size, 0, &imgstatus, 0, assembly_name);
	assert(mi && imgstatus == MONO_IMAGE_OK);
	
	imgstatus = MONO_IMAGE_OK;
	MonoAssembly* ma = mono_assembly_load_from_full(mi, assembly_name, &imgstatus, 0);
	assert(ma && imgstatus == MONO_IMAGE_OK);
	
	if (loadingDlls)
	{
		++dllsLoadedCount;
		if (dllsLoadedCount == libNamesLength) PostMessage("Mono Loaded Dlls");
	}
	else
	{
		PostMessage("Mono Loaded Exe");
	}
}

bool coreLoaded, libFileNamesLoaded;
void checkIfCoreIsLoaded()
{
	if (coreLoaded && libFileNamesLoaded) PostMessage("Mono Initialized");
}

void loadLibFileNames(int success, const char* buffer, size_t size, const void* data)
{
	assert(success);
	
	char name[256];
	memset(name, 0, sizeof(char)*256);
	libNamesLength = 0;
	
	int i = 0, i2 = 0, i3 = 0;
	while (i != size)
	{
		bool copy = false;
		if (buffer[i] != '\n')
		{
			name[i2] = buffer[i];
			if (i + 1 >= size) copy = true;
			++i2;
		}
		else
		{
			copy = true;
		}
		
		if (copy)
		{
			name[i2] = '\0';
			libNames[i3] = strdup(name);
			i2 = 0;
			++i3;
		}
		
		++i;
	}
	libNamesLength = i3;
	
	libFileNamesLoaded = true;
	checkIfCoreIsLoaded();
}

void loadCore(int success, const char* buffer, size_t size, const void* data)
{
	assert(success);
	
	mono_set_corlib_data((char*)buffer, size);
	mono_set_dirs("/", "/");
	mono_config_parse (NULL);
	mono_jit_init("main.exe");
	
	coreLoaded = true;
	checkIfCoreIsLoaded();
}

void Mono_Init(PPB_Core* core, PP_Instance instance, PPB_Graphics3D* graphics, PPB_Instance* pbbInstance, struct PPB_OpenGLES2* gles)
{
	g_core = core;
	g_instance = instance;
	g_graphics = graphics;
	g_pbbInstance = pbbInstance;
	g_gles = gles;

	coreLoaded = false;
	libFileNamesLoaded = false;
	URLLoader_LoadFileFromURL("mscorlib.dll", loadCore, NULL);
	URLLoader_LoadFileFromURL("main.dep", loadLibFileNames, NULL);
}

void Mono_LoadDlls()
{
	dllsLoadedCount = 0;
	loadingDlls = true;
	int i = 0;
	while (i != libNamesLength)
	{
		char* fileName = libNames[i];
		URLLoader_LoadFileFromURL(fileName, loadLib, fileName);
		++i;
	}
}

void Mono_LoadExe()
{
	loadingDlls = false;
	URLLoader_LoadFileFromURL("main.exe", loadLib, "main.exe");
}

void Mono_Execute()
{
	g_monoRunning = true;
	MonoDomain *domain = mono_get_root_domain();
	MonoThread* monoThread = mono_thread_attach(domain);
	
	mono_add_internal_call("Reign.Core.OS::PostMessage", Mono_PostMessageCallback);
	mono_add_internal_call("Reign.Core.OS::GetInstance", Mono_GetInstanceCallback);
	mono_add_internal_call("Reign.Core.OS::GetGraphics", Mono_GetGraphicsCallback);
	mono_add_internal_call("Reign.Core.OS::GetPBBInstance", Mono_GetPBBInstanceCallback);
	mono_add_internal_call("Reign.Core.OS::GetGlES", Mono_GetGLESCallback);
	
	MonoAssemblyName* assemblyName = mono_assembly_name_new("main");
	assert(assemblyName);
	MonoAssembly* assembly = mono_assembly_loaded(assemblyName);
	assert(assembly);
	char* args[0];
	mono_jit_exec(domain, assembly, 1, args);
	
	mono_thread_detach(monoThread);
	PostMessage("Mono Executed Exe");
	g_monoRunning = false;
}

void Mono_PostMessageCallback(MonoString* arg)
{
	PostMessage(mono_string_to_utf8(arg));
}

int Mono_GetInstanceCallback()
{
	return g_instance;
}

PPB_Graphics3D* Mono_GetGraphicsCallback()
{
	return g_graphics;
}

PPB_Instance* Mono_GetPBBInstanceCallback()
{
	return g_pbbInstance;
}

struct PPB_OpenGLES2* Mono_GetGLESCallback()
{
	return g_gles;
}

void mono_InvokeCallback(void* data, int32_t result)
{
	char** names = (char**)data;
	Mono_InvokeMethod(names[0], names[1], true);
	free(names[0]);
	free(names[1]);
	free(names);
}

void Mono_InvokeMethodOnMainThread(const char* assemblyName, const char* method)
{
	char** names = malloc(2*sizeof(char*));
	names[0] = strdup(assemblyName);
	names[1] = strdup(method);

	if (g_core->IsMainThread())
	{
		mono_InvokeCallback(names, 0);
	}
	else
	{
		struct PP_CompletionCallback cb = PP_MakeCompletionCallback(mono_InvokeCallback, names);
		g_core->CallOnMainThread(0, cb, 0);
	}
}

void Mono_InvokeMethod(const char* assemblyName, const char* method, bool hasNamespace)
{
	//MonoDomain *domain = mono_get_root_domain();
	//MonoThread* monoThread = mono_thread_attach(domain);

	MonoAssemblyName* monoAssemblyName = mono_assembly_name_new(assemblyName);
	MonoAssembly* assembly = mono_assembly_loaded(monoAssemblyName);
    MonoImage* image = mono_assembly_get_image(assembly);
    MonoMethodDesc *desc = mono_method_desc_new(method, hasNamespace);
    MonoMethod *monoMethod = mono_method_desc_search_in_image(desc, image);
    mono_runtime_invoke(monoMethod, NULL, NULL, NULL);
}

void Mono_InvokeMethodStringArg(const char* assemblyName, const char* method, bool hasNamespace, const char* arg)
{
	MonoAssemblyName* monoAssemblyName = mono_assembly_name_new(assemblyName);
	MonoAssembly* assembly = mono_assembly_loaded(monoAssemblyName);
    MonoImage* image = mono_assembly_get_image(assembly);
    MonoMethodDesc *desc = mono_method_desc_new(method, hasNamespace);
    MonoMethod *monoMethod = mono_method_desc_search_in_image(desc, image);
    
    MonoDomain *domain = mono_get_root_domain();
    void* args[1];
	args[0] = mono_string_new(domain, arg);
    mono_runtime_invoke(monoMethod, NULL, args, NULL);
}

void Mono_InvokeMethodArgs(const char* assemblyName, const char* method, bool hasNamespace, void* args)
{
	MonoAssemblyName* monoAssemblyName = mono_assembly_name_new(assemblyName);
	MonoAssembly* assembly = mono_assembly_loaded(monoAssemblyName);
    MonoImage* image = mono_assembly_get_image(assembly);
    MonoMethodDesc *desc = mono_method_desc_new(method, hasNamespace);
    MonoMethod *monoMethod = mono_method_desc_search_in_image(desc, image);
    
    mono_runtime_invoke(monoMethod, NULL, args, NULL);
}

void Mono_InvokeMethodArgs2(MonoMethod* method, void* args)
{
    mono_runtime_invoke(method, NULL, args, NULL);
}

MonoMethod* Mono_FindMethod(const char* assemblyName, const char* method, bool hasNamespace)
{
	MonoAssemblyName* monoAssemblyName = mono_assembly_name_new(assemblyName);
	MonoAssembly* assembly = mono_assembly_loaded(monoAssemblyName);
    MonoImage* image = mono_assembly_get_image(assembly);
    MonoMethodDesc *desc = mono_method_desc_new(method, hasNamespace);
    return mono_method_desc_search_in_image(desc, image);
}