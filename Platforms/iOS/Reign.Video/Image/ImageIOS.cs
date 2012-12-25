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
		public ImageIOS(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			new StreamLoader(fileName,
			delegate(object sender)
			{
				init(((StreamLoader)sender).LoadedStream, flip, loadedCallback, failedToLoadCallback);
			},
			delegate
			{
				FailedToLoad = true;
				if (failedToLoadCallback != null) failedToLoadCallback();
			});
		}
		
		public ImageIOS(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			init(stream, flip, loadedCallback, failedToLoadCallback);
		}
		
		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
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
				if (failedToLoadCallback != null) failedToLoadCallback();
				return;
			}
			
			Loaded = true;
			if (loadedCallback != null) loadedCallback(this);
		}
	}
}

