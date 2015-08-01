#pragma once
#include "Video.h"
//#include <D3DX9Effect.h>// TODO load effects from compiled files

namespace Reign_Video_D3D9_Component
{
	public enum class ShaderModelErrors
	{
		None,
		Compile
	};

	public ref class ShaderModelCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		//internal: ID3DXBuffer* code;
		//internal: ID3DXConstantTable* variables;
		#pragma endregion

		#pragma region Constructors
		public: ShaderModelErrors Init(VideoCom^ video, IntPtr codePtr, int codeLength, IntPtr shaderVersionTypePtr, [Out] string^% errorText);
		public: ~ShaderModelCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		//private: D3DXHANDLE variableHandle(string^ name);
		public: IntPtr Variable(string^ name);
		public: int Resource(string^ name);
		public: static string^ Compile(string^ code, int codeSize, string^ shaderType, [Out] IntPtr% buffer, [Out] int% bufferSize);
		#pragma endregion
	};
}