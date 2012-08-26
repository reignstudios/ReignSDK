﻿#pragma once
#if METRO
#include <d3d11_1.h>
#else
#include <d3d11.h>
#endif

#include "../CPP_CLR-CX_Helpers/Common.h"

namespace Reign_Video_D3D11_Component
{
	public enum class VideoError
	{
		None,
		#if WINDOWS
		DeviceAndSwapChainFailed,
		#else
		DeviceFailed,
		SwapChainFailed,
		#endif
		GetSwapChainFailed,
		RenderTargetViewFailed,
		DepthStencilTextureFailed,
		DepthStencilViewFailed
	};

	public enum class REIGN_D3D_FEATURE_LEVEL
	{
		#if METRO
		LEVEL_11_1 = D3D_FEATURE_LEVEL_11_1,
		#endif
		LEVEL_11_0 = D3D_FEATURE_LEVEL_11_0,
		LEVEL_10_1 = D3D_FEATURE_LEVEL_10_1,
		LEVEL_10_0 = D3D_FEATURE_LEVEL_10_0,
		LEVEL_9_3 = D3D_FEATURE_LEVEL_9_3,
		LEVEL_9_2 = D3D_FEATURE_LEVEL_9_2,
		LEVEL_9_1 = D3D_FEATURE_LEVEL_9_1
	};

	public enum class REIGN_D3D11_USAGE
	{
		DEFAULT = 0,
		DYNAMIC = 2,
		STAGING = 3
	};

	public enum class REIGN_D3D11_CPU_ACCESS_FLAG
	{
		READ = 0x20000,
		WRITE = 0x10000
	};

	public enum class REIGN_DXGI_FORMAT
	{
		R8G8B8A8_UNORM = 28,
		R10G10B10A2_UNORM = 24,
		R16G16B16A16_FLOAT = 10,
		R32G32B32A32_FLOAT = 2,

		R32_FLOAT = 41,
		R32G32_FLOAT = 16,
		R32G32B32_FLOAT = 6
	};

	ref class DepthStencilCom;

	public ref class VideoCom sealed
	{
		#pragma region Properties
		private: IDXGISwapChain* swapChain;
		private: ID3D11RenderTargetView* renderTarget;
		private: ID3D11DepthStencilView* depthStencil;
		private: ID3D11Texture2D* depthTexture;
		internal: ID3D11Device* device;
		internal: ID3D11DeviceContext* deviceContext;
		internal: ID3D11RenderTargetView* currentRenderTarget;
		internal: ID3D11DepthStencilView* currentDepthStencil;

		internal: int currentVertexResourceCount, currentPixelResourceCount;
		internal: ID3D11ShaderResourceView **currentVertexResources, **currentPixelResources;

		private: bool vSync;
		private: int lastWidth, lastHeight;
		#pragma endregion

		#pragma region Constructors
		#if WINDOWS
		public: VideoError Init(IntPtr handle, bool vSync, int width, int height, bool fullscreen, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel);
		#else
		public: VideoError Init(IntPtr handle, bool vSync, int width, int height, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel);
		#endif
		private: VideoError createViews(int width, int height);
		public: virtual ~VideoCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Update(int width, int height);
		public: void EnableRenderTarget();
		public: void EnableRenderTarget(DepthStencilCom^ depthStencil);
		public: void Clear(float r, float g, float b, float a);
		public: void ClearColor(float r, float g, float b, float a);
		public: void ClearDepthStencil();
		public: void Present();
		internal: void removeActiveResource(ID3D11ShaderResourceView* shaderResource);
		#pragma endregion
	};
}