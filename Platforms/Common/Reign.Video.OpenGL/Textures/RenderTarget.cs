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
		public RenderTarget(DisposableI parent, string fileName)
		: base(parent, fileName)
		{}

		public RenderTarget(DisposableI parent, string fileName, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent, fileName, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage)
		{}

		public RenderTarget(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage)
		{}

		protected unsafe override void init(DisposableI parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			base.init(parent, image, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage, usage, true);
			
			try
			{
				uint frameBufferTEMP = 0;
				GL.GenFramebuffers(1, &frameBufferTEMP);
				frameBuffer = frameBufferTEMP;
				
				uint error;
				string errorName;
				if (Video.checkForError(out error, out errorName)) Debug.ThrowError("RenderTarget", string.Format("{0} {1}: Failed to create renderTarget", error, errorName));
			}
			catch (Exception ex)
			{
				Dispose();
				throw ex;
			}
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
		#endregion
	}
}