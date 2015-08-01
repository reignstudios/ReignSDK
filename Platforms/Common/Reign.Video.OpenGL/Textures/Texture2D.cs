using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.OpenGL
{
	public class Texture2D : DisposableResource, ITexture2D
	{
		#region Properties
		public bool Loaded {get; protected set;}
		public bool FailedToLoad {get; protected set;}

		public Size2 Size {get; protected set;}
		public Vector2 SizeF {get; protected set;}
		public Vector2 TexelOffset {get; protected set;}
		public int PixelByteSize {get; protected set;}

		protected Video video;
		public uint Texture {get; private set;}
		internal bool hasMipmaps;
		#endregion

		#region Constructors
		public Texture2D(IDisposableResource parent, string filename, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			Image.New(filename, false,
			delegate (object sender, bool succeeded)
			{
				if (succeeded)
				{
					var image = (Image)sender;
					init(parent, image, image.Size.Width, image.Size.Height, generateMipmaps, MultiSampleTypes.None, image.SurfaceFormat, RenderTargetUsage.PlatformDefault, usage, false, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}
		
		public Texture2D(IDisposableResource parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			init(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, false, loadedCallback);
		}

		protected unsafe virtual bool init(IDisposableResource parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				if (usage == BufferUsages.Read && !isRenderTarget) Debug.ThrowError("Texture2D", "Only RenderTargets may be readable");

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
						uint pixelOrder, byteSizes;
						int format = Video.surfaceFormat(surfaceFormat, out pixelOrder, out byteSizes);
						var mipmap = image.Mipmaps[0];
						fixed (byte* data = mipmap.Data)
						{
							GL.TexImage2D(GL.TEXTURE_2D, 0, format, mipmap.Size.Width, mipmap.Size.Height, 0, pixelOrder, byteSizes, data);
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
					PixelByteSize = image.CalculatePixelByteSize();
				}
				else
				{
					//GL.TexImage2D(GL.TEXTURE_2D, 0, Video.surfaceFormat(surfaceFormat), width, height, 0, GL.RGBA, GL.UNSIGNED_BYTE, IntPtr.Zero.ToPointer());
					uint pixelOrder, byteSizes;
					int format = Video.surfaceFormat(surfaceFormat, out pixelOrder, out byteSizes);
					GL.TexImage2D(GL.TEXTURE_2D, 0, format, width, height, 0, pixelOrder, byteSizes, IntPtr.Zero.ToPointer());
					if (generateMipmaps)
					{
						hasMipmaps = true;
						GL.GenerateMipmap(GL.TEXTURE_2D);
					}
					Size = new Size2(width, height);
					PixelByteSize = Image.CalculatePixelByteSize((surfaceFormat == SurfaceFormats.Defualt ? Video.DefaultSurfaceFormat() : surfaceFormat), width, height);
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
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(this, false);
				return false;
			}

			if (!isRenderTarget)
			{
				Loaded = true;
				if (loadedCallback != null) loadedCallback(this, true);
			}
			return true;
		}

		public bool UpdateLoad()
		{
			return Loaded;
		}

		public unsafe override void Dispose()
		{
			disposeChilderen();
			if (Texture != 0)
			{
				if (!IPlatform.Singleton.AutoDisposedGL)
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
		public void Copy(ITexture2D texture)
		{
			var textureTEMP = (Texture2D)texture;
			uint pixelOrder, byteSizes;
			int format = Video.surfaceFormat(Video.DefaultSurfaceFormat(), out pixelOrder, out byteSizes);
			GL.CopyTexImage2D(textureTEMP.Texture, 0, (uint)format, 0, 0, Size.Width, Size.Height, 0);
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