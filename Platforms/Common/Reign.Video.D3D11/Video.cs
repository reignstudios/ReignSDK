using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public enum FeatureLevels
	{
		D3D11_1,
		D3D11,
		D3D10_1,
		D3D10,
		D3D9_3,
		D3D9_2,
		D3D9_1
	};

	public class Caps
	{
		public FeatureLevels FeatureLevel {get; internal set;}
		public ShaderVersions MaxShaderVersion {get; internal set;}
	}

	public class Video : Disposable, VideoI
	{
		#region Properties
		public string FileTag {get; private set;}
		public Size2 BackBufferSize {get; private set;}
		private ApplicationI application;

		internal VideoCom com;
		public Caps Cap {get; private set;}
		#endregion

		#region Constructors
		public Video(DisposableI parent, ApplicationI application, DepthStenicFormats depthStencilFormats, bool vSync)
		: base(parent)
		{
			#if WINRT
			var xamlApp = application as XAMLApplication;
			init(parent, application, depthStencilFormats, vSync, xamlApp != null ? ((XAMLApplication)application).SwapChainPanel : null);
			#else
			init(parent, application, depthStencilFormats, vSync);
			#endif
		}

		#if WINRT
		private void init(DisposableI parent, ApplicationI application, DepthStenicFormats depthStencilFormats, bool vSync, Windows.UI.Xaml.Controls.SwapChainBackgroundPanel swapChainBackgroundPanel)
		#else
		private void init(DisposableI parent, ApplicationI application, DepthStenicFormats depthStencilFormats, bool vSync)
		#endif
		{
			this.application = application;
			
			try
			{
				FileTag = "D3D11_";
				Cap = new Caps();

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
				var featureLevel = REIGN_D3D_FEATURE_LEVEL.LEVEL_9_1;
				var frame = application.FrameSize;
				#if WIN32
				var error = com.Init(application.Handle, vSync, frame.Width, frame.Height, depthBit, stencilBit, false, out featureLevel);
				#elif WINRT
				var error = com.Init(application.CoreWindow, vSync, frame.Width, frame.Height, depthBit, stencilBit, out featureLevel, swapChainBackgroundPanel);
				#else
				var error = com.Init(vSync, frame.Width, frame.Height, depthBit, stencilBit, out featureLevel, OS.UpdateAndRender);
				#endif
				BackBufferSize = frame;

				switch (error)
				{
					case VideoError.DepthStencilTextureFailed: Debug.ThrowError("Video", "Failed to create DepthStencilTexture"); break;
					case VideoError.DepthStencilViewFailed: Debug.ThrowError("Video", "Failed to create DepthStencilView"); break;
					case VideoError.RenderTargetViewFailed: Debug.ThrowError("Video", "Failed to create RenderTargetView"); break;
					#if !WP8
					case VideoError.GetSwapChainFailed: Debug.ThrowError("Video", "Failed to get SwapChain"); break;
					#endif
					#if WIN32
					case VideoError.DeviceAndSwapChainFailed: Debug.ThrowError("Video", "Failed to create Device and SwapChain"); break;
					#else
					case VideoError.DeviceFailed: Debug.ThrowError("Video", "Failed to create Device"); break;
					#if !WP8
					case VideoError.SwapChainFailed: Debug.ThrowError("Video", "Failed to create SwapChain"); break;
					case VideoError.D2DFactoryFailed: Debug.ThrowError("Video", "Failed to create D2D Factory"); break;
					case VideoError.D2DDeviceFailed: Debug.ThrowError("Video", "Failed to create D2D Device"); break;
					case VideoError.D2DDeviceContextFailed: Debug.ThrowError("Video", "Failed to D2D DeviceContext"); break;
					case VideoError.NativeSwapChainPanelFailed: Debug.ThrowError("Video", "Failed to get native SwapChainPanel"); break;
					case VideoError.GetDXGIBackBufferFailed: Debug.ThrowError("Video", "Failed to create DXGI BackBuffer"); break;
					case VideoError.DXGISurfaceFailed: Debug.ThrowError("Video", "Failed to create DXGI Surface"); break;
					case VideoError.D2DBitmapFailed: Debug.ThrowError("Video", "Failed to create D2D Bitmap"); break;
					#else
					case VideoError.RenderTextureFailed: Debug.ThrowError("Video", "Failed to create RenderTexture"); break;
					#endif
					#endif
				}

				#if WP8
				((XAMLApplication)application).MainPage.Surface.SetContentProvider(com.GetProvider());
				#endif

				switch (featureLevel)
				{
					#if WINRT || WP8
					case REIGN_D3D_FEATURE_LEVEL.LEVEL_11_1:
						Cap.MaxShaderVersion = ShaderVersions.HLSL_5_0;
						Cap.FeatureLevel = FeatureLevels.D3D11_1;
						break;
					#endif

					case REIGN_D3D_FEATURE_LEVEL.LEVEL_11_0:
						Cap.MaxShaderVersion = ShaderVersions.HLSL_5_0;
						Cap.FeatureLevel = FeatureLevels.D3D11;
						break;

					case REIGN_D3D_FEATURE_LEVEL.LEVEL_10_1:
						Cap.MaxShaderVersion = ShaderVersions.HLSL_4_1;
						Cap.FeatureLevel = FeatureLevels.D3D10_1;
						break;

					case REIGN_D3D_FEATURE_LEVEL.LEVEL_10_0:
						Cap.MaxShaderVersion = ShaderVersions.HLSL_4_0;
						Cap.FeatureLevel = FeatureLevels.D3D10;
						break;

					case REIGN_D3D_FEATURE_LEVEL.LEVEL_9_3:
						Cap.MaxShaderVersion = ShaderVersions.HLSL_3_0;
						Cap.FeatureLevel = FeatureLevels.D3D9_3;
						break;

					case REIGN_D3D_FEATURE_LEVEL.LEVEL_9_2:
						Cap.MaxShaderVersion = ShaderVersions.HLSL_2_a;
						Cap.FeatureLevel = FeatureLevels.D3D9_2;
						break;

					case REIGN_D3D_FEATURE_LEVEL.LEVEL_9_1:
						Cap.MaxShaderVersion = ShaderVersions.HLSL_2_0;
						Cap.FeatureLevel = FeatureLevels.D3D9_1;
						break;
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
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

		internal REIGN_DXGI_FORMAT surfaceFormat(SurfaceFormats surfaceFormat)
		{
			switch (surfaceFormat)
			{
				case SurfaceFormats.Defualt: return REIGN_DXGI_FORMAT.R8G8B8A8_UNORM;
				case SurfaceFormats.DXT1: return REIGN_DXGI_FORMAT.BC1_UNORM;
				case SurfaceFormats.DXT3: return REIGN_DXGI_FORMAT.BC2_UNORM;
				case SurfaceFormats.DXT5: return REIGN_DXGI_FORMAT.BC3_UNORM;
				case SurfaceFormats.RGBAx8: return REIGN_DXGI_FORMAT.R8G8B8A8_UNORM;
				case SurfaceFormats.RGBx10_Ax2: return REIGN_DXGI_FORMAT.R10G10B10A2_UNORM;
				case SurfaceFormats.RGBAx16f: return REIGN_DXGI_FORMAT.R16G16B16A16_FLOAT;
				case SurfaceFormats.RGBAx32f: return REIGN_DXGI_FORMAT.R32G32B32A32_FLOAT;
				default:
					Debug.ThrowError("Video", "Unsuported SurfaceFormat: " + surfaceFormat);
					return REIGN_DXGI_FORMAT.R8G8B8A8_UNORM;
			}
		}
		#endregion
	}
}
