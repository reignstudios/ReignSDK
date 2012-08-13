using System.IO;

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
		public ImageBMP(string fileName, bool flip, bool generateMipmaps)
		: base(fileName, flip, generateMipmaps)
		{
			
		}

		public ImageBMP(Stream stream, bool flip, bool generateMipmaps)
		: base(stream, flip, generateMipmaps)
		{
			
		}
		#endregion
	}
}