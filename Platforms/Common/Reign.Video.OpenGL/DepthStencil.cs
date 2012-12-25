using Reign.Core;
using System;

namespace Reign.Video.OpenGL
{
	public class DepthStencil : Disposable, DepthStencilI
	{
		#region Properties
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}

		public uint Surface {get; private set;}
		#endregion

		#region Constructors
		public static DepthStencilI New(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		{
			return new DepthStencil(parent, width, height, depthStenicFormats);
		}

		public unsafe DepthStencil(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		: base(parent)
		{
			Size = new Size2(width, height);
			SizeF = Size.ToVector2();

			uint surface = 0;
			GL.GenRenderbuffers(1, &surface);
			Surface = surface;

			GL.BindRenderbuffer(GL.RENDERBUFFER, surface);
			GL.RenderbufferStorage(GL.RENDERBUFFER, GL.DEPTH_COMPONENT16, width, height);

			uint error;
			string errorName;
			if (Video.checkForError(out error, out errorName)) Debug.ThrowError("DepthStencil", string.Format("{0} {1}: Failed to create DepthStencil", error, errorName));
		}

		public unsafe override void Dispose()
		{
		    disposeChilderen();
		    if (Surface != 0)
		    {
		    	if (!OS.AutoDisposedGL)
		    	{
					uint SurfaceTEMP = Surface;
					GL.BindRenderbuffer(GL.RENDERBUFFER, 0);
			        GL.DeleteRenderbuffers(1, &SurfaceTEMP);
		        }
		        Surface = 0;

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
			GL.BindRenderbuffer(GL.RENDERBUFFER, Surface);
			GL.FramebufferRenderbuffer(GL.FRAMEBUFFER, GL.DEPTH_ATTACHMENT, GL.RENDERBUFFER, Surface);

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}