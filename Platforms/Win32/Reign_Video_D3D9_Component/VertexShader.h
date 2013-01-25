#pragma once
#include "ShaderModel.h"

namespace Reign_Video_D3D9_Component
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
		private: IDirect3DVertexShader9* shader;
		#pragma endregion

		#pragma region Constructors
		public: VertexShaderErrors Init(VideoCom^ video, ShaderModelCom^ shaderModel);
		public: ~VertexShaderCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Apply();
		#pragma endregion
	};
}