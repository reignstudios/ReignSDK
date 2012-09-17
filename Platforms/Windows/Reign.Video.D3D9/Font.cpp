#include "pch.h"
#include "Font.h"
using namespace Reign::Core;

namespace Reign
{namespace Video
{namespace D3D9
{
	ref class FonttreamLoader : StreamLoaderI
	{
		private: Font^ font;
		private: ShaderI^ shader;
		private: Texture2DI^ texture;
		private: string^ metricsFileName;

		public: FonttreamLoader(Font^ font, ShaderI^ shader, Texture2DI^ texture, string^ metricsFileName)
		{
			this->font = font;
			this->shader = shader;
			this->texture = texture;
			this->metricsFileName = metricsFileName;
		}

		public: virtual bool Load() override
		{
			if (!shader->Loaded || !texture->Loaded) return false;
			font->load(shader, texture, metricsFileName);
			return true;
		}
	};

	#pragma region Constructors
	Font::Font(DisposableI^ parent, ShaderI^ shader, Texture2DI^ texture)
	: FontI(parent)
	{
		gcnew FonttreamLoader(this, shader, texture, nullptr);
	}

	Font::Font(DisposableI^ parent, ShaderI^ shader, Texture2DI^ texture, string^ metricsFileName)
	: FontI(parent, metricsFileName)
	{
		gcnew FonttreamLoader(this, shader, texture, metricsFileName);
	}

	void Font::load(ShaderI^ shader, Texture2DI^ texture, string^ metricsFileName)
	{
		init(shader, texture, metricsFileName);
	}

	void Font::init(ShaderI^ shader, Texture2DI^ texture, string^ metricsFileName)
	{
		if (metricsFileName != nullptr) FontI::init(shader, texture, metricsFileName);

		this->fontTexture = texture;
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
		Loaded = true;
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