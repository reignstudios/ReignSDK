#include "pch.h"
#include "PixelShader.h"
#include "Shader.h"

using namespace System::Runtime::InteropServices;
using namespace Reign::Core;

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	PixelShader::PixelShader(ShaderI^ shader, string^ code, ShaderVersions shaderVersion)
	: ShaderModel(shader, code, shaderVersion, ShaderTypes::PS)
	{
		null();
		try
		{
			IDirect3DPixelShader9* shaderTEMP = 0;
			video->Device->CreatePixelShader((DWORD*)this->code->GetBufferPointer(), &shaderTEMP);
			this->shader = shaderTEMP;
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}

	PixelShader::~PixelShader()
	{
		disposeChilderen();
		if (shader) shader->Release();
		null();
	}

	void PixelShader::null()
	{
		shader = 0;
	}
	#pragma endregion

	#pragma region Methods
	void PixelShader::Apply()
	{
		video->Device->SetPixelShader(shader);
	}
	#pragma endregion
}
}
}