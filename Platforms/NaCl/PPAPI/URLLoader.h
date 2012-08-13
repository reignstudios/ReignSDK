#pragma once
#include "Common.h"

typedef void (*URLLoaderCallback)(int, const char*, size_t, const void* data);
struct URLLoaderData
{
	char *buf;
	size_t size;
	size_t alloced;
	URLLoaderCallback user_cb;
	const void* user_data;
	PP_Resource loader;
};

void URLLoader_Init(PP_Instance instance, const void *url_loader, const void *url_request, const void *core, const void *var);
void URLLoader_LoadFileFromURL(const char* url, URLLoaderCallback callBack, const void* userData);
void URLLoader_ReadData(void* data);

static void URLLoader_ReadDataCallback(void* data, int32_t result);
static void URLLoader_ReadSomeDataCallback(void* data, int32_t result);

