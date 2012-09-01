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
		public ImageGDI(string fileName, bool flip)
		{
			new ImageStreamLoader(this, fileName, flip);
		}

		public ImageGDI(Stream stream, bool flip)
		{
			init(stream, flip);
		}

		protected override void init(Stream stream, bool flip)
		{
			SurfaceFormat = SurfaceFormats.RGBAx8;

			using (var bitmap = new Bitmap(stream))
			{
				int width = bitmap.Width;
				int height = bitmap.Height;
				Mipmaps = new Mipmap[1];
				Size = new Size2(bitmap.Width, bitmap.Height);
				
				var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

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
						
				Mipmaps[0] = new Mipmap(data, width, height, 1, 4);
				if (flip) Mipmaps[0].FlipVertical();
				bitmap.UnlockBits(bitmapData);
			}

			Loaded = true;
		}
		#endregion
	}
}