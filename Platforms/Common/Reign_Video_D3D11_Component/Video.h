#pragma once
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
		#if WINDOWS
		private: IDXGISwapChain* swapChain;
		internal: ID3D11Device* device;
		internal: ID3D11DeviceContext* deviceContext;
		#else
		private: IDXGISwapChain1* swapChain;
		internal: ID3D11Device1* device;
		internal: ID3D11DeviceContext1* deviceContext;
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
		#else
		public: VideoError Init(Windows::UI::Core::CoreWindow^ coreWindow, bool vSync, int width, int height, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel);
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