#include "PostMessage.h"

static const PPB_Core* g_core;
static const PPB_Messaging* g_messaging;
static const PPB_Var* g_var;
static PP_Instance g_instance;

void PostMessage_Init(PP_Instance instance, const void* messaging, const void* core, const void* var)
{
	assert(messaging);
	g_instance = instance;
	g_messaging = (const PPB_Messaging*)messaging;
	g_core = (const PPB_Core*)core;
	g_var = (const PPB_Var*)var;
}

void PostMessage_Callback(void* data, int32_t result)
{
	char* msg = (char*)data;
	struct PP_Var msg_var = g_var->VarFromUtf8(msg, strlen(msg));
	g_messaging->PostMessage(g_instance, msg_var);
	g_var->Release(msg_var);
	free(msg);
}

void PostMessage(const char *msg)
{
	char* new_msg = strdup(msg);
	if (g_core->IsMainThread())
	{
		PostMessage_Callback((void*)new_msg, 0);
	}
	else
	{
		struct PP_CompletionCallback cb = PP_MakeCompletionCallback(PostMessage_Callback, (void*)new_msg);
		g_core->CallOnMainThread(0, cb, 0);
	}
}

