#include "pch.h"
#include "ShaderResource.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properties
	string^ ShaderResource::Name::get() {return name;}
	#pragma endregion

	#pragma region Contructors
	ShaderResource::ShaderResource(VideoI^ video, int vertexIndex, int pixelIndex, string^ name)
	{
		this->video = (Video^)video;

		this->vertexIndex = vertexIndex;
		this->pixelIndex = pixelIndex;

		this->name = name;
		Apply = gcnew ApplyFunc(this, &ShaderResource::setNothing);
	}
	#pragma endregion

	#pragma region Methods
	void ShaderResource::setNothing()
	{
		// Place holder.
	}

	void ShaderResource::setTexture2D()
	{
		if (vertexIndex != -1) video->Device->SetTexture(D3DVERTEXTEXTURESAMPLER0 + vertexIndex, resource);
		if (pixelIndex != -1) video->Device->SetTexture(pixelIndex, resource);
	}

	void ShaderResource::Set(Texture2DI^ resource)
	{
		Texture2D^ texture = (Texture2D^)resource;
		if (vertexIndex != -1) video->currentVertexTextures[vertexIndex] = texture;
		if (pixelIndex != -1) video->currentPixelTextures[pixelIndex] = texture;
		this->resource = texture->Texture;
		Apply = gcnew ApplyFunc(this, &ShaderResource::setTexture2D);
	}

	void ShaderResource::Set(Texture3DI^ resource)
	{
		/*Texture2D^ texture = (Texture2D^)resource;
		if (vertexIndex != -1) video->currentVertexTextures[vertexIndex] = texture;
		if (pixelIndex != -1) video->currentPixelTextures[pixelIndex] = texture;
		this->resource = texture->Texture;
		Apply = gcnew ApplyFunc(this, &ShaderResource::setTexture2D);*/
	}
	#pragma endregion
}
}
}