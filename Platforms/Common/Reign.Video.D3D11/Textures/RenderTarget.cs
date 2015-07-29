using Reign.Core;
using Reign_Video_D3D11_Component;
using System;

namespace Reign.Video.D3D11
{
	public class RenderTarget : Texture2D, IRenderTarget
	{
		#region Properties
		private RenderTargetCom renderTargetCom;
		private DepthStencil depthStencil;
		private bool initSuccess;
		#endregion

		#region Constructors
		public RenderTarget(IDisposableResource parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, width, height, surfaceFormat, usage, loadedCallback)
		{
			if (initSuccess) initDepthStencil(width, height, depthStencilFormat);
		}

		public RenderTarget(IDisposableResource parent, string filename, MultiSampleTypes multiSampleType, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, filename, false, usage, loadedCallback)
		{
		}

		protected override bool init(IDisposableResource parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, Loader.LoadedCallbackMethod loadedCallback)
		{
			initSuccess = base.init(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, true, loadedCallback);
			if (!initSuccess) return false;

			try
			{
				if (usage == BufferUsages.Write) Debug.ThrowError("RenderTarget", "Only Textures may be writable");

				if (image != null)
				{
					width = image.Size.Width;
					height = image.Size.Height;
					surfaceFormat = image.SurfaceFormat;
				}

				renderTargetCom = new RenderTargetCom();
				var error = renderTargetCom.Init(video.com, width, height, com, 0, video.surfaceFormat(surfaceFormat), usage == BufferUsages.Read);

				switch (error)
				{
					case RenderTargetError.RenderTargetView: Debug.ThrowError("RenderTarget", "Failed to create RenderTargetView"); break;
					case RenderTargetError.StagingTexture: Debug.ThrowError("RenderTarget", "Failed to create Staging Texture"); break;
				}
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(this, false);
				return false;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
			return true;
		}

		private void initDepthStencil(int width, int height, DepthStencilFormats depthStencilFormat)
		{
			if (depthStencilFormat != DepthStencilFormats.None) depthStencil = new DepthStencil(this, width, height, depthStencilFormat);
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (renderTargetCom != null)
			{
				renderTargetCom.Dispose();
				renderTargetCom = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			if (depthStencil != null) renderTargetCom.Enable(depthStencil.com);
			else renderTargetCom.Enable();
		}

		public void Enable(IDepthStencil depthStencil)
		{
			renderTargetCom.Enable(((DepthStencil)depthStencil).com);
		}

		#if WP8
		public void ReadPixels(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void ReadPixels(Color4[] colors)
		{
			throw new NotImplementedException();
		}
		#else
		public unsafe void ReadPixels(byte[] data)
		{
			fixed (byte* ptr = data)
			{
				renderTargetCom.ReadPixels((int)ptr, data.Length);
			}
		}

		public unsafe void ReadPixels(Color4[] colors)
		{
			fixed (Color4* ptr = colors)
			{
				renderTargetCom.ReadPixels((int)ptr, colors.Length * 4);
			}
		}
		#endif

		public bool ReadPixel(Point2 position, out Color4 color)
		{
			if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
			{
				color = new Color4();
				return false;
			}

			color = new Color4(renderTargetCom.ReadPixel(position.X, position.Y, Size.Height));
			return true;
		}
		#endregion
	}
}