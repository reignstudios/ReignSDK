using System;
using System.IO;
using Reign.Core;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace Reign.Video
{
	public class ImageIOS : Image
	{
		public ImageIOS(string fileName, bool flip, bool generateMipmaps)
		{
			using (var stream = Streams.OpenFile(fileName))
			{
				init(stream, flip, generateMipmaps);
			}
		}

		public ImageIOS(Stream stream, bool flip, bool generateMipmaps)
		{
			init(stream, flip, generateMipmaps);
		}

		private void init(Stream stream, bool flip, bool generateMipmaps)
		{
			using (var imageData = NSData.FromStream(stream))
			using (var image = UIImage.LoadFromData(imageData))
			{
				int width = (int)image.Size.Width;
				int height = (int)image.Size.Height;
				int mipLvls = generateMipmaps ? Image.Mipmap.CalculateMipmapLvls(width, height) : 1;
				Mipmaps = new Mipmap[mipLvls];
				Size = new Size2(width, height);
			
				for (int i = 0; i != mipLvls; ++i)
				{
					var data = new byte[width * height * 4];
					using (CGContext imageContext = new CGBitmapContext(data, width, height, 8, width*4, CGColorSpace.CreateDeviceRGB(), CGImageAlphaInfo.PremultipliedLast))
					{
						imageContext.DrawImage(new RectangleF(0, 0, width, height), image.CGImage);
					
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

