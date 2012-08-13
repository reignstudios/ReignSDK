#include "pch.h"
#include "ShaderResource.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	ShaderResourceCom::ShaderResourceCom(VideoCom^ video, ShaderModelCom^ vertexShader, ShaderModelCom^ pixelShader, int vertexIndex, int pixelIndex)
	{
		this->video = video;
		this->vertexResources = vertexShader->resources;
		this->pixelResources = pixelShader->resources;
		this->vertexIndex = vertexIndex;
		this->pixelIndex = pixelIndex;
	}
	#pragma endregion

	#pragma region Methods
	void ShaderResourceCom::Set(Texture2DCom^ resource)
	{
		if (vertexIndex != -1)
		{
			vertexResources[vertexIndex] = resource->shaderResource;
			video->currentVertexResources[vertexIndex] = resource->shaderResource;
		}
		if (pixelIndex != -1)
		{
			pixelResources[pixelIndex] = resource->shaderResource;
			video->currentPixelResources[pixelIndex] = resource->shaderResource;
		}
	}

	/*void ShaderResource::Set(Texture3DI^ resource)
	{
		Texture3D^ texture = (Texture3D^)resource;
		if (vertexIndex != -1)
		{
			vertexResources[vertexIndex] = texture->ShaderResource;
			video->currentVertexResources[vertexIndex] = texture->ShaderResource;
		}
		if (pixelIndex != -1)
		{
			pixelResources[pixelIndex] = texture->ShaderResource;
			video->currentPixelResources[pixelIndex] = texture->ShaderResource;
		}
	}*/
	#pragma endregion
}