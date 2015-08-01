#include "pch.h"
#include "PixelShader.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	PixelShaderErrors PixelShaderCom::Init(VideoCom^ video, ShaderModelCom^ shaderModel)
	{
		null();
		this->video = video;

		IDirect3DPixelShader9* shaderTEMP = 0;
		// TODO: load data file file
		/*if (FAILED(video->device->CreatePixelShader((DWORD*)shaderModel->code->GetBufferPointer(), &shaderTEMP)))
		{
			return PixelShaderErrors::PixelShader;
		}*/
		this->shader = shaderTEMP;

		return PixelShaderErrors::None;
	}

	PixelShaderCom::~PixelShaderCom()
	{
		if (shader) shader->Release();
		null();
	}

	void PixelShaderCom::null()
	{
		shader = 0;
	}
	#pragma endregion

	#pragma region Methods
	void PixelShaderCom::Apply()
	{
		video->device->SetPixelShader(shader);
	}
	#pragma endregion
}