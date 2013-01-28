#include "ppapi.h"
#include "MonoInitialization.h"
#include "Reign.Video.OpenGL.h"
#include <pthread.h>
#include <ppapi/c/ppb_graphics_3d.h>
#include <ppapi/c/ppb_instance.h>
#include <ppapi/c/ppb_opengles2.h>
#include <ppapi/c/ppb_input_event.h>
#include <ppapi/c/ppp_input_event.h>
#include <ppapi/c/pp_point.h>

static PPB_GetInterface g_get_interface = NULL;
static PPB_Var* g_ppb_var_interface = NULL;
pthread_t mainThread;

int pthread_cancel(pthread_t thread)
{
	return 0;
}

void* mainLoop(void* args)
{
	Mono_Execute();
}

void startMainLoop()
{
	void* args = NULL;
	pthread_create(&mainThread, NULL, mainLoop, args);
}

char* VarToCStr(struct PP_Var var)
{
	uint32_t len = 0;
	if (g_ppb_var_interface != NULL)
	{
		const char* var_c_str = g_ppb_var_interface->VarToUtf8(var, &len);
		if (len > 0)
		{
			char* c_str = (char*)malloc(len + 1);
			memcpy(c_str, var_c_str, len);
			c_str[len] = '\0';
			return c_str;
		}
	}
	return NULL;
}

// Handle massages from javaScript
static void HandleMessage(PP_Instance instance, struct PP_Var message)
{
	if (message.type != PP_VARTYPE_STRING) return;
	
	char* msg = VarToCStr(message);
	
	const char* loadAssemblyDlls = "LoadAssemblyDlls";
	const char* loadAssemblyExe = "LoadAssemblyExe";
	const char* executeAssembly = "ExecuteAssembly";
	if (strncmp(msg, loadAssemblyDlls, strlen(loadAssemblyDlls)) == 0) Mono_LoadDlls();
	if (strncmp(msg, loadAssemblyExe, strlen(loadAssemblyExe)) == 0) Mono_LoadExe();
	if (strncmp(msg, executeAssembly, strlen(executeAssembly)) == 0) startMainLoop();
	
	free(msg);
}

// Handle input
void** inputArgs;
PPB_InputEvent* inputEvent;
PPB_MouseInputEvent* mouseEvent;

MonoMethod* monoMethod_MouseMoveEvent = 0;
MonoMethod* monoMethod_LeftMouseDownEvent, *monoMethod_MiddleMouseDownEvent, *monoMethod_RightMouseDownEvent;
MonoMethod* monoMethod_LeftMouseUpEvent, *monoMethod_MiddleMouseUpEvent, *monoMethod_RightMouseUpEvent;

static PP_Bool HandleInputEvent(PP_Instance instance, PP_Resource input_event)
{
	if (monoMethod_MouseMoveEvent == 0)
	{
		monoMethod_MouseMoveEvent = Mono_FindMethod("Reign.Core", "Reign.Core.NaClApplication:naclHandleMouseMoveEvent", true);
		monoMethod_LeftMouseDownEvent = Mono_FindMethod("Reign.Core", "Reign.Core.NaClApplication:naclHandleLeftMouseDownEvent", true);
		monoMethod_MiddleMouseDownEvent = Mono_FindMethod("Reign.Core", "Reign.Core.NaClApplication:naclHandleMiddleMouseDownEvent", true);
		monoMethod_RightMouseDownEvent = Mono_FindMethod("Reign.Core", "Reign.Core.NaClApplication:naclHandleRightMouseDownEvent", true);
		monoMethod_LeftMouseUpEvent = Mono_FindMethod("Reign.Core", "Reign.Core.NaClApplication:naclHandleLeftMouseUpEvent", true);
		monoMethod_MiddleMouseUpEvent = Mono_FindMethod("Reign.Core", "Reign.Core.NaClApplication:naclHandleMiddleMouseUpEvent", true);
		monoMethod_RightMouseUpEvent = Mono_FindMethod("Reign.Core", "Reign.Core.NaClApplication:naclHandleRightMouseUpEvent", true);
	}

	if (mouseEvent->IsMouseInputEvent(input_event))
	{
		PP_InputEvent_Type eventType = inputEvent->GetType(input_event);
		struct PP_Point point = mouseEvent->GetPosition(input_event);
		inputArgs[0] = &point.x;
		inputArgs[1] = &point.y;
		
		if (eventType == PP_INPUTEVENT_TYPE_MOUSEMOVE)
		{
			Mono_InvokeMethodArgs2(monoMethod_MouseMoveEvent, inputArgs);
		}
		else if (eventType == PP_INPUTEVENT_TYPE_MOUSEDOWN)
		{
			PP_InputEvent_MouseButton buttonType = mouseEvent->GetButton(input_event);
			switch (buttonType)
			{
				case (PP_INPUTEVENT_MOUSEBUTTON_LEFT): Mono_InvokeMethodArgs2(monoMethod_LeftMouseDownEvent, inputArgs); break;
				case (PP_INPUTEVENT_MOUSEBUTTON_MIDDLE): Mono_InvokeMethodArgs2(monoMethod_MiddleMouseDownEvent, inputArgs); break;
				case (PP_INPUTEVENT_MOUSEBUTTON_RIGHT): Mono_InvokeMethodArgs2(monoMethod_RightMouseDownEvent, inputArgs); break;
			}
		}
		else if (eventType == PP_INPUTEVENT_TYPE_MOUSEUP)
		{
			PP_InputEvent_MouseButton buttonType = mouseEvent->GetButton(input_event);
			switch (buttonType)
			{
				case (PP_INPUTEVENT_MOUSEBUTTON_LEFT): Mono_InvokeMethodArgs2(monoMethod_LeftMouseUpEvent, inputArgs); break;
				case (PP_INPUTEVENT_MOUSEBUTTON_MIDDLE): Mono_InvokeMethodArgs2(monoMethod_MiddleMouseUpEvent, inputArgs); break;
				case (PP_INPUTEVENT_MOUSEBUTTON_RIGHT): Mono_InvokeMethodArgs2(monoMethod_RightMouseUpEvent, inputArgs); break;
			}
		}
	}
}

// Called when the NaCl module is instantiated on the web page.
static PP_Bool Instance_DidCreate(PP_Instance instance, uint32_t argc, const char* argn[], const char* argv[])
{
	// Init url loader
	URLLoader_Init
	(
		instance,
		g_get_interface(PPB_URLLOADER_INTERFACE),
		g_get_interface(PPB_URLREQUESTINFO_INTERFACE),
		g_get_interface(PPB_CORE_INTERFACE),
		g_get_interface(PPB_VAR_INTERFACE)
	);
		
	// Init messaging from javaScript
	PostMessage_Init
	(
		instance,
		g_get_interface(PPB_MESSAGING_INTERFACE),
		g_get_interface(PPB_CORE_INTERFACE),
		g_get_interface(PPB_VAR_INTERFACE)
	);
		
	// Init input
	inputArgs = malloc(sizeof(void*) * 2);
	inputEvent = (PPB_InputEvent*)g_get_interface(PPB_INPUT_EVENT_INTERFACE);
	mouseEvent = (PPB_MouseInputEvent*)g_get_interface(PPB_MOUSE_INPUT_EVENT_INTERFACE);
	inputEvent->RequestInputEvents(instance, PP_INPUTEVENT_CLASS_MOUSE);
		
	// Init mono
	PPB_Core* core = (PPB_Core*)g_get_interface(PPB_CORE_INTERFACE);
	PPB_Graphics3D* graphics = (PPB_Graphics3D*)g_get_interface(PPB_GRAPHICS_3D_INTERFACE);
	PPB_Instance* pbbInstance = (PPB_Instance*)g_get_interface(PPB_INSTANCE_INTERFACE);
	struct PPB_OpenGLES2* gles = (struct PPB_OpenGLES2*)g_get_interface(PPB_OPENGLES2_INTERFACE);
	Mono_Init(core, instance, graphics, pbbInstance, gles);
	
	return PP_TRUE;
}

static PP_Bool Instance_HandleDocumentLoad(PP_Instance instance, PP_Resource url_loader)
{
	return PP_FALSE;
}

static void Instance_DidDestroy(PP_Instance instance)
{
	StopSwapBufferLoop();
	Mono_InvokeMethodOnMainThread("Reign.Core", "Reign.Core.OS:exit");
	while (!g_monoRunning) sleep(1);
	free(inputArgs);
}

static void Instance_DidChangeView(PP_Instance instance, PP_Resource view_resource) {}
static void Instance_DidChangeFocus(PP_Instance instance, PP_Bool has_focus) {}

#pragma region EntryPoints
PP_EXPORT int32_t PPP_InitializeModule(PP_Module a_module_id, PPB_GetInterface get_interface)
{
	g_ppb_var_interface = (PPB_Var*)(get_interface(PPB_VAR_INTERFACE));
	g_get_interface = get_interface;
	
	return PP_OK;
}

PP_EXPORT const void* PPP_GetInterface(const char* interface_name)
{
	if (strcmp(interface_name, PPP_INSTANCE_INTERFACE) == 0)
	{
		static PPP_Instance instance_interface =
		{
			&Instance_DidCreate,
			&Instance_DidDestroy,
			&Instance_DidChangeView,
			&Instance_DidChangeFocus,
			&Instance_HandleDocumentLoad,
		};
		return &instance_interface;
	}
	else if (strcmp(interface_name, PPP_MESSAGING_INTERFACE) == 0)
	{
		static PPP_Messaging messaging_interface =
		{
			&HandleMessage
		};
		return &messaging_interface;
	}
	else if (strcmp(interface_name, PPP_INPUT_EVENT_INTERFACE) == 0)
	{
		static PPP_InputEvent input_interface =
		{
			&HandleInputEvent
		};
		return &input_interface;
	}
	
	return NULL;
}

PP_EXPORT void PPP_ShutdownModule() {}
#pragma endregion

