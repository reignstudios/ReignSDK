﻿#pragma once
#include "../Video.h"
#if WIN32
#include <d3d11shader.h>
#endif

namespace Reign_Video_D3D11_Component
{
	public enum class ShaderModelErrors
	{
		None,
		#if WIN32
		CompileCode,
		#endif
		VariableBuffer,
		Reflect
	};

	public ref class ShaderModelCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;

		#if WIN32
		internal: ID3DBlob* code;
		private: ID3D11ShaderReflection* reflection;
		private: ID3D11ShaderReflectionConstantBuffer* variables;
		#else
		internal: byte* code;
		internal: int codeSize;
		#endif

		internal: ID3D11Buffer* variableBuffer;
		internal: byte* variableBufferBytes;
		private: uint variableBufferBytesCount;
		
		internal: ID3D11ShaderResourceView** resources;
		internal: uint resourceKnownCount;
		private: uint resourceCount;
		#pragma endregion

		#pragma region Constructors
		#if WIN32
		public: ShaderModelErrors Init(VideoCom^ video, string^ code, int codeSize, string^ shaderType, OutType(string^) errorText);
		#else
		public: ShaderModelErrors Init(VideoCom^ video, const array<byte>^ code, int codeSize, int variableBufferSize, int resourceCount);
		#endif
		public: virtual ~ShaderModelCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Apply();
		#if WIN32
		public: int Variable(string^ name);
		public: int Resource(string^ name);
		public: static string^ Compile(string^ code, int codeSize, string^ shaderType, [Out] IntPtr% buffer, [Out] int% bufferSize);
		#endif
		#pragma endregion
	};
}