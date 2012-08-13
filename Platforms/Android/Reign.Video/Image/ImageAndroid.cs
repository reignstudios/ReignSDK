using System;
using System.IO;
using Reign.Core;
using Android.Graphics;

namespace Reign.Video
{
	public class ImageAndroid : Image
	{
		public ImageAndroid(string fileName, bool flip, bool generateMipmaps)
		{
			using (var stream = Streams.OpenFile(fileName))
			{
				init(stream, flip, generateMipmaps);
			}
		}

		public ImageAndroid(Stream stream, bool flip, bool generateMipmaps)
		{
			init(stream, flip, generateMipmaps);
		}

		private void init(Stream stream, bool flip, bool generateMipmaps)
		{
			using (var bitmap = Android.Graphics.BitmapFactory.DecodeStream(stream))
			{
				int width = bitmap.Width;
				int height = bitmap.Height;
				int mipLvls = generateMipmaps ? Image.Mipmap.CalculateMipmapLvls(width, height) : 1;
				Mipmaps = new Mipmap[mipLvls];
				Size = new Size2(bitmap.Width, bitmap.Height);
			
				for (int i = 0; i != mipLvls; ++i)
				{
					using (var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, true))
					{
						var pixels = new int[width * height];
						scaledBitmap.GetPixels(pixels, 0, width, 0, 0, width, height);
						
						// Convert to bytes
						var data = new byte[pixels.Length * 4];
						int i3 = 0;
						for (int i2 = 0; i2 != pixels.Length; ++i2)
						{
							data[i3] = (byte)Color.GetRedComponent(pixels[i2]);
							data[i3+1] = (byte)Color.GetGreenComponent(pixels[i2]);
							data[i3+2] = (byte)Color.GetBlueComponent(pixels[i2]);
							data[i3+3] = (byte)Color.GetAlphaComponent(pixels[i2]);
							i3 += 4;
						}
						
						Mipmaps[i] = new Mipmap(data, width, height);
						if (flip) Mipmaps[i].FlipVertical();
						
						width /= 2;
						height /= 2;
					}
				}
			}
		}
	}
}

