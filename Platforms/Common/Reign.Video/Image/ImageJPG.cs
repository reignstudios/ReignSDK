using System.IO;

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
	#else
	public class ImageJPG : ImageGDI
	#endif
	{
		#region Construtors
		public ImageJPG(string fileName, bool flip)
		: base(fileName, flip)
		{
			
		}

		public ImageJPG(Stream stream, bool flip)
		: base(stream, flip)
		{
			
		}
		#endregion
	}
}