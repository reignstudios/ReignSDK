﻿using Reign.Core;
using Reign_Video_D3D11_Component;
using System;
using System.Runtime.InteropServices;

namespace Reign.Video.D3D11
{
	class Texture2DStreamLoader : StreamLoaderI
	{
		private Image image;

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
			if (image == null)
			{
				image = Image.Load(fileName, false);
				return false;
			}
			else if (!image.Loaded)
			{
				return false;
			}

			texture.load(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, isRenderTarget);
			return true;
		}
	}

	public class Texture2D : Disposable, Texture2DI
	{
		#region Properties
		public bool Loaded {get; private set;}
		protected Video video;
		internal Texture2DCom com;

		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}
		public Vector2 TexelOffset {get; private set;}
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

		public Texture2D(DisposableI parent, int width, int height, SurfaceFormats surfaceFormat)
		: base(parent)
		{
			init(parent, null, width, height, false, MultiSampleTypes.None, surfaceFormat, RenderTargetUsage.PlatformDefault, BufferUsages.Default, false);
		}

		public Texture2D(DisposableI parent, int width, int height, SurfaceFormats surfaceFormat, BufferUsages usage)
		: base(parent)
		{
			init(parent, null, width, height, false, MultiSampleTypes.None, surfaceFormat, RenderTargetUsage.PlatformDefault, usage, false);
		}

		public Texture2D(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, SurfaceFormats surfaceFormat, BufferUsages usage)
		: base(parent)
		{
			new Texture2DStreamLoader(this, parent, fileName, width, height, generateMipmaps, MultiSampleTypes.None, surfaceFormat, RenderTargetUsage.PlatformDefault, usage, false);
		}

		internal void load(DisposableI parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			init(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, isRenderTarget);
		}

		protected virtual void init(DisposableI parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			long[] mipmaps = null;
			int[] mipmapSizes = null, mipmapPitches = null;
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();

				// load image data
				if (image != null)
				{
					mipmaps = new long[image.Mipmaps.Length];
					mipmapSizes = new int[image.Mipmaps.Length];
					mipmapPitches = new int[image.Mipmaps.Length];
					for (int i = 0; i != mipmaps.Length; ++i)
					{
						var imageMipmap = image.Mipmaps[i];
						IntPtr mipmapPtr = Marshal.AllocHGlobal(imageMipmap.Data.Length);
						Marshal.Copy(imageMipmap.Data, 0, mipmapPtr, imageMipmap.Data.Length);
						mipmapSizes[i] = imageMipmap.Data.Length;
						mipmapPitches[i] = imageMipmap.Pitch;
						mipmaps[i] = mipmapPtr.ToInt64();
					}

					Size = image.Size;
					surfaceFormat = image.SurfaceFormat;
				}
				else
				{
					if (width == 0 || height == 0) Debug.ThrowError("Texture2D", "Width or Height cannot be 0");
					Size = new Size2(width, height);
				}
				SizeF = Size.ToVector2();

				// init texture
				REIGN_D3D11_USAGE usageType = REIGN_D3D11_USAGE.DEFAULT;
				REIGN_D3D11_CPU_ACCESS_FLAG cpuAccessFlags = (REIGN_D3D11_CPU_ACCESS_FLAG)0;
				if (usage == BufferUsages.Read)
				{
					usageType = REIGN_D3D11_USAGE.STAGING;
					cpuAccessFlags = REIGN_D3D11_CPU_ACCESS_FLAG.READ;
				}
				if (usage == BufferUsages.Write)
				{
					usageType = REIGN_D3D11_USAGE.DYNAMIC;
					cpuAccessFlags = REIGN_D3D11_CPU_ACCESS_FLAG.WRITE;
				}
				com = new Texture2DCom();
				var error = com.Init(video.com, Size.Width, Size.Height, generateMipmaps, mipmaps, mipmapSizes, mipmapPitches, 0, video.surfaceFormat(surfaceFormat), usageType, cpuAccessFlags, isRenderTarget);

				switch (error)
				{
					case (TextureError.Texture): Debug.ThrowError("Texture2D", "Failed to create Texture2D"); break;
					case (TextureError.ShaderResourceView): Debug.ThrowError("Texture2D", "Failed to create ShaderResourceView"); break;
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
			finally
			{
				if (mipmaps != null)
				{
					for (int i = 0; i != mipmaps.Length; ++i)
					{
						if (mipmaps[i] != 0) Marshal.FreeHGlobal(new IntPtr(mipmaps[i]));
					}
				}
			}

			Loaded = true;
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

		#region Methods
		public void Copy(Texture2DI texture)
		{
			com.Copy(((Texture2D)texture).com);
		}

		public void Update(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void WritePixels(byte[] data)
		{
			throw new NotImplementedException();
		}

		public unsafe void ReadPixels(byte[] data)
		{
			fixed (byte* ptr = data)
			{
				com.ReadPixels((int)ptr, data.Length);
			}
		}

		public unsafe void ReadPixels(Color4[] colors)
		{
			fixed (Color4* ptr = colors)
			{
				com.ReadPixels((int)ptr, colors.Length * 4);
			}
		}

		public bool ReadPixel(Point2 position, out Color4 color)
		{
			if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
			{
				color = new Color4();
				return false;
			}

			color = new Color4(com.ReadPixel(position.X, position.Y, Size.Height));
			return true;
		}
		#endregion
	}
}