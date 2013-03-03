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
		public static DepthStencilI New(DisposableI parent, int width, int height, DepthStenicFormats depthStencilFormats)
		{
			return new DepthStencil(parent, width, height, depthStencilFormats);
		}

		public DepthStencil(DisposableI parent, int width, int height, DepthStenicFormats depthStencilFormats)
		: base(parent)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				
				PixelFormat format = PixelFormat.None;
				switch (depthStencilFormats)
				{
					case DepthStenicFormats.None: format = PixelFormat.None; break;
					case DepthStenicFormats.Defualt: format = PixelFormat.Depth16; break;
					case DepthStenicFormats.Depth24Stencil8: format = PixelFormat.Depth24Stencil8; break;
					case DepthStenicFormats.Depth16: format = PixelFormat.Depth16; break;

					default:
						Debug.ThrowError("Video", "Unsuported DepthStencilFormat type");
						break;
				}
				
				depthBuffer = new DepthBuffer(width, height, format);
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