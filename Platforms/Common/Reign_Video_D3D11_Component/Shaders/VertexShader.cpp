#include "pch.h"
#include "VertexShader.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	VertexShaderErrors VertexShaderCom::Init(VideoCom^ video, ShaderModelCom^ shaderModel)
	{
		null();
		this->video = video;
		resources = shaderModel->resources;
		variableBuffer = shaderModel->variableBuffer;
		resourcesKnownCount = shaderModel->resourcesKnownCount;

		ID3DBlob* codeBlob = shaderModel->code;
		ID3D11VertexShader* shaderTEMP;
		if (FAILED(video->device->CreateVertexShader(codeBlob->GetBufferPointer(), codeBlob->GetBufferSize(), NULL, &shaderTEMP)))
		{
			return VertexShaderErrors::VertexShader;
		}
		shader = shaderTEMP;

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
		if (variableBuffer)
		{
			ID3D11Buffer* buffer = variableBuffer;
			video->deviceContext->VSSetConstantBuffers(0, 1, &buffer);
		}

		if (resourcesKnownCount != 0 && resources)
		{
			video->deviceContext->VSSetShaderResources(0, resourcesKnownCount, resources);
		}

		video->deviceContext->VSSetShader(shader, NULL, 0);
	}
	#pragma endregion
}