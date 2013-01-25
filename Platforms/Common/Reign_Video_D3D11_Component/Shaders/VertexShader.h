#pragma once
#include "../Video.h"
#include "ShaderModel.h"
#if WIN32
#include <d3d11shader.h>
#endif

namespace Reign_Video_D3D11_Component
{
	public enum class VertexShaderErrors
	{
		None,
		VertexShader
	};

	public ref class VertexShaderCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: ID3D11VertexShader* shader;

		private: ID3D11ShaderResourceView** resources;
		private: ID3D11Buffer* variableBuffer;
		private: uint resourceKnownCount;
		#pragma endregion

		#pragma region Constructors
		public: VertexShaderErrors Init(VideoCom^ video, ShaderModelCom^ shaderModel);
		public: virtual ~VertexShaderCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Apply();
		#pragma endregion
	};
}