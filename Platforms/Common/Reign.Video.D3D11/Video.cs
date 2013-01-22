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

		#if WINDOWS
		private Window window;
		#elif METRO
		private ApplicationI application;
		#else
		private Application application;
		#endif

		internal VideoCom com;
		public Caps Cap {get; private set;}
		#endregion

		#region Constructors
		#if WINDOWS
		public Video(DisposableI parent, Window window, bool vSync)
		: base(parent)
		{
			init(parent, window, vSync);
		}
		#elif METRO
		public Video(DisposableI parent, Application application, bool vSync)
		: base(parent)
		{
			init(parent, application, vSync, null);
		}

		public Video(DisposableI parent, XAMLApplication application, bool vSync)
		: base(parent)
		{
			init(parent, application, vSync, application.SwapChainPanel);
		}
		#else
		public Video(DisposableI parent, Application application, bool vSync)
		: base(parent)
		{
			init(parent, application, vSync);
		}
		#endif

		#if WINDOWS
		private void init(DisposableI parent, Window window, bool vSync)
		#elif METRO
		private void init(DisposableI parent, ApplicationI application, bool vSync, Windows.UI.Xaml.Controls.SwapChainBackgroundPanel swapChainBackgroundPanel)
		#else
		private void init(DisposableI parent, Application application, bool vSync)
		#endif
		{
			#if WINDOWS
			this.window = window;
			#else
			this.application = application;
			#endif
			
			try
			{
				FileTag = "D3D11_";
				Cap = new Caps();

				com = new VideoCom();
				var featureLevel = REIGN_D3D_FEATURE_LEVEL.LEVEL_9_1;
				#if WINDOWS
				var frame = window.FrameSize;
				var error = com.Init(window.Handle, vSync, frame.Width, frame.Height, false, out featureLevel);
				#elif METRO
				var frame = application.Metro_FrameSize;
				var error = com.Init(OS.CoreWindow, vSync, frame.Width, frame.Height, out featureLevel, swapChainBackgroundPanel);
				#else
				var frame = application.FrameSize;
				var error = com.Init(vSync, frame.Width, frame.Height, out featureLevel);
				#endif
				BackBufferSize = frame;

				switch (error)
				{
					case (VideoError.DepthStencilTextureFailed): Debug.ThrowError("Video", "Failed to create DepthStencilTexture"); break;
					case (VideoError.DepthStencilViewFailed): Debug.ThrowError("Video", "Failed to create DepthStencilView"); break;
					case (VideoError.RenderTargetViewFailed): Debug.ThrowError("Video", "Failed to create RenderTargetView"); break;
					#if !WP8
					case (VideoError.GetSwapChainFailed): Debug.ThrowError("Video", "Failed to get SwapChain"); break;
					#endif
					#if WINDOWS
					case (VideoError.DeviceAndSwapChainFailed): Debug.ThrowError("Video", "Failed to create Device and SwapChain"); break;
					#else
					case (VideoError.DeviceFailed): Debug.ThrowError("Video", "Failed to create Device"); break;
					#if !WP8
					case (VideoError.SwapChainFailed): Debug.ThrowError("Video", "Failed to create SwapChain"); break;
					case (VideoError.D2DFactoryFailed): Debug.ThrowError("Video", "Failed to create D2D Factory"); break;
					case (VideoError.D2DDeviceFailed): Debug.ThrowError("Video", "Failed to create D2D Device"); break;
					case (VideoError.D2DDeviceContextFailed): Debug.ThrowError("Video", "Failed to D2D DeviceContext"); break;
					case (VideoError.NativeSwapChainPanelFailed): Debug.ThrowError("Video", "Failed to get native SwapChainPanel"); break;
					case (VideoError.GetDXGIBackBufferFailed): Debug.ThrowError("Video", "Failed to create DXGI BackBuffer"); break;
					case (VideoError.DXGISurfaceFailed): Debug.ThrowError("Video", "Failed to create DXGI Surface"); break;
					case (VideoError.D2DBitmapFailed): Debug.ThrowError("Video", "Failed to create D2D Bitmap"); break;
					#else
					case (VideoError.RenderTextureFailed): Debug.ThrowError("Video", "Failed to create RenderTexture"); break;
					#endif
					#endif
				}

				switch (featureLevel)
				{
					#if METRO || WP8
					case (REIGN_D3D_FEATURE_LEVEL.LEVEL_11_1):
						Cap.MaxShaderVersion = ShaderVersions.HLSL_5_0;
						Cap.FeatureLevel = FeatureLevels.D3D11_1;
						break;
					#endif

					case (REIGN_D3D_FEATURE_LEVEL.LEVEL_11_0):
						Cap.MaxShaderVersion = ShaderVersions.HLSL_5_0;
						Cap.FeatureLevel = FeatureLevels.D3D11;
						break;

					case (REIGN_D3D_FEATURE_LEVEL.LEVEL_10_1):
						Cap.MaxShaderVersion = ShaderVersions.HLSL_4_1;
						Cap.FeatureLevel = FeatureLevels.D3D10_1;
						break;

					case (REIGN_D3D_FEATURE_LEVEL.LEVEL_10_0):
						Cap.MaxShaderVersion = ShaderVersions.HLSL_4_0;
						Cap.FeatureLevel = FeatureLevels.D3D10;
						break;

					case (REIGN_D3D_FEATURE_LEVEL.LEVEL_9_3):
						Cap.MaxShaderVersion = ShaderVersions.HLSL_3_0;
						Cap.FeatureLevel = FeatureLevels.D3D9_3;
						break;

					case (REIGN_D3D_FEATURE_LEVEL.LEVEL_9_2):
						Cap.MaxShaderVersion = ShaderVersions.HLSL_2_a;
						Cap.FeatureLevel = FeatureLevels.D3D9_2;
						break;

					case (REIGN_D3D_FEATURE_LEVEL.LEVEL_9_1):
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
			#if WINDOWS
			var frame = window.FrameSize;
			#elif METRO
			var frame = application.Metro_FrameSize;
			#else
			var frame = application.FrameSize;
			#endif

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
				case (SurfaceFormats.DXT1): return REIGN_DXGI_FORMAT.BC1_UNORM;
				case (SurfaceFormats.DXT3): return REIGN_DXGI_FORMAT.BC2_UNORM;
				case (SurfaceFormats.DXT5): return REIGN_DXGI_FORMAT.BC3_UNORM;
				case (SurfaceFormats.RGBAx8): return REIGN_DXGI_FORMAT.R8G8B8A8_UNORM;
				case (SurfaceFormats.RGBx10_Ax2): return REIGN_DXGI_FORMAT.R10G10B10A2_UNORM;
				case (SurfaceFormats.RGBAx16f): return REIGN_DXGI_FORMAT.R16G16B16A16_FLOAT;
				case (SurfaceFormats.RGBAx32f): return REIGN_DXGI_FORMAT.R32G32B32A32_FLOAT;
				default:
					Debug.ThrowError("Video", "Unsuported SurfaceFormat.");
					return REIGN_DXGI_FORMAT.R8G8B8A8_UNORM;
			}
		}
		#endregion
	}
}
