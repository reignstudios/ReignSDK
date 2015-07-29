using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class DepthStencil : DisposableResource, IDepthStencil
	{
		#region Properties
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}

		internal DepthStencilCom com;
		#endregion

		#region Constructors
		public DepthStencil(IDisposableResource parent, int width, int height, DepthStencilFormats depthStencilFormats)
		: base(parent)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();
				Size = new Size2(width, height);
				SizeF = Size.ToVector2();

				int depthBit = 16, stencilBit = 0;
				switch (depthStencilFormats)
				{
					case DepthStencilFormats.Defualt:
						depthBit = 24;
						stencilBit = 0;
						break;

					case DepthStencilFormats.Depth24Stencil8:
						depthBit = 24;
						stencilBit = 8;
						break;

					case DepthStencilFormats.Depth16:
						depthBit = 16;
						stencilBit = 0;
						break;

					case DepthStencilFormats.Depth24:
						depthBit = 24;
						stencilBit = 0;
						break;

					case DepthStencilFormats.Depth32:
						depthBit = 32;
						stencilBit = 0;
						break;

					default:
						Debug.ThrowError("Video", "Unsuported DepthStencilFormat type");
						break;
				}

				com = new DepthStencilCom();
				var error = com.Init(video.com, width, height, depthBit, stencilBit);

				switch (error)
				{
					case DepthStencilErrors.Textrue: Debug.ThrowError("DepthStencil", "Failed to create Texture2D"); break;
					case DepthStencilErrors.DepthStencilView: Debug.ThrowError("DepthStencil", "Failed to create DepthStencilView"); break;
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
	}
}