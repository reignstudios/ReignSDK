using System;
using System.IO;
using Reign.Core;
using Android.Graphics;

namespace Reign.Video
{
	public class ImageAndroid : Image
	{
		public ImageAndroid(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback)
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
		
		public ImageAndroid(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			init(stream, flip, loadedCallback);
		}
		
		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				using (var bitmap = Android.Graphics.BitmapFactory.DecodeStream(stream))
				{
					int width = bitmap.Width;
					int height = bitmap.Height;
					Mipmaps = new Mipmap[1];
					Size = new Size2(bitmap.Width, bitmap.Height);
				
					var pixels = new int[width * height];
					bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);
					
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
					
					Mipmaps[0] = new Mipmap(data, width, height, 1, 4);
					if (flip) Mipmaps[0].FlipVertical();
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

