#pragma once
#include "../Video.h"
#include <d3d11shader.h>
#include "ShaderModel.h"

namespace Reign_Video_D3D11_Component
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
		private: ID3D11PixelShader* shader;

		private: ID3D11ShaderResourceView** resources;
		private: ID3D11Buffer* variableBuffer;
		private: uint resourceKnownCount;
		#pragma endregion

		#pragma region Constructors
		public: PixelShaderErrors Init(VideoCom^ video, ShaderModelCom^ shaderModel);
		public: virtual ~PixelShaderCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Apply();
		#pragma endregion
	};
}