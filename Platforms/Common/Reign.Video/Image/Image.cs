using System.IO;
using System;
using Reign.Core;

#if METRO
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

	public abstract class Image : LoadableI
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

		public static Image New(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			string ext = Streams.GetFileExt(fileName);
			switch (ext.ToLower())
			{
				case (".dds"): return new ImageDDS(fileName, flip, loadedCallback, failedToLoadCallback);
				case (".atc"): return new ImageDDS(fileName, flip, loadedCallback, failedToLoadCallback);
				case (".pvr"): return new ImagePVR(fileName, flip, loadedCallback, failedToLoadCallback);
				#if !XNA && NaCl
				case (".bmpc"): return new ImageBMPC(fileName, flip, loadedCallback, failedToLoadCallback);
				#endif
				#if !XNA && !NaCl
				case (".bmpc"): return new ImageBMPC(fileName, flip, loadedCallback, failedToLoadCallback);
				case (".png"): return new ImagePNG(fileName, flip, loadedCallback, failedToLoadCallback);
				case (".jpg"): return new ImageJPG(fileName, flip, loadedCallback, failedToLoadCallback);
				case (".jpeg"): return new ImageJPG(fileName, flip, loadedCallback, failedToLoadCallback);
				#if !iOS && !ANDROID
				case (".bmp"): return new ImageBMP(fileName, flip, loadedCallback, failedToLoadCallback);
				#endif
				#endif
				default:
					Debug.ThrowError("Image", string.Format("File 'ext' {0} not supported.", ext));
					return null;
			}
		}

		protected abstract void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback);

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
				case (SurfaceFormats.RGBAx8): return width * height * 4;
				case (SurfaceFormats.RGBx10_Ax2): return width * height * 4;
				case (SurfaceFormats.RGBAx16f): return (width * height * 4) * 2;
				case (SurfaceFormats.RGBAx32f): return (width * height * 4) * 4;
				case (SurfaceFormats.DXT1): return (width * height) / 2;
				case (SurfaceFormats.DXT3): return width * height;
				case (SurfaceFormats.DXT5): return width * height;
				case (SurfaceFormats.ATC_RGB): return (width * height) / 2;
				case (SurfaceFormats.ATC_RGBA_Explicit): return width * height;
				case (SurfaceFormats.ATC_RGBA_Interpolated): return width * height;
				case (SurfaceFormats.PVR_RGB_2): return (width * height) / 4;
				case (SurfaceFormats.PVR_RGB_4): return (width * height) / 2;
				case (SurfaceFormats.PVR_RGBA_2): return (width * height) / 4;
				case (SurfaceFormats.PVR_RGBA_4): return (width * height) / 2;
				default: Debug.ThrowError("Image", string.Format("Unsuported surface format: ", surfaceFormat)); break;
			}

			return -1;
		}

		#if METRO
		public static async Task Save(Stream stream, byte[] data, int width, int height, ImageFormats imageFormat)
		{
			switch (imageFormat)
			{
				case (ImageFormats.BMPC): ImageBMPC.Save(data, width, height, stream); break;
				case (ImageFormats.PNG): await ImagePNG.Save(data, width, height, stream); break;
				default: Debug.ThrowError("Image", string.Format("Unsuported format: ", imageFormat)); break;
			}
		}
		#else
		#if !NaCl
		public static void Save(Stream stream, byte[] data, int width, int height, ImageFormats imageFormat)
		{
			switch (imageFormat)
			{
				#if !XNA && NaCl
				case (ImageFormats.BMPC): ImageBMPC.Save(data, width, height, stream); break;
				#endif
				default: Debug.ThrowError("Image", string.Format("Unsuported format: ", imageFormat)); break;
			}
		}
		#endif
		#endif
		#endregion
	}
}