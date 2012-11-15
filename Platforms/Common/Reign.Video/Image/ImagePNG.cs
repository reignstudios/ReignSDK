using System.IO;

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
	#else
	public class ImagePNG : ImageGDI
	#endif
	{
		#region Construtors
		public ImagePNG(string fileName, bool flip)
		: base(fileName, flip)
		{
			
		}

		public ImagePNG(Stream stream, bool flip)
		: base(stream, flip)
		{
			
		}
		#endregion

		#region Methods
		#if METRO
		public static async Task Save(byte[] data, int width, int height, Stream outStream)
		{
			await ImageMetro.save(data, width, height, outStream, ImageTypes.png);
		}
		#endif
		#endregion
	}
}