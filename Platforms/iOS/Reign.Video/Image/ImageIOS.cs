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
		public ImageIOS(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			new StreamLoader(fileName,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init(((StreamLoader)sender).LoadedStream, flip, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}
		
		public ImageIOS(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			init(stream, flip, loadedCallback);
		}
		
		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				using (var imageData = NSData.FromStream(stream))
				using (var image = UIImage.LoadFromData(imageData))
				{
					int width = (int)image.Size.Width;
					int height = (int)image.Size.Height;
					Mipmaps = new Mipmap[1];
					Size = new Size2(width, height);
				
					var data = new byte[width * height * 4];
					using (CGContext imageContext = new CGBitmapContext(data, width, height, 8, width*4, CGColorSpace.CreateDeviceRGB(), CGImageAlphaInfo.PremultipliedLast))
					{
						imageContext.DrawImage(new RectangleF(0, 0, width, height), image.CGImage);
					
						Mipmaps[0] = new Mipmap(data, width, height, 1, 4);
						if (flip) Mipmaps[0].FlipVertical();
					}
				}
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				if (loadedCallback != null) loadedCallback(this, false);
				return;
			}
			
			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
		}
	}
}

