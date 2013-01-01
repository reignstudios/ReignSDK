using System.IO;
using Reign.Core;

namespace Reign.Video
{
	#if iOS
	public class ImageJPG : ImageIOS
	#elif ANDROID
	public class ImageJPG : ImageAndroid
	#elif OSX
	public class ImageJPG : ImageOSX
	#elif METRO
	public class ImageJPG : ImageMetro
	#elif SILVERLIGHT
	public class ImageJPG : ImageSilverlight
	#else
	public class ImageJPG : ImageGDI
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