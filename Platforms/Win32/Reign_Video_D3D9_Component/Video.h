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

	public enum class REIGN_D3DPRIMITIVETYPE
	{
		POINTLIST = D3DPT_POINTLIST,
		LINELIST = D3DPT_LINELIST,
		TRIANGLELIST = D3DPT_TRIANGLELIST
	};

	public enum class REIGN_D3DPOOL
	{
		DEFAULT = D3DPOOL_DEFAULT,
		MANAGED = D3DPOOL_MANAGED,
		SYSTEMMEM = D3DPOOL_SYSTEMMEM
	};

	public enum class REIGN_D3DUSAGE
	{
		NONE = 0,
		DYNAMIC = D3DUSAGE_DYNAMIC,
		WRITEONLY = D3DUSAGE_WRITEONLY,
		RENDERTARGET = D3DUSAGE_RENDERTARGET,
		DEPTHSTENCIL = D3DUSAGE_DEPTHSTENCIL
	};

	public enum class REIGN_D3DFORMAT
	{
		DXT1 = D3DFMT_DXT1,
		DXT3 = D3DFMT_DXT3,
		DXT5 = D3DFMT_DXT5,
		R5G6B5 = D3DFMT_R5G6B5,
		A4R4G4B4 = D3DFMT_A4R4G4B4,
		A1R5G5B5 = D3DFMT_A1R5G5B5,
		A8R8G8B8 = D3DFMT_A8R8G8B8,
		A2R10G10B10 = D3DFMT_A2R10G10B10,
		A16B16G16R16F = D3DFMT_A16B16G16R16F,
		A32B32G32R32F = D3DFMT_A32B32G32R32F
	};

	public enum class REIGN_D3DDECLUSAGE
	{
		POSITION = D3DDECLUSAGE_POSITION,
		BLENDWEIGHT = D3DDECLUSAGE_BLENDWEIGHT,
		BLENDINDICES = D3DDECLUSAGE_BLENDINDICES,
		NORMAL = D3DDECLUSAGE_NORMAL,
		TEXCOORD = D3DDECLUSAGE_TEXCOORD,
		TANGENT = D3DDECLUSAGE_TANGENT,
		BINORMAL = D3DDECLUSAGE_BINORMAL,
		TESSFACTOR = D3DDECLUSAGE_TESSFACTOR,
		COLOR = D3DDECLUSAGE_COLOR
	};

	public enum class REIGN_D3DDECLTYPE
	{
		FLOAT1 = D3DDECLTYPE_FLOAT1,
		FLOAT2 = D3DDECLTYPE_FLOAT2,
		FLOAT3 = D3DDECLTYPE_FLOAT3,
		FLOAT4 = D3DDECLTYPE_FLOAT4,
		D3DCOLOR = D3DDECLTYPE_D3DCOLOR
	};

	public enum class REIGN_D3DDECLMETHOD
	{
		 DEFAULT = D3DDECLMETHOD_DEFAULT
	};

	public ref class ComponentCaps
	{
		public: bool ExDevice;

		public: bool HardwareInstancing;
		public: uint MaxTextureCount;
		public: float MaxVertexShaderVersion;
		public: float MaxPixelShaderVersion;
		public: float MaxShaderVersion;

		public: ComponentCaps() {}
	};

	ref class DepthStencilCom;

	public ref class VideoCom sealed
	{
		#pragma region Properties
		internal: IDirect3DDevice9* device;
		private: IDirect3DSurface9* backBuffer, *depthStencilBuffer;
		private: HWND handle;
		internal: bool isExDevice;
		private: bool active, fullScreen, vSync;
		private: int lastWidth, lastHeight;
		internal: ComponentCaps^ caps;
		private: int depthBit, stencilBit;

		public: delegate void DeviceLostMethod();
		public: DeviceLostMethod ^DeviceLost, ^DeviceReset;
		#pragma endregion

		#pragma region Constructors
		public: VideoError Init(IntPtr handle, bool vSync, int width, int height, int depthBit, int stencilBit, bool fullscreen, bool multithreaded, [Out] ComponentCaps^% caps);
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
		public: void EnableRenderTarget(DepthStencilCom^ depthStencil);
		public: void Present();
		public: void ClearAll(float r, float g, float b, float a);
		public: void ClearColor(float r, float g, float b, float a);
		public: void ClearColorDepth(float r, float g, float b, float a);
		public: void ClearDepthStencil();
		public: void DisableTexture(int index);
		public: void DisableVertexTexture(int index);
		public: void DisableRenderTarget(int index);
		#pragma endregion
	};
}