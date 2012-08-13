using System;
using X = Microsoft.Xna.Framework.Graphics;
using XF = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Reign.Core;
using System.IO;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class RenderTarget : Texture2D, RenderTargetI
	{
		#region Properties
		public X.RenderTarget2D renderTarget;
		#endregion

		#region Constructors
		public RenderTarget(DisposableI parent, string fileName)
		: base(parent, fileName)
		{}

		public RenderTarget(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage)
		{}

		protected override void init(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool isRenderTarget)
		{
			base.init(parent, fileName, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage, true);

			try
			{
				if (fileName == null)
				{
					// TODO: handle multiSampleType types
					var xUsage = X.RenderTargetUsage.PlatformContents;
					switch (renderTargetUsage)
					{
						case (RenderTargetUsage.PlatformDefault): xUsage = X.RenderTargetUsage.PlatformContents; break;
						case (RenderTargetUsage.PreserveContents): xUsage = X.RenderTargetUsage.PreserveContents; break;
						case (RenderTargetUsage.DiscardContents): xUsage = X.RenderTargetUsage.DiscardContents; break;
					}
					renderTarget = new X.RenderTarget2D(video.Device, width, height, false, Video.surfaceFormat(surfaceFormat), X.DepthFormat.None, 0, xUsage);
				}
				else
				{
					Debug.ThrowError("RenderTarget", "(Load image data into RenderTarget Texture) -- Not implemented yet...");
				}

				texture = renderTarget;
			}
			catch (Exception ex)
			{
				Dispose();
				throw ex;
			}
		}
		#endregion

		#region Methods
		public void Enable()
		{
			video.Device.SetRenderTarget(renderTarget);
		}

		public void Enable(DepthStencilI depthStencil)
		{
			if (depthStencil == null)
			{
				video.Device.SetRenderTarget(renderTarget);
			}
			else
			{
				var bindings = new X.RenderTargetBinding[2]
				{
					renderTarget,
					((DepthStencil)depthStencil).depthStencil
				};
				video.Device.SetRenderTargets(bindings);
			}
		}
		#endregion
	}
}