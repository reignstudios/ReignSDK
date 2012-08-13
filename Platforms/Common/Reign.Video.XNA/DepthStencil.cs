using System;
using X = Microsoft.Xna.Framework.Graphics;
using Reign.Core;
using System.IO;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class DepthStencil : Disposable, DepthStencilI
	{
		#region Properties
		private Video video;
		internal X.RenderTarget2D depthStencil;
		#endregion

		#region Constructors
		public DepthStencil(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		: base(parent)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				depthStencil = new X.RenderTarget2D(video.Device, width, height, false, X.SurfaceFormat.Color, X.DepthFormat.Depth24Stencil8);
			}
			catch (Exception ex)
			{
				Dispose();
				throw ex;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (depthStencil != null)
			{
				depthStencil.Dispose();
				depthStencil = null;
			}
			base.Dispose();
		}
		#endregion
	}
}