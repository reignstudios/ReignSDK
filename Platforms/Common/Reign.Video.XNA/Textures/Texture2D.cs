using System;
using X = Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Reign.Core;
using System.IO;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class Texture2D : Disposable, Texture2DI
	{
		#region Properties
		protected Video video;
		internal X.Texture2D texture;
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}
		public Vector2 TexelOffset {get; private set;}
		private bool loadedFromContentManager;
		#endregion

		#region Constructors
		public Texture2D(DisposableI parent, string fileName)
		: base(parent)
		{
			init(parent, fileName, 0, 0, false, MultiSampleTypes.None, SurfaceFormats.RGBAx8, RenderTargetUsage.PlatformDefault, false);
		}

		public Texture2D(DisposableI parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat)
		: base(parent)
		{
			init(parent, null, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage.PlatformDefault, false);
		}

		protected Texture2D(DisposableI parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent)
		{
			init(parent, null, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, false);
		}
		
		protected virtual void init(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool isRenderTarget)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();

				if (!isRenderTarget)
				{
					if (fileName != null)
					{
						texture = parent.FindParentOrSelfWithException<RootDisposable>().Content.Load<X.Texture2D>(Streams.StripFileExt(fileName));
						loadedFromContentManager = true;
					}
					else
					{
						texture = new X.Texture2D(video.Device, width, height, generateMipmaps, Video.surfaceFormat(surfaceFormat));
					}

					Size = new Size2(texture.Width, texture.Height);
				}
				else
				{
					Size = new Size2(width, height);
				}

				TexelOffset = (1 / Size.ToVector2()) * .5f;
				SizeF = Size.ToVector2();
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
			if (texture != null && !loadedFromContentManager)
			{
				texture.Dispose();
				texture = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Update(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void UpdateDynamic(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void Copy(Texture2DI texture)
		{
			throw new NotImplementedException();
		}

		public byte[] Copy()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}