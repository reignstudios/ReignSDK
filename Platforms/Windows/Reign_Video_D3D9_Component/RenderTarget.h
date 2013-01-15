#pragma once
#include "Texture2D.h"
#include "DepthStencil.h"

namespace Reign_Video_D3D9_Component
{
	public enum class RenderTargetError
	{
		None,
		RenderTarget,
		StagingTexture
	};

	public ref class RenderTargetCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: IDirect3DSurface9* renderTarget, *surface, *stagingSurface;
		#pragma endregion

		#pragma region Constructors
		public: RenderTargetError Init(VideoCom^ video, Texture2DCom^ texture, int width, int height, int multiSampleMultiple, REIGN_D3DFORMAT surfaceFormat, bool readable, bool lockable);
		public: ~RenderTargetCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Enable();
		public: void Enable(DepthStencilCom^ depthStencil);
		public: void ResolveMultisampled();
		public: void ReadPixels(void* data, int dataLength);
		public: int ReadPixel(int x, int y, int height);
		#pragma endregion
	};
}