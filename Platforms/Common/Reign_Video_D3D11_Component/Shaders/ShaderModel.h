#pragma once
#include "../Video.h"
#include <d3d11shader.h>

namespace Reign_Video_D3D11_Component
{
	public enum class ShaderModelErrors
	{
		None,
		CompileCode,
		VariableBuffer,
		Reflect
	};

	public ref class ShaderModelCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		internal: ID3DBlob* code;
		internal: ID3D11Buffer* variableBuffer;
		private: uint variableBufferBytesCount;
		private: ID3D11ShaderReflection* reflection;
		private: ID3D11ShaderReflectionConstantBuffer* variables;

		internal: char* variableBufferBytes;

		internal: ID3D11ShaderResourceView** resources;
		internal: uint resourcesCount, resourcesKnownCount;
		#pragma endregion

		#pragma region Constructors
		public: ShaderModelErrors Init(VideoCom^ video, string^ code, int codeSize, string^ shaderType, OutType(string^) errorText);
		public: virtual ~ShaderModelCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Apply();
		public: int Variable(string^ name);
		public: int Resource(string^ name);
		#pragma endregion
	};
}