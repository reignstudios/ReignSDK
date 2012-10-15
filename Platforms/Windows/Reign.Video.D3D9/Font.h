#pragma once
#include "IndexBuffer.h"
#include "VertexBuffer.h"
#include "BufferLayout.h"
#include "Shader.h"
#include "Texture2D.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class Font : FontI
	{
		#pragma region Properties
		private: ShaderI^ shader;
		private: ShaderVariableI^ shaderCamera, ^shaderLocation, ^shaderSize, ^shaderLocationUV, ^shaderSizeUV, ^texelOffset, ^shaderColor;
		private: ShaderResourceI^ shaderTexture;
		private: Texture2DI^ fontTexture;
		private: BufferLayoutDesc^ layoutDesc;
		private: BufferLayout^ layout;
		private: IndexBuffer^ indexBuffer;
		private: VertexBuffer^ vertexBuffer;
		private: bool instancing;
		#pragma endregion

		#pragma region Constructors
		public: Font(DisposableI^ parent, ShaderI^ shader, Texture2DI^ font);
		public: Font(DisposableI^ parent, ShaderI^ shader, Texture2DI^ font, string^ metricsFileName);
		internal: void load(ShaderI^ shader, Texture2DI^ texture, string^ metricsFileName);
		protected: virtual void init(ShaderI^ shader, Texture2DI^ texture, string^ metricsFileName) override;
		#pragma endregion

		#pragma region Methods
		public: virtual void DrawStart(Camera^ camera) override;
		public: virtual void Draw(string^ text, Vector2 location, Vector4 color, float size, bool centeredX, bool centeredY) override;
		protected: virtual void draw(Vector2 location, Vector2 size, Vector2 locationUV, Vector2 sizeUV, Vector4 color) override;
		#pragma endregion
	};
}
}
}