using System.IO;
using Reign.Core;

namespace Reign.Video
{
	public class ImageBMP
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
		public ImageBMP(string filename, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		: base(filename, flip, loadedCallback)
		{
			
		}

		public ImageBMP(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		: base(stream, flip, loadedCallback)
		{
			
		}

		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			ImageType = ImageTypes.BMP;
			ImageFormat = ImageFormats.BMP;
			SurfaceFormat = SurfaceFormats.RGBAx8;

			base.init(stream, flip, loadedCallback);
		}
		#endregion
	}
}