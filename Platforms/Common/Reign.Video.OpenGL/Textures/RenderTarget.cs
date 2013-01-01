using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class RenderTarget : Texture2D, RenderTargetI
	{
		#region Properties
		private uint frameBuffer;
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

		protected unsafe override bool init(DisposableI parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, Loader.LoadedCallbackMethod loadedCallback)
		{
			if (!base.init(parent, image, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage, usage, true, loadedCallback)) return false;
			
			try
			{
				if (usage == BufferUsages.Write) Debug.ThrowError("RenderTarget", "Only Textures may be writable");

				uint frameBufferTEMP = 0;
				GL.GenFramebuffers(1, &frameBufferTEMP);
				frameBuffer = frameBufferTEMP;
				
				uint error;
				string errorName;
				if (Video.checkForError(out error, out errorName)) Debug.ThrowError("RenderTarget", string.Format("{0} {1}: Failed to create renderTarget", error, errorName));
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

		public unsafe override void Dispose()
		{
		    disposeChilderen();
		    if (frameBuffer != 0)
		    {
		    	if (!OS.AutoDisposedGL)
		    	{
					uint frameBufferTEMP = frameBuffer;
					GL.BindFramebuffer(GL.FRAMEBUFFER, 0);
			        GL.DeleteFramebuffers(1, &frameBufferTEMP);
		        }
		        frameBuffer = 0;

				#if DEBUG && !ANDROID
				Video.checkForError();
				#endif
		    }
		    base.Dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			// TODO: disable unsused active renderTargets
			video.disableActiveTextures(this);
			GL.BindFramebuffer(GL.FRAMEBUFFER, frameBuffer);
			GL.FramebufferTexture2D(GL.FRAMEBUFFER, GL.COLOR_ATTACHMENT0, GL.TEXTURE_2D, Texture, 0);
			GL.BindRenderbuffer(GL.RENDERBUFFER, 0);

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public void Enable(DepthStencilI depthStencil)
		{
			video.disableActiveTextures(this);
			GL.BindFramebuffer(GL.FRAMEBUFFER, frameBuffer);
			GL.FramebufferTexture2D(GL.FRAMEBUFFER, GL.COLOR_ATTACHMENT0, GL.TEXTURE_2D, Texture, 0);

			if (depthStencil != null)
			{
				((DepthStencil)depthStencil).enable();
			}
			else
			{
				GL.BindRenderbuffer(GL.RENDERBUFFER, 0);
			}

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public void ReadPixels(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void ReadPixels(Color4[] colors)
		{
			throw new NotImplementedException();
		}

		public unsafe bool ReadPixel(Point2 position, out Color4 color)
		{
			// make sure position is within the texture bounds
			if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
			{
				color = new Color4();
				return false;
			}

			// TODO: make sure i'm the active render target

			// read data
			int data;
			GL.ReadPixels(position.X, position.Y, 1, 1, GL.RGBA, GL.UNSIGNED_BYTE, &data);
			color = new Color4(data);
			return true;
		}
		#endregion
	}
}