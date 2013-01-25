#pragma once
#include "ShaderModel.h"

namespace Reign_Video_D3D9_Component
{
	public enum class PixelShaderErrors
	{
		None,
		PixelShader
	};

	public ref class PixelShaderCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: IDirect3DPixelShader9* shader;
		#pragma endregion

		#pragma region Constructors
		public: PixelShaderErrors Init(VideoCom^ video, ShaderModelCom^ shaderModel);
		public: ~PixelShaderCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Apply();
		#pragma endregion
	};
}