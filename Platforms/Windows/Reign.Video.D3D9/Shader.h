#pragma once
#include "Video.h"
#include "ShaderVariable.h"
#include "ShaderResource.h"
#include "VertexShader.h"
#include "PixelShader.h"
#include <d3d9.h>

using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class Shader : ShaderI
	{
		#pragma region Properties
		private: Video^ video;
		private: List<ShaderVariable^>^ variables;
		private: List<ShaderResource^>^ resources;

		private: VertexShader^ vertex;
		public: property VertexShader^ Vertex {VertexShader^ get();}

		private: PixelShader^ pixel;
		public: property PixelShader^ Pixel {PixelShader^ get();}
		#pragma endregion

		#pragma region Constructors
		public: static Shader^ New(DisposableI^ parent, string^ fileName, ShaderVersions shaderVersion);
		public: Shader(DisposableI^ parent, string^ fileName, ShaderVersions shaderVersion);
		#pragma endregion

		#pragma region Methods
		public: virtual void Apply() override;
		public: virtual ShaderVariableI^ Variable(string^ name) override;
		public: virtual ShaderResourceI^ Resource(string^ name) override;
		#pragma endregion
	};
}
}
}