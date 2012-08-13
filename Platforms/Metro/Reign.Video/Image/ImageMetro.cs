using System;
using System.IO;
using Reign.Core;

namespace Reign.Video
{
	public class ImageMetro : Image
	{
		public ImageMetro(string fileName, bool flip, bool generateMipmaps)
		{
			using (var stream = Streams.OpenFile(fileName))
			{
				init(stream, flip, generateMipmaps);
			}
		}

		public ImageMetro(Stream stream, bool flip, bool generateMipmaps)
		{
			init(stream, flip, generateMipmaps);
		}

		private void init(Stream stream, bool flip, bool generateMipmaps)
		{
			
		}
	}
}
