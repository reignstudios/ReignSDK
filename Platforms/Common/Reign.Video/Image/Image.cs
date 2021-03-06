﻿using System.IO;
using System;
using Reign.Core;

#if WINRT
using System.Threading.Tasks;
#endif

namespace Reign.Video
{
	public enum ImageTypes
	{
		DDS,
		PVR,
		PNG,
		JPG,
		BMP,
		BMPC
	}

	public enum ImageFormats
	{
		DXT1,
		DXT3,
		DXT5,
		ATC_RGB,
		ATC_RGBA_Explicit,
		ATC_RGBA_Interpolated,
		PVR_RGB_2,
		PVR_RGBA_2,
		PVR_RGB_4,
		PVR_RGBA_4,
		PNG,
		JPG,
		BMP,
		BMPC
	}

	public abstract class Image : ILoadable
	{
		public class Mipmap
		{
			#region Properties
			public byte[] Data {get; private set;}
			public Size2 Size {get; private set;}
			public int Pitch {get; private set;}
			#endregion

			#region Constructors
			public Mipmap(int dataSize, int width, int height, int blockDev, int channels)
			{
				Data = new byte[dataSize];
				Size = new Size2(width, height);
				Pitch = calculatePitch(width, blockDev, channels);
			}

			public Mipmap(byte[] data, int width, int height, int blockDev, int channels)
			{
				Data = data;
				Size = new Size2(width, height);
				Pitch = calculatePitch(width, blockDev, channels);
			}
			#endregion

			#region Methods
			private int calculatePitch(int width, int blockDev, int channels)
			{
				return (width / blockDev) * channels;
			}
			
			public void FlipVertical()
			{
				int loopH = Size.Height / 2, loopW = Size.Width * 4;
				int r2 = Size.Height - 1;
				for (int r = 0; r != loopH; ++r)
				{
					for (int c = 0; c != loopW; ++c)
					{
						int index = c + (loopW * r);
						int index2 = c + (loopW * r2);
						byte temp = Data[index];
						Data[index] = Data[index2];
						Data[index2] = temp;
					}
					
					--r2;
				}
			}

			public byte[] SwapRBColorChannels()
			{
				var data = new byte[Data.Length];
				for (int i = 0; i != data.Length; i += 4)
				{
					data[i] = Data[i+2];
					data[i+1] = Data[i+1];
					data[i+2] = Data[i];
					data[i+3] = Data[i+3];
				}

				return data;
			}
			
			public static int CalculateMipmapLvls(int width, int height)
			{
				return (int)System.Math.Log(width > height ? width : height, 2) + 1;
			}
			#endregion
		}

		#region Properties
		public bool Loaded {get; protected set;}
		public bool FailedToLoad {get; protected set;}

		public Mipmap[] Mipmaps;
		public Size2 Size {get; protected set;}
		public bool Compressed {get; protected set;}
		public SurfaceFormats SurfaceFormat {get; protected set;}
		public ImageTypes ImageType {get; protected set;}
		public ImageFormats ImageFormat {get; protected set;}
		#endregion

		#region Constructors
		public Image()
		{
			SurfaceFormat = SurfaceFormats.Unknown;
		}

		public static Image New(string filename, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			string ext = Streams.GetFileExt(filename);
			switch (ext.ToLower())
			{
				case ".dds": return new ImageDDS(filename, flip, loadedCallback);
				case ".atc": return new ImageDDS(filename, flip, loadedCallback);
				case ".pvr": return new ImagePVR(filename, flip, loadedCallback);
				#if !XNA && NaCl
				case ".bmpc": return new ImageBMPC(filename, flip, loadedCallback);
				#endif
				#if (!XNA && !NaCl && !VITA) || SILVERLIGHT
				#if !WP8
				case ".bmpc": return new ImageBMPC(filename, flip, loadedCallback);
				#endif
				case ".png": return new ImagePNG(filename, flip, loadedCallback);
				case ".jpg": return new ImageJPG(filename, flip, loadedCallback);
				case ".jpeg": return new ImageJPG(filename, flip, loadedCallback);
				#if !iOS && !ANDROID
				case ".bmp": return new ImageBMP(filename, flip, loadedCallback);
				#endif
				#endif
				default:
					Debug.ThrowError("Image", string.Format("File 'ext' {0} not supported.", ext));
					return null;
			}
		}

		protected abstract void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback);

		public bool UpdateLoad()
		{
			return Loaded;
		}
		#endregion

		#region Methods
		public int CalculatePixelByteSize()
		{
			return CalculatePixelByteSize(SurfaceFormat, Size.Width, Size.Height);
		}

		public static int CalculatePixelByteSize(SurfaceFormats surfaceFormat, int width, int height)
		{
			switch (surfaceFormat)
			{
				case SurfaceFormats.RGBx565: return width * height * 2;
				case SurfaceFormats.RGBAx4: return width * height * 2;
				case SurfaceFormats.RGBx5_Ax1: return width * height * 2;
				case SurfaceFormats.RGBAx8: return width * height * 4;
				case SurfaceFormats.RGBx10_Ax2: return width * height * 4;
				case SurfaceFormats.RGBAx16f: return (width * height * 4) * 2;
				case SurfaceFormats.RGBAx32f: return (width * height * 4) * 4;
				case SurfaceFormats.DXT1: return (width * height) / 2;
				case SurfaceFormats.DXT3: return width * height;
				case SurfaceFormats.DXT5: return width * height;
				case SurfaceFormats.ATC_RGB: return (width * height) / 2;
				case SurfaceFormats.ATC_RGBA_Explicit: return width * height;
				case SurfaceFormats.ATC_RGBA_Interpolated: return width * height;
				case SurfaceFormats.PVR_RGB_2: return (width * height) / 4;
				case SurfaceFormats.PVR_RGB_4: return (width * height) / 2;
				case SurfaceFormats.PVR_RGBA_2: return (width * height) / 4;
				case SurfaceFormats.PVR_RGBA_4: return (width * height) / 2;
				default: Debug.ThrowError("Image", string.Format("Unsuported surface format: {0}", surfaceFormat)); break;
			}

			return -1;
		}

		public delegate void ImageSavedCallbackMethod(bool succeeded);
		public static void Save(byte[] inData, int width, int height, Stream outStream, ImageFormats imageFormat, ImageSavedCallbackMethod imageSavedCallback)
		{
			switch (imageFormat)
			{
				#if !NaCl && !XNA && !VITA && !WP8
				case ImageFormats.BMPC: ImageBMPC.Save(inData, width, height, outStream, imageSavedCallback); break;
				#endif
				#if WINRT
				case ImageFormats.PNG: ImagePNG.Save(inData, width, height, outStream, imageSavedCallback); break;
				#endif
				default: Debug.ThrowError("Image", string.Format("Unsuported format: ", imageFormat)); break;
			}
		}
		#endregion
	}
}