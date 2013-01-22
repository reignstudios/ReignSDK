#pragma once
#if METRO
#include <d3d11_1.h>
#include <d2d1_1.h>
#include <windows.ui.xaml.media.dxinterop.h>
#elif WP8
#include <d3d11_1.h>
#include <DrawingSurfaceNative.h>
#include "Direct3DContentProvider.h"
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
		#if !WP8
		SwapChainFailed,
		D2DFactoryFailed,
		D2DDeviceFailed,
		D2DDeviceContextFailed,
		NativeSwapChainPanelFailed,
		GetDXGIBackBufferFailed,
		DXGISurfaceFailed,
		D2DBitmapFailed,
		#else
		RenderTextureFailed,
		#endif
		#endif
		#if !WP8
		GetSwapChainFailed,
		#endif
		RenderTargetViewFailed,
		DepthStencilTextureFailed,
		DepthStencilViewFailed
	};

	public enum class REIGN_D3D_FEATURE_LEVEL
	{
		#if METRO || WP8
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
		DEFAULT = D3D11_USAGE_DEFAULT,
		DYNAMIC = D3D11_USAGE_DYNAMIC,
		STAGING = D3D11_USAGE_STAGING
	};

	public enum class REIGN_D3D11_CPU_ACCESS_FLAG
	{
		READ = D3D11_CPU_ACCESS_READ,
		WRITE = D3D11_CPU_ACCESS_WRITE
	};

	public enum class REIGN_DXGI_FORMAT
	{
		BC1_UNORM = DXGI_FORMAT_BC1_UNORM,
		BC2_UNORM = DXGI_FORMAT_BC2_UNORM,
		BC3_UNORM = DXGI_FORMAT_BC3_UNORM,

		R8G8B8A8_UNORM = DXGI_FORMAT_R8G8B8A8_UNORM,
		R10G10B10A2_UNORM = DXGI_FORMAT_R10G10B10A2_UNORM,
		R16G16B16A16_FLOAT = DXGI_FORMAT_R16G16B16A16_FLOAT,
		R32G32B32A32_FLOAT = DXGI_FORMAT_R32G32B32A32_FLOAT,

		R32_FLOAT = DXGI_FORMAT_R32_FLOAT,
		R32G32_FLOAT = DXGI_FORMAT_R32G32_FLOAT,
		R32G32B32_FLOAT = DXGI_FORMAT_R32G32B32_FLOAT
	};

	ref class DepthStencilCom;

	public ref class VideoCom sealed
	{
		#pragma region Properties
		private: DXGI_FORMAT swapChainFromat;
		#if WINDOWS
		private: IDXGISwapChain* swapChain;
		internal: ID3D11Device* device;
		internal: ID3D11DeviceContext* deviceContext;
		#else
		#if METRO
		private: IDXGISwapChain1* swapChain;
		#else
		private: ComPtr<Direct3DContentProvider> contentProvider;
		private: ID3D11Texture2D* renderTexture;
		#endif
		internal: ID3D11Device1* device;
		internal: ID3D11DeviceContext1* deviceContext;
		private: bool compositionMode;
		#if !WP8
		private: float dpi;
		private: ID2D1Factory1* d2dFactory;
		private: ID2D1Device* d2dDevice;
		private: ID2D1DeviceContext* d2dDeviceContext;
		private: IDXGISurface2* dxgiSurface;
		private: ID2D1Bitmap1* d2dRenderTarget;
		private: ISwapChainBackgroundPanelNative* swapChainBackgroundPanelNative;
		#endif
		#endif
		private: ID3D11RenderTargetView* renderTarget;
		private: ID3D11DepthStencilView* depthStencil;
		private: ID3D11Texture2D* depthTexture;
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
		#elif METRO
		public: VideoError Init(Windows::UI::Core::CoreWindow^ coreWindow, bool vSync, int width, int height, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel, Windows::UI::Xaml::Controls::SwapChainBackgroundPanel^ swapChainBackgroundPanel);
		#else
		public: VideoError Init(bool vSync, int width, int height, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel);
		#endif
		private: VideoError createViews(int width, int height);
		public: virtual ~VideoCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Update(int width, int height);
		public: void EnableRenderTarget();
		public: void EnableRenderTarget(DepthStencilCom^ depthStencil);
		public: void ClearAll(float r, float g, float b, float a);
		public: void ClearColor(float r, float g, float b, float a);
		public: void ClearColorDepth(float r, float g, float b, float a);
		public: void ClearDepthStencil();
		public: void Present();
		internal: void removeActiveResource(ID3D11ShaderResourceView* shaderResource);
		#pragma endregion
	};
}