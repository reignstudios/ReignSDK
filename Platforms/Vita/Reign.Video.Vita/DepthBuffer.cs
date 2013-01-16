using Reign.Core;
using System;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class DepthStencil : Disposable, DepthStencilI
	{
		#region Properties
		private Video video;
		internal DepthBuffer depthBuffer;
		
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}
		#endregion

		#region Constructors
		public static DepthStencilI New(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		{
			return new DepthStencil(parent, width, height, depthStenicFormats);
		}

		public DepthStencil(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		: base(parent)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				depthBuffer = new DepthBuffer(width, height, PixelFormat.Depth16);
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
		    if (depthBuffer != null)
		    {
		    	depthBuffer.Dispose();
		    	depthBuffer = null;
		    }
		    base.Dispose();
		}
		#endregion

		#region Methods
		internal void enable()
		{
			video.currentFrameBuffer.SetDepthTarget(depthBuffer);
		}
		#endregion
	}
}