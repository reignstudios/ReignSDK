#pragma once
#include "../Video.h"
#include "Texture2D.h"
#include "../DepthStencil/DepthStencil.h"

namespace Reign_Video_D3D11_Component
{
	public enum class RenderTargetError
	{
		None,
		RenderTargetView,
		StagingTexture
	};

	public ref class RenderTargetCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: Texture2DCom^ texture;
		private: ID3D11RenderTargetView* renderTarget;
		private: ID3D11Texture2D* stagingTexture;
		#pragma endregion

		#pragma region Constructors
		public: RenderTargetError Init(VideoCom^ video, int width, int height, Texture2DCom^ texture, int multiSampleMultiple, REIGN_DXGI_FORMAT surfaceFormat, bool readable);
		public: virtual ~RenderTargetCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Enable();
		public: void Enable(DepthStencilCom^ depthStencil);
		public: void ReadPixels(int dataPtr, int dataLength);
		public: int ReadPixel(int x, int y, int height);
		#pragma endregion
	};
}