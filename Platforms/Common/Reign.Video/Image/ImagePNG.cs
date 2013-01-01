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
			#if METRO
			ImageMetro.save(inData, width, height, outStream, ImageFormats.PNG, imageSavedCallback);
			#else
			throw new System.NotImplementedException();
			#endif
		}
		#endregion
	}
}