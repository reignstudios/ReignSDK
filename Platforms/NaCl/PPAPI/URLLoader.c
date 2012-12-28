#include "URLLoader.h"

static PP_Instance g_instance;
static const PPB_URLLoader* g_url_loader;
static const PPB_URLRequestInfo* g_url_request;
static const PPB_Core* g_core;
static const PPB_Var* g_var;
const size_t BUFFER_READ_SIZE = 4096;

void URLLoader_Init(PP_Instance instance, const void *url_loader, const void *url_request, const void *core, const void *var)
{
	assert(url_loader && url_request && core && var);
	g_instance = instance;
	g_url_loader = (const PPB_URLLoader*)url_loader;
	g_url_request = (const PPB_URLRequestInfo*)url_request;
	g_core = (const PPB_Core*)core;
	g_var = (const PPB_Var*)var;
}

void URLLoader_LoadFileFromURL(const char* url, URLLoaderCallback callBack, const void* userData)
{
	PP_Resource resource = g_url_request->Create(g_instance);
	struct PP_Var url_var = g_var->VarFromUtf8(url, strlen(url));
	g_url_request->SetProperty(resource, PP_URLREQUESTPROPERTY_URL, url_var);
	g_var->Release(url_var);
	PP_Resource loader = g_url_loader->Create(g_instance);
	
	struct URLLoaderData* data = (struct URLLoaderData*)malloc(sizeof(struct URLLoaderData));
	data->buf = (char*)malloc(BUFFER_READ_SIZE);
	data->size = 0;
	data->alloced = BUFFER_READ_SIZE;
	data->user_cb = callBack;
	data->user_data = userData;
	data->loader = loader;
	
	struct PP_CompletionCallback cb = PP_MakeCompletionCallback(URLLoader_ReadDataCallback, data);
	int32_t open_ret = g_url_loader->Open(loader, resource, cb);
	assert(open_ret == PP_OK_COMPLETIONPENDING);
	g_core->ReleaseResource(resource);
}

static void URLLoader_ReadDataCallback(void* data, int32_t result)
{
	assert(result == PP_OK);
	URLLoader_ReadData(data);
}

static void URLLoader_ReadSomeDataCallback(void* data, int32_t result)
{
	struct URLLoaderData *ldata = (struct URLLoaderData*)data;
	if (result == PP_OK)
	{
		// We're done reading the file.
		ldata->user_cb(1, ldata->buf, ldata->size, ldata->user_data);
		free(ldata);
	}
	else if (result > 0)
	{
		// 'result' bytes were read into memory.
		ldata->size += (size_t)result;
		URLLoader_ReadData(data);
	}
	else
	{
		ldata->user_cb(0, NULL, 0, ldata->user_data);
	}
}

// Read up to BUFFER_READ_SIZE from the URLLoader.
// Allocate more space in the destination buffer if needed.
void URLLoader_ReadData(void* data)
{
	struct URLLoaderData *ldata = (struct URLLoaderData*)data;
	while (ldata->alloced < ldata->size + BUFFER_READ_SIZE)
	{
		// The buffer isn't big enough to hold BUFFER_READ_SIZE.
		char *temp = (char*)malloc(ldata->alloced * 2);
		memcpy(temp, ldata->buf, ldata->size);
		free(ldata->buf);
		ldata->buf = temp;
		ldata->alloced *= 2;
	}
	
	struct PP_CompletionCallback callback = PP_MakeCompletionCallback(URLLoader_ReadSomeDataCallback, data);
	int32_t read_ret = g_url_loader->ReadResponseBody(ldata->loader, ldata->buf + ldata->size, BUFFER_READ_SIZE, callback);
	assert(read_ret == PP_OK_COMPLETIONPENDING);
}

// Used by Reign.Core.Streams
void URLLoader_LoadData(int success, const char* buffer, size_t size, const void* data)
{
	void** args = malloc(4*sizeof(void*));
	args[0] = buffer;
	args[1] = &size;
	args[2] = data;// ID
	bool failedToLoad = buffer == 0 ? true : false;//success == 0 ? true : false;
	args[3] = &failedToLoad;
	Mono_InvokeMethodArgs("Reign.Core", "Reign.Core.Streams:URLLoader_Done", true, args);
	free(data);
	free(args);
}

void URLLoader_LoadFile(const char* url, const char* id)
{
	URLLoader_LoadFileFromURL(url, URLLoader_LoadData, strdup(id));
}