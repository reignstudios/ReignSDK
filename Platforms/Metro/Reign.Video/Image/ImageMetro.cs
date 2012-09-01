using System;
using System.IO;
using Reign.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
//using Windows.Graphics.Imaging;

namespace Reign.Video
{
	public class ImageMetro : Image
	{
		public ImageMetro(string fileName, bool flip)
		{
			new ImageStreamLoader(this, fileName, flip);
		}

		public ImageMetro(Stream stream, bool flip)
		{
			init(stream, flip);
		}

		protected override async void init(Stream stream, bool flip)
		{
			var memoryStream = new InMemoryRandomAccessStream();
			await stream.CopyToAsync(memoryStream.AsStreamForWrite());
			var image = new BitmapImage();
			image.SetSource(memoryStream);

			Loaded = true;
		}
	}
}
