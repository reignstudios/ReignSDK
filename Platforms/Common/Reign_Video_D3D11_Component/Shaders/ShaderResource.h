#pragma once
#include "../Video.h"
#include "ShaderModel.h"
#include "../Textures/Texture2D.h"
#if WIN32
#include <d3d11shader.h>
#endif

namespace Reign_Video_D3D11_Component
{
	public ref class ShaderResourceCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: ID3D11ShaderResourceView **vertexResources, **pixelResources;
		private: int vertexIndex, pixelIndex;
		#pragma endregion

		#pragma region Constructors
		public: ShaderResourceCom(VideoCom^ video, ShaderModelCom^ vertexShader, ShaderModelCom^ pixelShader, int vertexIndex, int pixelIndex);
		#pragma endregion

		#pragma region Methods
		public: void Set(Texture2DCom^ resource);
		//public: void Set(Texture3DI^ resource);
		#pragma endregion
	};
}