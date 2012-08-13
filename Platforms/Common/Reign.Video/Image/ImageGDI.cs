using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.Runtime.InteropServices;
using Reign.Core;

namespace Reign.Video
{
	public abstract class ImageGDI : Image
	{
		#region Constructors
		public ImageGDI(string fileName, bool flip, bool generateMipmaps)
		{
			using (var stream = Streams.OpenFile(fileName))
			{
				init(stream, flip, generateMipmaps);
			}
		}

		public ImageGDI(Stream stream, bool flip, bool generateMipmaps)
		{
			init(stream, flip, generateMipmaps);
		}

		private void init(Stream stream, bool flip, bool generateMipmaps)
		{
			using (var bitmap = new Bitmap(stream))
			{
				int width = bitmap.Width;
				int height = bitmap.Height;
				int mipLvls = generateMipmaps ? Image.Mipmap.CalculateMipmapLvls(width, height) : 1;
				Mipmaps = new Mipmap[mipLvls];
				Size = new Size2(bitmap.Width, bitmap.Height);
				
				for (int i = 0; i != mipLvls; ++i)
				{
					using (var scaledBitmap = ResizeBitmap(bitmap, width, height))
					{
						var bitmapData = scaledBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

						// Copy unmanaged data to managed array
						int size = System.Math.Abs(bitmapData.Stride) * height;
						byte[] data = new byte[size];
						Marshal.Copy(bitmapData.Scan0, data, 0, size);

						// Flip RB Color bits
						for (int i2 = 0; i2 != data.Length; i2 += 4)
						{
							byte c = data[i2];
							data[i2] = data[i2+2];
							data[i2+2] = c;
						}
						
						Mipmaps[i] = new Mipmap(data, width, height);
						if (flip) Mipmaps[i].FlipVertical();
						scaledBitmap.UnlockBits(bitmapData);

						width /= 2;
						height /= 2;
					}
				}
			}
		}
		#endregion

		#region Methods
		public Bitmap ResizeBitmap(Bitmap bitmap, int width, int height)
		{
			var result = new Bitmap(width, height);
			using (var graphics = Graphics.FromImage(result))
			{
				graphics.DrawImage(bitmap, 0, 0, width, height);
			}

			return result;
		}
		#endregion
	}
}