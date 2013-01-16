using Reign.Core;
using System;

namespace Reign.Video.OpenGL
{
	public class DepthStencil : Disposable, DepthStencilI
	{
		#region Properties
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}

		internal uint surface;
		#endregion

		#region Constructors
		public static DepthStencilI New(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		{
			return new DepthStencil(parent, width, height, depthStenicFormats);
		}

		public unsafe DepthStencil(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		: base(parent)
		{
			try
			{
				Size = new Size2(width, height);
				SizeF = Size.ToVector2();

				uint surfaceTEMP = 0;
				GL.GenRenderbuffers(1, &surfaceTEMP);
				surface = surfaceTEMP;

				GL.BindRenderbuffer(GL.RENDERBUFFER, surface);
				GL.RenderbufferStorage(GL.RENDERBUFFER, GL.DEPTH_COMPONENT16, width, height);

				uint error;
				string errorName;
				if (Video.checkForError(out error, out errorName)) Debug.ThrowError("DepthStencil", string.Format("{0} {1}: Failed to create DepthStencil", error, errorName));
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public unsafe override void Dispose()
		{
		    disposeChilderen();
		    if (surface != 0)
		    {
		    	if (!OS.AutoDisposedGL)
		    	{
					uint SurfaceTEMP = surface;
					GL.BindRenderbuffer(GL.RENDERBUFFER, 0);
			        GL.DeleteRenderbuffers(1, &SurfaceTEMP);
		        }
		        surface = 0;

				#if DEBUG && !ANDROID
				Video.checkForError();
				#endif
		    }
		    base.Dispose();
		}
		#endregion

		#region Methods
		internal void enable()
		{
			GL.BindRenderbuffer(GL.RENDERBUFFER, surface);
			GL.FramebufferRenderbuffer(GL.FRAMEBUFFER, GL.DEPTH_ATTACHMENT, GL.RENDERBUFFER, surface);

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}