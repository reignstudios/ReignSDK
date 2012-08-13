#include "pch.h"
#include "PixelShader.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	PixelShaderErrors PixelShaderCom::Init(VideoCom^ video, ShaderModelCom^ shaderModel)
	{
		null();
		this->video = video;
		resources = shaderModel->resources;
		variableBuffer = shaderModel->variableBuffer;
		resourcesKnownCount = shaderModel->resourcesKnownCount;

		ID3DBlob* codeBlob = shaderModel->code;
		ID3D11PixelShader* shaderTEMP;
		if (FAILED(video->device->CreatePixelShader(codeBlob->GetBufferPointer(), codeBlob->GetBufferSize(), NULL, &shaderTEMP)))
		{
			return PixelShaderErrors::PixelShader;
		}
		shader = shaderTEMP;

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
		if (variableBuffer)
		{
			ID3D11Buffer* buffer = variableBuffer;
			video->deviceContext->PSSetConstantBuffers(0, 1, &buffer);
		}

		if (resourcesKnownCount != 0 && resources)
		{
			video->deviceContext->PSSetShaderResources(0, resourcesKnownCount, resources);
		}

		video->deviceContext->PSSetShader(shader, NULL, 0);
	}
	#pragma endregion
}