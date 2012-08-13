using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.OpenGL
{
	public class Texture2D : Disposable, Texture2DI
	{
		#region Properties
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
			init(parent, fileName, 0, 0, false, MultiSampleTypes.None, SurfaceFormats.RGBAx8, RenderTargetUsage.PlatformDefault, false);
		}

		public Texture2D(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat)
		: base(parent)
		{
			init(parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage.PlatformDefault, false);
		}

		public Texture2D(DisposableI parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat)
		: base(parent)
		{
			init(parent, null, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage.PlatformDefault, false);
		}

		protected Texture2D(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent)
		{
			init(parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, false);
		}

		protected Texture2D(DisposableI parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent)
		{
			init(parent, null, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, false);
		}

		protected unsafe virtual void init(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool isRenderTarget)
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
				if (fileName != null)
				{
					var image = Image.Load(fileName, true, false);
					var imageType = image.GetType();

					if (imageType == typeof(ImagePNG) || imageType == typeof(ImageJPG) || imageType == typeof(ImageBMP) || imageType == typeof(ImageBMPC))
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
							compressed = imageDDS.IsCompressed;
							format = imageDDS.FormatGL;
							errorType = "DDS";
						}
						else if (imageType == typeof(ImagePVR))
						{
							var imagePVR = (ImagePVR)image;
							compressed = imagePVR.IsCompressed;
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
					fileName = "RenderTarget";
					Debug.ThrowError("Texture2D", string.Format("{0} {1}: Failed to load/create texture: {2}", error, errorName, fileName));
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
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