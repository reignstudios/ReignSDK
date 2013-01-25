using System.IO;
using Reign.Core;

namespace Reign.Video
{
	public class ImageJPG
	#if iOS
	: ImageIOS
	#elif ANDROID
	: ImageAndroid
	#elif OSX
	: ImageOSX
	#elif WINRT
	: ImageMetro
	#elif WP8
	: ImageWP8
	#elif SILVERLIGHT
	: ImageSilverlight
	#else
	: ImageGDI
	#endif
	{
		#region Construtors
		public ImageJPG(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		: base(fileName, flip, loadedCallback)
		{
			
		}

		public ImageJPG(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		: base(stream, flip, loadedCallback)
		{
			
		}

		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			ImageType = ImageTypes.JPG;
			ImageFormat = ImageFormats.JPG;
			SurfaceFormat = SurfaceFormats.RGBAx8;

			base.init(stream, flip, loadedCallback);
		}
		#endregion
	}
}