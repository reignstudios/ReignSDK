#include "pch.h"
#include "VertexShader.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	VertexShaderErrors VertexShaderCom::Init(VideoCom^ video, ShaderModelCom^ shaderModel)
	{
		null();
		this->video = video;

		IDirect3DVertexShader9* shaderTEMP = 0;
		// TODO: load data file file
		/*if (FAILED(video->device->CreateVertexShader((DWORD*)shaderModel->code->GetBufferPointer(), &shaderTEMP)))
		{
			return VertexShaderErrors::VertexShader;
		}*/
		this->shader = shaderTEMP;

		return VertexShaderErrors::None;
	}

	VertexShaderCom::~VertexShaderCom()
	{
		if (shader) shader->Release();
		null();
	}

	void VertexShaderCom::null()
	{
		shader = 0;
	}
	#pragma endregion

	#pragma region Methods
	void VertexShaderCom::Apply()
	{
		video->device->SetVertexShader(shader);
	}
	#pragma endregion
}