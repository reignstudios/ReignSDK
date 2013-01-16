using Reign.Core;
using Reign.Video;
using System;
using G = Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class RenderTarget : Texture2D, RenderTargetI
	{
		#region Properties
		private G.FrameBuffer frameBuffer;
		#endregion

		#region Constructors
		public static RenderTarget New(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new RenderTarget(parent, width, height, multiSampleType, surfaceFormat, usage, renderTargetUsage, loadedCallback);
		}

		public static RenderTarget New(DisposableI parent, string fileName, MultiSampleTypes multiSampleType, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new RenderTarget(parent, fileName, multiSampleType, usage, renderTargetUsage, loadedCallback);
		}

		public RenderTarget(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, width, height, surfaceFormat, usage, loadedCallback)
		{
		}

		public RenderTarget(DisposableI parent, string fileName, MultiSampleTypes multiSampleType, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, fileName, false, usage, loadedCallback)
		{
		}

		protected override bool init(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, Loader.LoadedCallbackMethod loadedCallback)
		{
			if (!base.init(parent, fileName, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage, usage, true, loadedCallback)) return false;
			
			try
			{
				if (usage == BufferUsages.Write) Debug.ThrowError("RenderTarget", "Only Textures may be writable");

				frameBuffer = new G.FrameBuffer();
				frameBuffer.SetColorTarget(texture, 0);
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
		#endregion

		#region Methods
		public void Enable()
		{
			// TODO: disable unsused active renderTargets
			video.disableActiveTextures(this);
			video.context.SetFrameBuffer(frameBuffer);
		}

		public void Enable(DepthStencilI depthStencil)
		{
			frameBuffer.SetDepthTarget(((DepthStencil)depthStencil).depthBuffer);
			video.context.SetFrameBuffer(frameBuffer);
		}

		public void ReadPixels(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void ReadPixels(Color4[] colors)
		{
			throw new NotImplementedException();
		}

		public bool ReadPixel(Point2 position, out Color4 color)
		{
			// make sure position is within the texture bounds
			if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
			{
				color = new Color4();
				return false;
			}

			// TODO: make sure i'm the active render target

			// read data
			byte[] data = new byte[4];
			video.context.ReadPixels(data, Sce.PlayStation.Core.Graphics.PixelFormat.Rgba, position.X, position.Y, 1, 1);
			color = new Color4(data[0], data[1], data[2], data[3]);
			return true;
		}
		#endregion
	}
}

