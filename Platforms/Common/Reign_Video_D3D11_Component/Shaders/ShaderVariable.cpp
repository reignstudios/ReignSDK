#include "pch.h"
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

	#if WP8
	void ShaderVariableCom::GetDataPtrs(OutType(__int32) vertexBytes, OutType(__int32) pixelBytes, OutType(int) vertexOffset, OutType(int) pixelOffset)
	{
		*vertexBytes = (__int32)vertexBytes;
		*pixelBytes = (__int32)pixelBytes;
		*vertexOffset = this->vertexOffset;
		*pixelOffset = this->pixelOffset;
	}
	#endif
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
		byte* ptr = (byte*)data;
		if (vertexOffset != -1)
		{
			byte* vertexPtr = vertexBytes + vertexOffset;
			for (int i = 0; i != objectCount; ++i)
			{
				memcpy(vertexPtr, ptr, size);
				vertexPtr += bufferStep;
				ptr += size;
			}
		}
		if (pixelOffset != -1)
		{
			byte* pixelPtr = pixelBytes + pixelOffset;
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