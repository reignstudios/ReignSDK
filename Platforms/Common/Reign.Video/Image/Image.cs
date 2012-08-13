using System.IO;
using System;
using Reign.Core;

namespace Reign.Video
{
	public abstract class Image
	{
		public class Mipmap
		{
			public byte[] Data {get; private set;}
			public Size2 Size {get; private set;}

			public Mipmap(int dataSize, int width, int height)
			{
				Data = new byte[dataSize];
				Size = new Size2(width, height);
			}

			public Mipmap(byte[] data, int width, int height)
			{
				Data = data;
				Size = new Size2(width, height);
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
			
			public static int CalculateMipmapLvls(int width, int height)
			{
				return (int)System.Math.Log(width > height ? width : height, 2) + 1;
			}
		}

		public Mipmap[] Mipmaps;
		public Size2 Size {get; protected set;}

		public static Image Load(string fileName, bool flip, bool generateMipmaps)
		{
			string ext = Streams.GetFileExt(fileName);
			switch (ext.ToLower())
			{
				case (".dds"): return new ImageDDS(fileName, flip);
				case (".pvr"): return new ImagePVR(fileName, flip);
				#if !XNA && NaCl
				case (".bmpc"): return new ImageBMPC(fileName, flip);
				#endif
				#if !XNA && !NaCl
				case (".png"): return new ImagePNG(fileName, flip, generateMipmaps);
				case (".jpg"): return new ImageJPG(fileName, flip, generateMipmaps);
				case (".jpeg"): return new ImageJPG(fileName, flip, generateMipmaps);
				#if !iOS && !ANDROID
				case (".bmp"): return new ImageBMP(fileName, flip, generateMipmaps);
				#endif
				#endif
				default:
					Debug.ThrowError("Image", string.Format("File 'ext' {0} not supported.", ext));
					return null;
			}
		}
	}
}