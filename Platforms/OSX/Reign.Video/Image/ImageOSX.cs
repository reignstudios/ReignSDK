using System;
using System.IO;
using Reign.Core;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;

namespace Reign.Video
{
	public class ImageOSX : Image
	{
		public ImageOSX(string fileName, bool flip, bool generateMipmaps)
		{
			using (var stream = Streams.OpenFile(fileName))
			{
				init(stream, flip, generateMipmaps);
			}
		}

		public ImageOSX(Stream stream, bool flip, bool generateMipmaps)
		{
			init(stream, flip, generateMipmaps);
		}

		private void init(Stream stream, bool flip, bool generateMipmaps)
		{
			using (var image = NSImage.FromStream(stream))
			{
				var rep = image.Representations()[0];
				int width = rep.PixelsWide;
				int height = rep.PixelsHigh;
				int mipLvls = generateMipmaps ? Image.Mipmap.CalculateMipmapLvls(width, height) : 1;
				Mipmaps = new Mipmap[mipLvls];
				Size = new Size2(width, height);
			
				for (int i = 0; i != mipLvls; ++i)
				{
					var data = new byte[width * height * 4];
					using (CGContext imageContext = new CGBitmapContext(data, width, height, 8, width*4, CGColorSpace.CreateDeviceRGB(), CGImageAlphaInfo.PremultipliedLast))
					using (var cgImage = image.AsCGImage(RectangleF.Empty, null, null))
					{
						imageContext.DrawImage(new RectangleF(0, 0, width, height), cgImage);
					
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

