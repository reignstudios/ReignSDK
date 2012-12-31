using System.IO;
using Reign.Core;

#if METRO
using System.Threading.Tasks;
#endif

namespace Reign.Video
{
	#if iOS
	public class ImagePNG : ImageIOS
	#elif ANDROID
	public class ImagePNG : ImageAndroid
	#elif OSX
	public class ImagePNG : ImageOSX
	#elif METRO
	public class ImagePNG : ImageMetro
	#elif SILVERLIGHT
	public class ImagePNG : ImageSilverlight
	#else
	public class ImagePNG : ImageGDI
	#endif
	{
		#region Construtors
		public ImagePNG(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		: base(fileName, flip, loadedCallback, failedToLoadCallback)
		{
			
		}

		public ImagePNG(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		: base(stream, flip, loadedCallback, failedToLoadCallback)
		{
			
		}

		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			ImageType = ImageTypes.PNG;
			ImageFormat = ImageFormats.PNG;
			SurfaceFormat = SurfaceFormats.RGBAx8;

			base.init(stream, flip, loadedCallback, failedToLoadCallback);
		}
		#endregion

		#region Methods
		#if METRO
		public static async Task Save(byte[] data, int width, int height, Stream outStream)
		{
			await ImageMetro.save(data, width, height, outStream, ImageFormats.PNG);
		}
		#endif
		#endregion
	}
}