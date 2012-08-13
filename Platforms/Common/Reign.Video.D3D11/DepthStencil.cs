using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class DepthStencil : Disposable, DepthStencilI
	{
		#region Properties
		internal DepthStencilCom com;
		#endregion

		#region Constructors
		public DepthStencil(DisposableI parent, int width, int height)
		: base(parent)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();
				com = new DepthStencilCom();
				var error = com.Init(video.com, width, height);

				switch (error)
				{
					case (DepthStencilErrors.Textrue): Debug.ThrowError("DepthStencil", "Failed to create Texture2D"); break;
					case (DepthStencilErrors.DepthStencilView): Debug.ThrowError("DepthStencil", "Failed to create DepthStencilView"); break;
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