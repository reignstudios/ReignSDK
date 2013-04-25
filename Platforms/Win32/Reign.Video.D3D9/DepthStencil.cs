using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class DepthStencil : Disposable, DepthStencilI
	{
		#region Properties
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}

		internal DepthStencilCom com;

		private Video video;
		private int width, height;
		private DepthStencilFormats depthStencilFormats;
		#endregion

		#region Constructors
		public static DepthStencilI New(DisposableI parent, int width, int height, DepthStencilFormats depthStencilFormats)
		{
			return new DepthStencil(parent, width, height, depthStencilFormats);
		}

		public DepthStencil(DisposableI parent, int width, int height, DepthStencilFormats depthStencilFormats)
		: base(parent)
		{
			init(parent, width, height, depthStencilFormats);
		}

		private void init(DisposableI parent, int width, int height, DepthStencilFormats depthStencilFormats)
		{
			try
			{
				this.width = width;
				this.height = height;
				this.depthStencilFormats = depthStencilFormats;

				video = parent.FindParentOrSelfWithException<Video>();
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
					case DepthStencilErrors.DepthStencilSurface: Debug.ThrowError("DepthStencil", "Failed to create DepthStencil Surface"); break;
				}

				if (!video.Caps.ExDevice && !video.deviceReseting)
				{
					video.DeviceLost += deviceLost;
					video.DeviceReset += deviceReset;
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
			dispose();
			base.Dispose();
		}

		private void dispose()
		{
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
		}

		private void deviceLost()
		{
			if (!video.Caps.ExDevice) dispose();
		}

		private void deviceReset()
		{
			if (!video.Caps.ExDevice) init(Parent, width, height, depthStencilFormats);
		}
		#endregion
	}
}