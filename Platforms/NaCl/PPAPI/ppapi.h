#pragma once
#include "Common.h"

char* VarToCStr(struct PP_Var var);
static void HandleMessage(PP_Instance instance, struct PP_Var message);
static PP_Bool HandleInputEvent(PP_Instance instance, PP_Resource input_event);

static PP_Bool Instance_DidCreate(PP_Instance instance, uint32_t argc, const char* argn[], const char* argv[]);
static void Instance_DidDestroy(PP_Instance instance);
static void Instance_DidChangeView(PP_Instance instance, PP_Resource view_resource);
static void Instance_DidChangeFocus(PP_Instance instance, PP_Bool has_focus);
static PP_Bool Instance_HandleDocumentLoad(PP_Instance instance, PP_Resource url_loader);

PP_EXPORT int32_t PPP_InitializeModule(PP_Module a_module_id, PPB_GetInterface get_browser);
PP_EXPORT const void* PPP_GetInterface(const char* interface_name);
PP_EXPORT void PPP_ShutdownModule();

