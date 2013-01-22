using System;
using System.IO;
using Reign.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.System.Threading;
using System.Threading;
using Windows.Foundation;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Reign.Video
{
	public abstract class ImageWP8 : Image
	{
		#region Properties
		private BitmapImage image;
		private bool flip;
		private Loader.LoadedCallbackMethod loadedCallback;
		#endregion

		#region Constructors
		public ImageWP8(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			this.flip = flip;
			this.loadedCallback = loadedCallback;

			image = new BitmapImage();
			image.ImageOpened += image_ImageOpened;
			image.ImageFailed += image_ImageFailed;
			image.UriSource = new Uri(fileName, UriKind.Relative);
		}

		public ImageWP8(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			this.flip = flip;
			this.loadedCallback = loadedCallback;

			image = new BitmapImage();
			image.ImageOpened += image_ImageOpened;
			image.ImageFailed += image_ImageFailed;
			image.SetSource(stream);
		}

		private void image_ImageFailed(object sender, ExceptionRoutedEventArgs args)
		{
			FailedToLoad = true;
			if (loadedCallback != null) loadedCallback(this, false);
			loadedCallback = null;
			image = null;
		}

		private void image_ImageOpened(object sender, RoutedEventArgs args)
		{
			init(null, flip, loadedCallback);
		}

		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				var bitmap = new WriteableBitmap(image);
				var pixels = bitmap.Pixels;
				var data = new byte[pixels.Length * 4];
				int i2 = 0;
				for (int i = 0; i != pixels.Length; ++i)
				{
					Color4 color = new Color4(pixels[i]);
					data[i2] = color.R;
					data[i2+1] = color.G;
					data[i2+2] = color.B;
					data[i2+3] = color.A;
					i2 += 4;
				}

				int width = (int)bitmap.PixelWidth;
				int height = (int)bitmap.PixelHeight;
				Mipmaps = new Mipmap[1];
				Size = new Size2(width, height);

				Mipmaps[0] = new Mipmap(data, width, height, 1, 4);
				if (flip) Mipmaps[0].FlipVertical();
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				if (loadedCallback != null) loadedCallback(this, false);
				loadedCallback = null;
				image = null;
				return;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
			loadedCallback = null;
			image = null;
		}
		#endregion
	}
}
