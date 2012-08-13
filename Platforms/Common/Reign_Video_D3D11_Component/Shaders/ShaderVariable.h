#pragma once
#include "../Video.h"
#include <d3d11shader.h>
#include "ShaderModel.h"

namespace Reign_Video_D3D11_Component
{
	public ref class ShaderVariableCom sealed
	{
		#pragma region Properties
		private: char* vertexBytes, *pixelBytes;
		private: int vertexOffset, pixelOffset;
		#pragma endregion

		#pragma region Constructors
		public: ShaderVariableCom(ShaderModelCom^ vertexShader, ShaderModelCom^ pixelShader, int vertexOffset, int pixelOffset);
		#pragma endregion

		#pragma region Methods
		public: void Set(IntPtr data, int size);
		public: void Set(IntPtr data, int size, int objectCount, int dataOffset);
		#pragma endregion
	};
}