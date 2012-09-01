using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public enum FeatureLevels
	{
		#if METRO
		D3D11_1,
		#endif
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

		#if WINDOWS
		private Window window;
		#else
		private Application application;
		#endif

		internal VideoCom com;
		internal Size2 windowFrameSize;
		public Caps Cap {get; private set;}
		#endregion

		#region Constructors
		#if WINDOWS
		public Video(DisposableI parent, Window window, bool vSync)
		: base(parent)
		#else
		public Video(DisposableI parent, Application application, bool vSync)
		: base(parent)
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
				#else
				var frame = application.FrameSize;
				windowFrameSize = frame;
				var error = com.Init(OS.CurrentApplication.CoreWindow, vSync, frame.Width, frame.Height, out featureLevel);
				#endif

				switch (error)
				{
					case (VideoError.DepthStencilTextureFailed): Debug.ThrowError("Video", "Failed to create DepthStencilTexture"); break;
					case (VideoError.DepthStencilViewFailed): Debug.ThrowError("Video", "Failed to create DepthStencilView"); break;
					case (VideoError.GetSwapChainFailed): Debug.ThrowError("Video", "Failed to get SwapChain"); break;
					case (VideoError.RenderTargetViewFailed): Debug.ThrowError("Video", "Failed to create RenderTargetView"); break;
					#if WINDOWS
					case (VideoError.DeviceAndSwapChainFailed): Debug.ThrowError("Video", "Failed to create Device and SwapChain"); break;
					#else
					case (VideoError.DeviceFailed): Debug.ThrowError("Video", "Failed to create Device"); break;
					case (VideoError.SwapChainFailed): Debug.ThrowError("Video", "Failed to create SwapChain"); break;
					#endif
				}

				switch (featureLevel)
				{
					#if METRO
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
			#else
			var frame = application.FrameSize;
			#endif

			if (frame.Width != 0 && frame.Height != 0) windowFrameSize = frame;
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

		public void Clear(float r, float g, float b, float a)
		{
			com.Clear(r, g, b, a);
		}

		public void ClearColor(float r, float g, float b, float a)
		{
			com.ClearColor(r, g, b, a);
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
