using System.IO;
using Reign.Core;

#if WINRT
using System.Threading.Tasks;
#endif

namespace Reign.Video
{
	public class ImagePNG
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
		public ImagePNG(string fileName, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		: base(fileName, flip, loadedCallback)
		{
			
		}

		public ImagePNG(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		: base(stream, flip, loadedCallback)
		{
			
		}

		protected override void init(Stream stream, bool flip, Loader.LoadedCallbackMethod loadedCallback)
		{
			ImageType = ImageTypes.PNG;
			ImageFormat = ImageFormats.PNG;
			SurfaceFormat = SurfaceFormats.RGBAx8;

			base.init(stream, flip, loadedCallback);
		}
		#endregion

		#region Methods
		public static void Save(byte[] inData, int width, int height, Stream outStream, ImageSavedCallbackMethod imageSavedCallback)
		{
			#if WINRT
			ImageMetro.save(inData, width, height, outStream, ImageFormats.PNG, imageSavedCallback);
			#else
			throw new System.NotImplementedException();
			#endif
		}
		#endregion
	}
}