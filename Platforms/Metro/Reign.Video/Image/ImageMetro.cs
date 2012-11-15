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
		#region Constructors
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

			using (var memoryStream = new InMemoryRandomAccessStream())
			{
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
			}

			Loaded = true;
		}
		#endregion

		#region Methods
		#if METRO
		protected static async Task save(byte[] data, int width, int height, Stream outStream, ImageTypes imageType)
		#else
		protected static void save(byte[] data, int width, int height, Stream outStream, ImageTypes imageType)
		#endif
		{
			using (var memoryStream = new InMemoryRandomAccessStream())
			{
				Guid encodeID = new Guid();
				switch (imageType)
				{
					case (ImageTypes.png): encodeID = BitmapEncoder.PngEncoderId; break;
					case (ImageTypes.jpg): encodeID = BitmapEncoder.PngEncoderId; break;
					case (ImageTypes.bmp): encodeID = BitmapEncoder.PngEncoderId; break;
					default: Debug.ThrowError("ImageMetro", "Unsuported image type: " + imageType.ToString()); break;
				}

				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encodeID, memoryStream);
				encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, (uint)width, (uint)height, 96, 96, data);
				await encoder.FlushAsync();
				
				var stream = memoryStream.AsStream();
				stream.Position = 0;
				await stream.CopyToAsync(outStream);
			}
		}
		#endregion
	}
}
