#pragma once
#include "Common.h"
#include <ppapi/c/ppb_graphics_3d.h>
#include <ppapi/c/ppb_instance.h>
#include <ppapi/c/ppb_opengles2.h>

bool g_monoRunning;

void Mono_Init(PPB_Core* core, PP_Instance instance, PPB_Graphics3D* graphics, PPB_Instance* pbbInstance, struct PPB_OpenGLES2* gles);
void Mono_LoadDlls();
void Mono_LoadExe();
void Mono_Execute();

int Mono_GetInstanceCallback();
PPB_Graphics3D* Mono_GetGraphicsCallback();
PPB_Instance* Mono_GetPBBInstanceCallback();
struct PPB_OpenGLES2* Mono_GetGLESCallback();

void Mono_PostMessageCallback(MonoString* arg);
void Mono_InvokeMethodOnMainThread(const char* libName, const char* method);
void Mono_InvokeMethod(const char* assemblyName, const char* method, bool hasNamespace);
void Mono_InvokeMethodStringArg(const char* assemblyName, const char* method, bool hasNamespace, const char* arg);
void Mono_InvokeMethodArgs(const char* assemblyName, const char* method, bool hasNamespace, void* args);
void Mono_InvokeMethodArgs2(MonoMethod* method, void* args);
MonoMethod* Mono_FindMethod(const char* assemblyName, const char* method, bool hasNamespace);