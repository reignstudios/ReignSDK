using System;
using System.IO;
using Reign.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;

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
			init(stream, flip).Wait();
		}

		protected override async Task init(Stream stream, bool flip)
		{
			SurfaceFormat = SurfaceFormats.RGBAx8;

			var memoryStream = new InMemoryRandomAccessStream();
			await RandomAccessStream.CopyAsync(stream.AsInputStream(), memoryStream);

			var decoder = await BitmapDecoder.CreateAsync(memoryStream);
			var frame = await decoder.GetFrameAsync(0);

			var transform = new BitmapTransform();
			transform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;
			transform.Rotation = BitmapRotation.None;
			var dataProvider = await decoder.GetPixelDataAsync(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.ColorManageToSRgb);
			var data = dataProvider.DetachPixelData();
			
			int width = (int)decoder.PixelWidth;
			int height = (int)decoder.PixelHeight;
			Mipmaps = new Mipmap[1];
			Size = new Size2(width, height);

			Mipmaps[0] = new Mipmap(data, width, height, 1, 4);
			if (flip) Mipmaps[0].FlipVertical();

			Loaded = true;
		}
	}
}
