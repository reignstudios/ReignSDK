#pragma once
#include "Video.h"
#include "ShaderVariable.h"
#include "ShaderModel.h"
#include <d3d9.h>

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class PixelShader : ShaderModel
	{
		#pragma region Properties
		private: IDirect3DPixelShader9* shader;
		#pragma endregion

		#pragma region Constructors
		public: PixelShader(ShaderI^ shader, string^ code, ShaderVersions shaderVersion);
		public: ~PixelShader();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Apply();
		#pragma endregion
	};
}
}
}