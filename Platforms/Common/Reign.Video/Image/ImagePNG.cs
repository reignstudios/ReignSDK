using System.IO;

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
	#else
	public class ImagePNG : ImageGDI
	#endif
	{
		#region Construtors
		public ImagePNG(string fileName, bool flip, bool generateMipmaps)
		: base(fileName, flip, generateMipmaps)
		{
			
		}

		public ImagePNG(Stream stream, bool flip, bool generateMipmaps)
		: base(stream, flip, generateMipmaps)
		{
			
		}
		#endregion
	}
}