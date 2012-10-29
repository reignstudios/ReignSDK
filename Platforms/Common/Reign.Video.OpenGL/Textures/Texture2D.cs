using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.OpenGL
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

		private Image image;

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
		public uint Texture {get; private set;}
		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}
		public Vector2 TexelOffset {get; private set;}
		internal bool hasMipmaps;
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

		public Texture2D(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat)
		: base(parent)
		{
			new Texture2DStreamLoader(this, parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage.PlatformDefault, BufferUsages.Default, false);
		}

		public Texture2D(DisposableI parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat)
		: base(parent)
		{
			init(parent, null, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage.PlatformDefault, BufferUsages.Default, false);
		}

		protected Texture2D(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent)
		{
			new Texture2DStreamLoader(this, parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, BufferUsages.Default, false);
		}

		protected Texture2D(DisposableI parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent)
		{
			init(parent, null, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, BufferUsages.Default, false);
		}

		internal void load(DisposableI parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			init(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, isRenderTarget);
		}

		protected unsafe virtual void init(DisposableI parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				if (isRenderTarget) generateMipmaps = false;

				uint texturesTEMP = 0;
				GL.GenTextures(1, &texturesTEMP);
				Texture = texturesTEMP;
				if (Texture == 0) Debug.ThrowError("Texture2D", "Failed to Generate Texture");

				GL.BindTexture(GL.TEXTURE_2D, Texture);
				if (!generateMipmaps) GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
				else GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR_MIPMAP_LINEAR);
				GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
				
				hasMipmaps = false;
				if (image != null)
				{
					var imageType = image.GetType();

					#if NaCl
					if (imageType == typeof(ImageBMPC))
					#else
					if (imageType == typeof(ImagePNG) || imageType == typeof(ImageJPG) || imageType == typeof(ImageBMP) || imageType == typeof(ImageBMPC))
					#endif
					{
						var mipmap = image.Mipmaps[0];
						fixed (byte* data = mipmap.Data)
						{
							GL.TexImage2D(GL.TEXTURE_2D, 0, Video.surfaceFormat(surfaceFormat), mipmap.Size.Width, mipmap.Size.Height, 0, GL.RGBA, GL.UNSIGNED_BYTE, data);
							if (generateMipmaps)
							{
								hasMipmaps = true;
								GL.GenerateMipmap(GL.TEXTURE_2D);
							}
						}
					}
					else if (imageType == typeof(ImageDDS) || imageType == typeof(ImagePVR))
					{
						if (image.Mipmaps.Length != Image.Mipmap.CalculateMipmapLvls(image.Size.Width, image.Size.Height))
						{
							Debug.ThrowError("Texture2D", "Compressed Textures require full mipmap chain");
						}
						GL.TexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR_MIPMAP_LINEAR);
						hasMipmaps = true;
					
						bool compressed = false;
						uint format = 0;
						string errorType = null;
						if (imageType == typeof(ImageDDS))
						{
							var imageDDS = (ImageDDS)image;
							compressed = imageDDS.Compressed;
							format = imageDDS.FormatGL;
							errorType = "DDS";
						}
						else if (imageType == typeof(ImagePVR))
						{
							var imagePVR = (ImagePVR)image;
							compressed = imagePVR.Compressed;
							format = imagePVR.FormatGL;
							errorType = "PVR";
						}
					    
						if (compressed)
						{
							for (int i = 0; i < image.Mipmaps.Length; ++i)
							{
								var mipmap = image.Mipmaps[i];
								fixed (byte* data = mipmap.Data)
								{
									// look up:: libtxc_dxtn.so for linux with mesa
									GL.CompressedTexImage2D(GL.TEXTURE_2D, i, format, mipmap.Size.Width, mipmap.Size.Height, 0, mipmap.Data.Length, data);
								}
							}
						}
						else
						{
							Debug.ThrowError("Texture2D", "Loading uncompresed " + errorType + " textures not supported");
						}
					}

					Size = image.Size;
				}
				else
				{
					GL.TexImage2D(GL.TEXTURE_2D, 0, Video.surfaceFormat(surfaceFormat), width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, IntPtr.Zero.ToPointer());
					if (generateMipmaps)
					{
						hasMipmaps = true;
						GL.GenerateMipmap(GL.TEXTURE_2D);
					}
					Size = new Size2(width, height);
				}

				SizeF = Size.ToVector2();

				uint error;
				string errorName;
				if (Video.checkForError(out error, out errorName))
				{
					Debug.ThrowError("Texture2D", string.Format("{0} {1}: Failed to load/create texture", error, errorName));
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}

			Loaded = true;
		}

		public unsafe override void Dispose()
		{
			disposeChilderen();
			if (Texture != 0)
			{
				if (!OS.AutoDisposedGL)
				{
					video.disableActiveTextures(this);
					uint textureTEMP = Texture;
					GL.DeleteTextures(1, &textureTEMP);
				}
				Texture = 0;

				#if DEBUG && !ANDROID
				Video.checkForError();
				#endif
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

		public void ReadPixels(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void ReadPixels(Color4[] colors)
		{
			throw new NotImplementedException();
		}

		public bool ReadPixel(Point position, out Color4 color)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}