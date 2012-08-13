#include "pch.h"
#include "Font.h"
using namespace Reign::Core;

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	Font::Font(DisposableI^ parent, ShaderI^ shader, Texture2DI^ fontTexture)
	: FontI(parent)
	{
		init(shader, fontTexture);
	}

	Font::Font(DisposableI^ parent, ShaderI^ shader, Texture2DI^ fontTexture, string^ metricsFileName)
	: FontI(parent, metricsFileName)
	{
		init(shader, fontTexture);
	}

	void Font::init(ShaderI^ shader, Texture2DI^ fontTexture)
	{
		this->fontTexture = fontTexture;
		this->shader = shader;
		shaderCamera = shader->Variable(L"Camera");
		shaderLocation = shader->Variable(L"Location");
		shaderSize = shader->Variable(L"Size");
		shaderLocationUV = shader->Variable(L"LocationUV");
		shaderSizeUV = shader->Variable(L"SizeUV");
		texelOffset = shader->Variable(L"TexelOffset");
		shaderColor = shader->Variable(L"Color");
		shaderTexture = shader->Resource(L"DiffuseTexture");

		layoutDesc = gcnew BufferLayoutDesc(BufferLayoutTypes::Position2);
		layout = gcnew BufferLayout(this, shader, layoutDesc);

		indexBuffer = gcnew IndexBuffer(this, BufferUsages::Default, Indices);
		vertexBuffer = gcnew VertexBuffer(this, layoutDesc, BufferUsages::Default, VertexBufferTopologys::Triangle, Vertices);
	}
	#pragma endregion

	#pragma region Methods
	void Font::DrawStart(Camera^ camera)
	{
		vertexBuffer->Enable(indexBuffer);
		shaderCamera->Set(camera->TransformMatrix);
		texelOffset->Set(fontTexture->TexelOffset);
		shaderTexture->Set(fontTexture);
		layout->Enable();
		instancing = false;
	}

	void Font::Draw(string^ text, Vector2 Location, Vector4 color, float size, bool centeredX, bool centeredY)
	{
		if (instancing)
		{
			
		}
		else
		{
			draw(text, fontTexture->SizeF, Location, color, size, centeredX, centeredY);
		}
	}

	void Font::draw(Vector2 location, Vector2 size, Vector2 locationUV, Vector2 sizeUV, Vector4 color)
	{
		shaderLocation->Set(location);
		shaderSize->Set(size);
		shaderLocationUV->Set(locationUV);
		shaderSizeUV->Set(sizeUV);
		shaderColor->Set(color);
		shader->Apply();
		vertexBuffer->Draw();
	}
	#pragma endregion
}
}
}