using System.IO;
using System;
using Reign.Core;

#if METRO
using System.Threading.Tasks;
#endif

namespace Reign.Video
{
	class ImageStreamLoader : StreamLoaderI
	{
		private Image image;
		private string fileName;
		private bool flip;

		public ImageStreamLoader(Image image, string fileName, bool flip)
		{
			this.image = image;
			this.fileName = fileName;
			this.flip = flip;
		}

		#if METRO
		public override async Task<bool> Load()
		{
			await image.load(fileName, flip);
			return true;
		}
		#else
		public override bool Load()
		{
			image.load(fileName, flip);
			return true;
		}
		#endif
	}

	public abstract class Image
	{
		public class Mipmap
		{
			public byte[] Data {get; private set;}
			public Size2 Size {get; private set;}
			public int Pitch {get; private set;}

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
		}

		public bool Loaded {get; protected set;}
		public Mipmap[] Mipmaps;
		public Size2 Size {get; protected set;}
		public bool Compressed {get; protected set;}
		public SurfaceFormats SurfaceFormat {get; protected set;}

		public Image()
		{
			SurfaceFormat = SurfaceFormats.Unknown;
		}

		#if METRO
		protected abstract Task init(Stream stream, bool flip);

		internal async Task load(string fileName, bool flip)
		{
			using (var stream = await Streams.OpenFile(fileName))
			{
				await init(stream, flip);
			}
		}
		#else
		protected abstract void init(Stream stream, bool flip);

		internal void load(string fileName, bool flip)
		{
			using (var stream = Streams.OpenFile(fileName))
			{
				init(stream, flip);
			}
		}
		#endif

		public static Image Load(string fileName, bool flip)
		{
			string ext = Streams.GetFileExt(fileName);
			switch (ext.ToLower())
			{
				case (".dds"): return new ImageDDS(fileName, flip);
				case (".atc"): return new ImageDDS(fileName, flip);
				case (".pvr"): return new ImagePVR(fileName, flip);
				#if !XNA && NaCl
				case (".bmpc"): return new ImageBMPC(fileName, flip);
				#endif
				#if !XNA && !NaCl
				case (".bmpc"): return new ImageBMPC(fileName, flip);
				case (".png"): return new ImagePNG(fileName, flip);
				case (".jpg"): return new ImageJPG(fileName, flip);
				case (".jpeg"): return new ImageJPG(fileName, flip);
				#if !iOS && !ANDROID
				case (".bmp"): return new ImageBMP(fileName, flip);
				#endif
				#endif
				default:
					Debug.ThrowError("Image", string.Format("File 'ext' {0} not supported.", ext));
					return null;
			}
		}
	}
}