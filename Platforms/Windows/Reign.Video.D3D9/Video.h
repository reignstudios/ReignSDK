#pragma once
#include <d3d9.h>
#include <D3DX9Effect.h>
#include "../../Common/CPP_CLR-CX_Helpers/Common.h"

using namespace System;
using namespace Reign::Core;

namespace Reign
{namespace Video
{namespace D3D9
{
	ref class DepthStencil;
	ref class Texture2D;

	public enum class SoftwareModes
	{
		None,
		Software,
		Reference,
		SwiftShader
	};

	public ref class Caps
	{
		private: bool d3d9Ex;
		public: property bool D3D9Ex {bool get(); internal: void set(bool value);}

		private: bool hardwareInstancing;
		public: property bool HardwareInstancing {bool get(); internal: void set(bool value);}

		private: uint maxTextureCount;
		public: property uint MaxTextureCount {uint get(); internal: void set(uint value);}

		private: ShaderVersions maxVertexShaderVersion;
		public: property ShaderVersions MaxVertexShaderVersion {ShaderVersions get(); internal: void set(ShaderVersions value);}

		private: ShaderVersions maxPixelShaderVersion;
		public: property ShaderVersions MaxPixelShaderVersion {ShaderVersions get(); internal: void set(ShaderVersions value);}

		private: ShaderVersions maxShaderVersion;
		public: property ShaderVersions MaxShaderVersion {ShaderVersions get(); internal: void set(ShaderVersions value);}
	};

	public ref class Video : Disposable, VideoI
	{
		#pragma region Properties
		private: Window^ window;
		internal: array<Texture2D^>^ currentVertexTextures, ^currentPixelTextures;
		private: IDirect3DSurface9* backBuffer, *depthStencilBuffer;
		internal: ID3DXEffect* LastEffect;
		internal: uint LastEffectPass;
		private: bool active, fullScreen, vSync;
		private: IntPtr handle;

		public: delegate void DeviceLostMethod();
		public: DeviceLostMethod ^DeviceLost, ^DeviceReset;
		internal: bool deviceReseting;

		private: Size2 backBufferSize;
		public: property Size2 BackBufferSize {virtual Size2 get();}

		private: bool isExDevice;
		public: property bool IsExDevice {bool get();}

		private: IDirect3DDevice9* device;
		public: property IDirect3DDevice9* Device {IDirect3DDevice9* get();}

		private: bool deviceIsLost;
		public: property bool DeviceIsLost {bool get();}

		private: Reign::Video::D3D9::Caps^ caps;
		public: property Reign::Video::D3D9::Caps^ Caps {Reign::Video::D3D9::Caps^ get(); private: void set(Reign::Video::D3D9::Caps^ value);}

		private: string^ fileTag;
		public: property string^ FileTag {virtual string^ get();}

		public: property float PointSize {void set(float value);}
		public: property float PointSizeMax {void set(float value);}
		public: property float PointSizeMin {void set(float value);}
		public: property bool PointDepthScalable {void set(bool value);}
		public: property bool PointSpriteEnable {void set(bool value);}
		#pragma endregion

		#pragma region Constructors
		public: Video(DisposableI^ parent, Window^ window, bool vSync);
		private: void init(Window^ window, bool fullScreen, bool vSync, bool multithreaded, SoftwareModes softwareMode);
		private: IDirect3D9* createD3D9Ex();
		public: ~Video();
		private: void null();
		private: void defualtStates();
		#pragma endregion

		#pragma region Methods
		private: D3DPRESENT_PARAMETERS createPresentParameters();
		public: virtual void Update();
		private: void deviceLost();
		private: void deviceReset();
		public: virtual void EnableRenderTarget();
		public: virtual void EnableRenderTarget(DepthStencilI^ depthStencil);
		public: virtual void DisableRenderTarget();
		public: virtual void Present();
		public: virtual void ClearAll(float r, float g, float b, float a);
		public: virtual void ClearColor(float r, float g, float b, float a);
		public: virtual void ClearColorDepth(float r, float g, float b, float a);
		public: virtual void ClearDepthStencil();
		internal: void removeActiveTexture(Texture2D^ texture);
		internal: static D3DFORMAT surfaceFormat(SurfaceFormats surfaceFormat);
		#pragma endregion
	};
}
}
}