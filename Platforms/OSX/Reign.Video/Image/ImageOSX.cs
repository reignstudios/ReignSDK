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
		public ImageOSX(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
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
		
		public ImageOSX(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			init(stream, flip, loadedCallback, failedToLoadCallback);
		}

		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			try
			{
				using (var image = NSImage.FromStream(stream))
				{
					var rep = image.Representations()[0];
					int width = rep.PixelsWide;
					int height = rep.PixelsHigh;
					Mipmaps = new Mipmap[1];
					Size = new Size2(width, height);
				
					var data = new byte[width * height * 4];
					using (CGContext imageContext = new CGBitmapContext(data, width, height, 8, width*4, CGColorSpace.CreateDeviceRGB(), CGImageAlphaInfo.PremultipliedLast))
					using (var cgImage = image.AsCGImage(RectangleF.Empty, null, null))
					{
						imageContext.DrawImage(new RectangleF(0, 0, width, height), cgImage);
					
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

