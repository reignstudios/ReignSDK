#pragma once
#include "Video.h"

namespace Reign_Video_D3D9_Component
{
	public ref class ShaderModelCom sealed
	{
		#pragma region Methods
		public: static string^ Compile(string^ code, int codeSize, string^ shaderType, [Out] IntPtr% buffer, [Out] int% bufferSize);
		#pragma endregion
	};
}