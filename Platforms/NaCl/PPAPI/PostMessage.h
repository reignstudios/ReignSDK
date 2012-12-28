#pragma once
#include "Common.h"

void PostMessage_Init(PP_Instance instance, const void* messaging, const void* core, const void* var);
void PostMessage_Callback(void* data, int32_t result);
void PostMessage(const char *msg);

