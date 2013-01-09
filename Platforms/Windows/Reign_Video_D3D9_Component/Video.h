#pragma once
#include <d3d9.h>
#include "../../Common/CPP_CLR-CX_Helpers/Common.h"

namespace Reign_Video_D3D9_Component
{
	public enum class VideoError
	{
		None,
		Direct3DInterfaceFailed,
		AdapterDisplayModeFailed,
		VideoHardwareNotSupported,
		GetCapsFailed,
		DeviceAndSwapChainFailed
	};

	public enum class REIGN_D3DFMT
	{
		DXT1 = D3DFMT_DXT1,
		DXT3 = D3DFMT_DXT3,
		DXT5 = D3DFMT_DXT5,
		A8R8G8B8 = D3DFMT_A8R8G8B8,
		A2R10G10B10 = D3DFMT_A2R10G10B10,
		A16B16G16R16F = D3DFMT_A16B16G16R16F,
		A32B32G32R32F = D3DFMT_A32B32G32R32F
	};

	public ref class ComponentCaps
	{
		public: bool D3D9Ex;

		public: bool HardwareInstancing;
		public: uint MaxTextureCount;
		public: float MaxVertexShaderVersion;
		public: float MaxPixelShaderVersion;
		public: float MaxShaderVersion;

		public: ComponentCaps() {}
	};

	public ref class VideoCom sealed
	{
		#pragma region Properties
		private: IDirect3DDevice9* device;
		private: IDirect3DSurface9* backBuffer, *depthStencilBuffer;
		private: HWND handle;
		internal: bool isExDevice;
		private: bool active, fullScreen, vSync;
		private: int lastWidth, lastHeight;
		private: ComponentCaps^ caps;

		public: delegate void DeviceLostMethod();
		public: DeviceLostMethod ^DeviceLost, ^DeviceReset;
		#pragma endregion

		#pragma region Constructors
		public: VideoError Init(IntPtr handle, bool vSync, int width, int height, bool fullscreen, bool multithreaded, [Out] ComponentCaps^% caps);
		private: D3DPRESENT_PARAMETERS createPresentParameters();
		public: ~VideoCom();
		private: void null();
		private: void defualtStates();
		#pragma endregion

		#pragma region Methods
		public: void Update(int width, int height);
		private: void deviceLost();
		private: void deviceReset();
		public: void EnableRenderTarget();
		//public: void EnableRenderTarget(DepthStencilI^ depthStencil);
		public: void DisableRenderTarget();
		public: void Present();
		public: void ClearAll(float r, float g, float b, float a);
		public: void ClearColor(float r, float g, float b, float a);
		public: void ClearColorDepth(float r, float g, float b, float a);
		public: void ClearDepthStencil();
		#pragma endregion
	};
}