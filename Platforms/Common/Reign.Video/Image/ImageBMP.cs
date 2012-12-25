using System.IO;
using Reign.Core;

namespace Reign.Video
{
	#if iOS
	public class ImageBMP : ImageIOS
	#elif ANDROID
	public class ImageBMP : ImageAndroid
	#elif OSX
	public class ImageBMP : ImageOSX
	#elif METRO
	public class ImageBMP : ImageMetro
	#else
	public class ImageBMP : ImageGDI
	#endif
	{
		#region Construtors
		public ImageBMP(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		: base(fileName, flip, loadedCallback, failedToLoadCallback)
		{
			
		}

		public ImageBMP(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		: base(stream, flip, loadedCallback, failedToLoadCallback)
		{
			
		}

		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			ImageType = ImageTypes.BMP;
			ImageFormat = ImageFormats.BMP;
			SurfaceFormat = SurfaceFormats.RGBAx8;

			base.init(stream, flip, loadedCallback, failedToLoadCallback);
		}
		#endregion
	}
}