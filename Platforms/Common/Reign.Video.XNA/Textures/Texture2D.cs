using System;
using X = Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Reign.Core;
using System.IO;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	class Texture2DStreamLoader : StreamLoaderI
	{
		private Texture2D texture;
		private DisposableI parent;
		private string fileName;
		private int width, height;
		private bool generateMipmaps;
		private MultiSampleTypes multiSampleType;
		private SurfaceFormats surfaceFormat;
		private RenderTargetUsage renderTargetUsage;
		private BufferUsages usage;
		private bool isRenderTarget;

		public Texture2DStreamLoader(Texture2D texture, DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			this.texture = texture;
			this.parent = parent;
			this.fileName = fileName;
			this.width = width;
			this.height = height;
			this.generateMipmaps = generateMipmaps;
			this.multiSampleType = multiSampleType;
			this.surfaceFormat = surfaceFormat;
			this.renderTargetUsage = renderTargetUsage;
			this.usage = usage;
			this.isRenderTarget = isRenderTarget;
		}

		public override bool Load()
		{
			texture.load(parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, isRenderTarget);
			return true;
		}
	}

	public class Texture2D : Disposable, Texture2DI
	{
		#region Properties
		public bool Loaded {get; private set;}
		protected Video video;
		internal X.Texture2D texture;
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}
		public Vector2 TexelOffset {get; private set;}
		private bool loadedFromContentManager;
		#endregion

		#region Constructors
		public static Texture2DI New(DisposableI parent, string fileName)
		{
			var texture = parent.FindChild<Texture2D>("New",
				new ConstructorParam(typeof(DisposableI), parent),
				new ConstructorParam(typeof(string), fileName));
			if (texture != null)
			{
				++texture.referenceCount;
				return texture;
			}
			return new Texture2D(parent, fileName);
		}

		public Texture2D(DisposableI parent, string fileName)
		: base(parent)
		{
			new Texture2DStreamLoader(this, parent, fileName, 0, 0, false, MultiSampleTypes.None, SurfaceFormats.RGBAx8, RenderTargetUsage.PlatformDefault, BufferUsages.Default, false);
		}

		public Texture2D(DisposableI parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat)
		: base(parent)
		{
			init(parent, null, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage.PlatformDefault, BufferUsages.Default, false);
		}

		protected Texture2D(DisposableI parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent)
		{
			init(parent, null, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, BufferUsages.Default, false);
		}

		internal void load(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			init(parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, isRenderTarget);
		}
		
		protected virtual void init(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			try
			{
				if (usage == BufferUsages.Read && !isRenderTarget) Debug.ThrowError("Texture2D", "Only RenderTargets may be readable");

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

			Loaded = true;
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
		public void Copy(Texture2DI texture)
		{
			throw new NotImplementedException();
		}

		public void Update(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void WritePixels(byte[] data)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}