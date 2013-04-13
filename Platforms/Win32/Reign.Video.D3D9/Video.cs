using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class Caps
	{
		public bool ExDevice {get; internal set;}

		public bool HardwareInstancing {get; internal set;}
		public uint MaxTextureCount {get; internal set;}
		public ShaderVersions MaxVertexShaderVersion {get; internal set;}
		public ShaderVersions MaxPixelShaderVersion {get; internal set;}
		public ShaderVersions MaxShaderVersion {get; internal set;}
	}

	public class Video : Disposable, VideoI
	{
		#region Properties
		internal VideoCom com;
		private ApplicationI application;
		internal Texture2D[] currentVertexTextures, currentPixelTextures;
		public Size2 BackBufferSize {get; private set;}

		public delegate void DeviceLostMethod();
		public DeviceLostMethod DeviceLost, DeviceReset;
		internal bool deviceIsLost, deviceReseting;

		public Caps Caps {get; private set;}
		public string FileTag {get; private set;}
		#endregion

		#region Constructors
		public Video(DisposableI parent, ApplicationI application, DepthStenicFormats depthStencilFormats, bool vSync)
		: base(parent)
		{
			try
			{
				this.application = application;
				FileTag = "D3D9_";
				currentVertexTextures = new Texture2D[4];
				currentPixelTextures = new Texture2D[8];

				int depthBit = 16, stencilBit = 0;
				switch (depthStencilFormats)
				{
					case DepthStenicFormats.None:
						depthBit = 0;
						stencilBit = 0;
						break;

					case DepthStenicFormats.Defualt:
						depthBit = 24;
						stencilBit = 0;
						break;

					case DepthStenicFormats.Depth24Stencil8:
						depthBit = 24;
						stencilBit = 8;
						break;

					case DepthStenicFormats.Depth16:
						depthBit = 16;
						stencilBit = 0;
						break;

					case DepthStenicFormats.Depth32:
						depthBit = 32;
						stencilBit = 0;
						break;

					default:
						Debug.ThrowError("Video", "Unsuported DepthStencilFormat type");
						break;
				}

				com = new VideoCom();
				var frame = application.FrameSize;
				BackBufferSize = frame;
				ComponentCaps componentCaps;
				var error = com.Init(application.Handle, vSync, frame.Width, frame.Height, depthBit, stencilBit, false, false, out componentCaps);

				switch (error)
				{
					case VideoError.Direct3DInterfaceFailed: Debug.ThrowError("Video", "Failed to create Direct3D9 interface.\nDo you have up to date graphics drivers?"); break;
					case VideoError.GetCapsFailed: Debug.ThrowError("Video", "Failed to get caps"); break;
					case VideoError.AdapterDisplayModeFailed: Debug.ThrowError("Video", "Failed to get adapter display mode"); break;
					case VideoError.VideoHardwareNotSupported: Debug.ThrowError("Video", "Your video hardware is not supported"); break;
					case VideoError.DeviceAndSwapChainFailed: Debug.ThrowError("Video", "Failed to create Device and SwapChain"); break;
				}

				Caps = new Caps()
				{
					ExDevice = componentCaps.ExDevice,
					HardwareInstancing = componentCaps.HardwareInstancing,
					MaxTextureCount = componentCaps.MaxTextureCount,
					MaxVertexShaderVersion = getMaxShaderVersion(componentCaps.MaxVertexShaderVersion),
					MaxPixelShaderVersion = getMaxShaderVersion(componentCaps.MaxPixelShaderVersion),
					MaxShaderVersion = getMaxShaderVersion(componentCaps.MaxShaderVersion)
				};

				com.DeviceLost = deviceLost;
				com.DeviceReset = deviceReset;
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		private ShaderVersions getMaxShaderVersion(float version)
		{
			if (version == 3.0f) return ShaderVersions.HLSL_3_0;
			else if (version == 2.1f) return ShaderVersions.HLSL_2_a;
			else if (version == 2.0f) return ShaderVersions.HLSL_2_0;

			return ShaderVersions.Unknown;
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Update()
		{
			var frame = application.FrameSize;
			if (frame.Width != 0 && frame.Height != 0) BackBufferSize = frame;
			com.Update(frame.Width, frame.Height);
		}

		private void deviceLost()
		{
			deviceIsLost = true;
			deviceReseting = true;
			if (DeviceLost != null) DeviceLost();
		}

		private void deviceReset()
		{
			deviceIsLost = false;
			if (DeviceReset != null) DeviceReset();
			deviceReseting = false;
		}

		public void EnableRenderTarget()
		{
			com.EnableRenderTarget();
		}

		public void EnableRenderTarget(DepthStencilI depthStencil)
		{
			if (depthStencil != null) com.EnableRenderTarget(((DepthStencil)depthStencil).com);
			else com.EnableRenderTarget(null);
		}

		public void ClearAll(float r, float g, float b, float a)
		{
			com.ClearAll(r, g, b, a);
		}

		public void ClearColor(float r, float g, float b, float a)
		{
			com.ClearColor(r, g, b, a);
		}

		public void ClearColorDepth(float r, float g, float b, float a)
		{
			com.ClearColorDepth(r, g, b, a);
		}

		public void ClearDepthStencil()
		{
			com.ClearDepthStencil();
		}

		public void Present()
		{
			com.Present();
		}

		internal void removeActiveTexture(Texture2D texture)
		{
			var textures = currentVertexTextures;
			for (int i = 0; i != textures.Length; ++i)
			{
				if (textures[i] == texture)
				{
					com.DisableVertexTexture(i);
					textures[i] = null;
				}
			}

			textures = currentPixelTextures;
			for (int i = 0; i != textures.Length; ++i)
			{
				if (textures[i] == texture)
				{
					com.DisableTexture(i);
					textures[i] = null;
				}
			}
		}

		internal REIGN_D3DFORMAT surfaceFormat(SurfaceFormats surfaceFormat)
		{
		    switch (surfaceFormat)
		    {
				case SurfaceFormats.Defualt: return REIGN_D3DFORMAT.A8R8G8B8;
		        case SurfaceFormats.DXT1: return REIGN_D3DFORMAT.DXT1;
		        case SurfaceFormats.DXT3: return REIGN_D3DFORMAT.DXT3;
		        case SurfaceFormats.DXT5: return REIGN_D3DFORMAT.DXT5;
		        case SurfaceFormats.RGBAx8: return REIGN_D3DFORMAT.A8R8G8B8;
		        case SurfaceFormats.RGBx10_Ax2: return REIGN_D3DFORMAT.A2R10G10B10;
		        case SurfaceFormats.RGBAx16f: return REIGN_D3DFORMAT.A16B16G16R16F;
		        case SurfaceFormats.RGBAx32f: return REIGN_D3DFORMAT.A32B32G32R32F;

		        default:
		            Debug.ThrowError("Video", "Unsuported SurfaceFormat: " + surfaceFormat);
		            return REIGN_D3DFORMAT.A8R8G8B8;
		    }
		}
		#endregion
	}
}
