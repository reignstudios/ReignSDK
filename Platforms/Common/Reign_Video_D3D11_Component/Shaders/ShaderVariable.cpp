﻿#include "pch.h"
#include "ShaderVariable.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	ShaderVariableCom::ShaderVariableCom(ShaderModelCom^ vertexShader, ShaderModelCom^ pixelShader, int vertexOffset, int pixelOffset)
	{
		this->vertexBytes = vertexShader->variableBufferBytes;
		this->pixelBytes = pixelShader->variableBufferBytes;
		this->vertexOffset = vertexOffset;
		this->pixelOffset = pixelOffset;
	}
	#pragma endregion

	#pragma region Methods
	void ShaderVariableCom::Set(__int32 data, int size)
	{
		void* ptr = (void*)data;
		if (vertexOffset != -1) memcpy(vertexBytes + vertexOffset, ptr, size);
		if (pixelOffset != -1) memcpy(pixelBytes + pixelOffset, ptr, size);
	}

	void ShaderVariableCom::Set(__int32 data, int size, int objectCount, int bufferStep)
	{
		char* ptr = (char*)data;
		if (vertexOffset != -1)
		{
			char* vertexPtr = vertexBytes + vertexOffset;
			for (int i = 0; i != objectCount; ++i)
			{
				memcpy(vertexPtr, ptr, size);
				vertexPtr += bufferStep;
				ptr += size;
			}
		}
		if (pixelOffset != -1)
		{
			char* pixelPtr = pixelBytes + pixelOffset;
			for (int i = 0; i != objectCount; ++i)
			{
				memcpy(pixelPtr, ptr, size);
				pixelPtr += bufferStep;
				ptr += size;
			}
		}
	}
	#pragma endregion
}