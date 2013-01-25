#pragma once
#include "../Video.h"
#include "ShaderModel.h"
#if WIN32
#include <d3d11shader.h>
#endif

namespace Reign_Video_D3D11_Component
{
	public ref class ShaderVariableCom sealed
	{
		#pragma region Properties
		private: byte* vertexBytes, *pixelBytes;
		private: int vertexOffset, pixelOffset;
		#pragma endregion

		#pragma region Constructors
		public: ShaderVariableCom(ShaderModelCom^ vertexShader, ShaderModelCom^ pixelShader, int vertexOffset, int pixelOffset);
		#if WP8
		public: void GetDataPtrs(OutType(__int32) vertexBytes, OutType(__int32) pixelBytes);
		#endif
		#pragma endregion

		#pragma region Methods
		public: void Set(__int32 data, int size);
		public: void Set(__int32 data, int size, int objectCount, int bufferStep);
		#pragma endregion
	};
}