using System;
using System.IO;
using Reign.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using Windows.System.Threading;
using System.Threading;
using Windows.Foundation;

namespace Reign.Video
{
	public class ImageMetro : Image
	{
		#region Properties
		private Stream loadingStream;
		private bool loadingFlip, doneLoading;
		#endregion

		#region Constructors
		public ImageMetro(string fileName, bool flip)
		{
			new ImageStreamLoader(this, fileName, flip);
		}

		public ImageMetro(Stream stream, bool flip)
		{
			init(stream, flip);
		}

		protected override void init(Stream stream, bool flip)
		{
			loadingStream = stream;
			loadingFlip = flip;
			var task = ThreadPool.RunAsync(new WorkItemHandler(loadStream), WorkItemPriority.Normal);
			while (!doneLoading) new ManualResetEvent(false).WaitOne(1);
			loadingStream = null;
		}

		private async void loadStream(Windows.Foundation.IAsyncAction op)
		{
			await initThread(loadingStream, loadingFlip);
			doneLoading = true;
		}

		private async Task initThread(Stream stream, bool flip)
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
		protected static async Task save(byte[] data, int width, int height, Stream outStream, ImageFormats imageFormat)
		{
			using (var memoryStream = new InMemoryRandomAccessStream())
			{
				Guid encodeID = new Guid();
				switch (imageFormat)
				{
					case (ImageFormats.PNG): encodeID = BitmapEncoder.PngEncoderId; break;
					case (ImageFormats.JPG): encodeID = BitmapEncoder.PngEncoderId; break;
					case (ImageFormats.BMP): encodeID = BitmapEncoder.PngEncoderId; break;
					default: Debug.ThrowError("ImageMetro", "Unsuported image format: " + imageFormat.ToString()); break;
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
