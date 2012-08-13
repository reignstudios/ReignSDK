#include "pch.h"
#include "VertexShader.h"
#include "Shader.h"

using namespace System::Runtime::InteropServices;
using namespace Reign::Core;

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	VertexShader::VertexShader(ShaderI^ shader, string^ code, ShaderVersions shaderVersion)
	: ShaderModel(shader, code, shaderVersion, ShaderTypes::VS)
	{
		null();
		try
		{
			IDirect3DVertexShader9* shaderTEMP = 0;
			video->Device->CreateVertexShader((DWORD*)this->code->GetBufferPointer(), &shaderTEMP);
			this->shader = shaderTEMP;
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}

	VertexShader::~VertexShader()
	{
		disposeChilderen();
		if (shader) shader->Release();
		null();
	}

	void VertexShader::null()
	{
		shader = 0;
	}
	#pragma endregion

	#pragma region Methods
	void VertexShader::Apply()
	{
		video->Device->SetVertexShader(shader);
	}
	#pragma endregion
}
}
}