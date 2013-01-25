#include "pch.h"
#include "ShaderResource.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Contructors
	ShaderResourceCom::ShaderResourceCom(VideoCom^ video, int vertexIndex, int pixelIndex)
	{
		this->video = video;
		this->vertexIndex = vertexIndex;
		this->pixelIndex = pixelIndex;
	}
	#pragma endregion

	#pragma region Methods
	void ShaderResourceCom::SetTexture2D(Texture2DCom^ texture)
	{
		if (vertexIndex != -1) video->device->SetTexture(D3DVERTEXTEXTURESAMPLER0 + vertexIndex, texture->texture);
		if (pixelIndex != -1) video->device->SetTexture(pixelIndex, texture->texture);
	}
	#pragma endregion
}