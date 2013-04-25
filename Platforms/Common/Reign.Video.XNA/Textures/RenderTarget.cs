using System;
using X = Microsoft.Xna.Framework.Graphics;
using XF = Microsoft.Xna.Framework;
using Reign.Core;
using System.IO;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class RenderTarget : Texture2D, RenderTargetI
	{
		#region Properties
		public X.RenderTarget2D renderTarget;
		private DepthStencilFormats initDepthStencilFormat;
		#endregion

		#region Constructors
		public static RenderTarget New(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new RenderTarget(parent, width, height, multiSampleType, surfaceFormat, depthStencilFormat, usage, renderTargetUsage, loadedCallback);
		}

		public static RenderTarget New(DisposableI parent, string fileName, MultiSampleTypes multiSampleType, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new RenderTarget(parent, fileName, multiSampleType, usage, renderTargetUsage, loadedCallback);
		}

		public RenderTarget(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, width, height, surfaceFormat, usage, loadedCallback)
		{
			initDepthStencilFormat = depthStencilFormat;
		}

		public RenderTarget(DisposableI parent, string fileName, MultiSampleTypes multiSampleType, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent, fileName, false, usage, loadedCallback)
		{
		}

		protected override bool init(DisposableI parent, string fileName, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, Loader.LoadedCallbackMethod loadedCallback)
		{
			if (!base.init(parent, fileName, image, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage, usage, true, loadedCallback)) return false;

			try
			{
				if (usage == BufferUsages.Write) Debug.ThrowError("RenderTarget", "Only Textures may be writable");

				if (fileName == null)
				{
					// TODO: handle multiSampleType types
					#if SILVERLIGHT
					var xUsage = X.RenderTargetUsage.PreserveContents;
					#else
					var xUsage = X.RenderTargetUsage.PlatformContents;
					#endif
					switch (renderTargetUsage)
					{
						case RenderTargetUsage.PlatformDefault:
							#if SILVERLIGHT
							xUsage = X.RenderTargetUsage.PreserveContents;
							#else
							xUsage = X.RenderTargetUsage.PlatformContents;
							#endif
							break;

						case RenderTargetUsage.PreserveContents: xUsage = X.RenderTargetUsage.PreserveContents; break;
						case RenderTargetUsage.DiscardContents: xUsage = X.RenderTargetUsage.DiscardContents; break;
					}

					X.DepthFormat format = X.DepthFormat.None;
					if (initDepthStencilFormat != DepthStencilFormats.None)
					{
						switch (initDepthStencilFormat)
						{
							case DepthStencilFormats.Defualt: format = X.DepthFormat.Depth24; break;
							case DepthStencilFormats.Depth24Stencil8: format = X.DepthFormat.Depth24Stencil8; break;
							case DepthStencilFormats.Depth16: format = X.DepthFormat.Depth16; break;
							case DepthStencilFormats.Depth24: format = X.DepthFormat.Depth24; break;

							default:
								Debug.ThrowError("RenderTarget", "Unsuported DepthStencilFormat type");
								break;
						}
					}

					renderTarget = new X.RenderTarget2D(video.Device, width, height, false, Video.surfaceFormat(surfaceFormat), format, 0, xUsage);
				}
				else
				{
					Debug.ThrowError("RenderTarget", "(Load image data into RenderTarget Texture) -- Not implemented yet...");
				}

				texture = renderTarget;
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(this, false);
				return false;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
			return true;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			video.Device.SetRenderTarget(renderTarget);
		}

		public void Enable(DepthStencilI depthStencil)
		{
			Debug.ThrowError("RenderTarget", "DepthStencils are build into Rendertargets, thus this method does not apply to XNA");
		}

		public void ReadPixels(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void ReadPixels(Color4[] colors)
		{
			throw new NotImplementedException();
		}

		public bool ReadPixel(Point2 position, out Color4 color)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}