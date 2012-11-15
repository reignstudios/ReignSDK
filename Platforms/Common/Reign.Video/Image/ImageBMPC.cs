using System.IO;
using System;
using System.Runtime.InteropServices;
using Reign.Core;

#if NaCl
using ICSharpCode.SharpZipLib.GZip;
#else
using System.IO.Compression;
#endif

#if METRO
using System.Threading.Tasks;
#endif

namespace Reign.Video
{
	public class ImageBMPC : Image
	{
		#region Construtors
		public ImageBMPC(string fileName, bool flip)
		{
			new ImageStreamLoader(this, fileName, flip);
		}

		public ImageBMPC(Stream stream, bool flip)
		{
			#if METRO
			init(stream, flip).Wait();
			#else
			init(stream, flip);
			#endif
		}

		#if METRO
		protected override async System.Threading.Tasks.Task init(Stream stream, bool flip)
		#else
		protected override void init(Stream stream, bool flip)
		#endif
		{
			SurfaceFormat = SurfaceFormats.RGBAx8;

			using (var reader = new BinaryReader(stream))
			{
				// File Type
				int type = reader.ReadInt32();
				if (type != Streams.MakeFourCC('b', 'm', 'p', 'c')) throw new Exception("Not a .bmpc file");

				// Version
				float version = reader.ReadSingle();
				if (version != 1.0f) throw new Exception("Unsuported .bmpc version");

				// Meta Data
				Size = new Size2(reader.ReadInt32(), reader.ReadInt32());
				bool zipCompressed = reader.ReadBoolean();

				// Data
				using (var decompressedDataStream = new MemoryStream())
				{
					int dataLength = reader.ReadInt32();
					int dataRead = 0;
					do
					{
						int read = 1024;
						if ((dataRead + read) > dataLength) read -= (int)((dataRead + read) - dataLength);

						var data = reader.ReadBytes(read);
						decompressedDataStream.Write(data, 0, data.Length);

						dataRead += read;
					} while (dataRead < dataLength);
					decompressedDataStream.Position = 0;

					#if NaCl
					using (var zip = new GZipInputStream(decompressedDataStream))
					using (var dataStream = new MemoryStream())
					{
						var buffer = new byte[4096];
						int read = 0;
						do
						{
							read = zip.Read(buffer, 0, buffer.Length);
							dataStream.Write(buffer, 0, buffer.Length);
							
						} while (read > 0);
						
						Mipmaps = new Mipmap[1];
						Mipmaps[0] = new Mipmap(dataStream.GetBuffer(), Size.Width, Size.Height);
						if (flip) Mipmaps[0].FlipVertical();
					}
					#else
					using (var decompressedStream = new GZipStream(decompressedDataStream, CompressionMode.Decompress))
					using (var dataStream = new MemoryStream())
					{
						decompressedStream.CopyTo(dataStream);
						Mipmaps = new Mipmap[1];
						Mipmaps[0] = new Mipmap(dataStream.ToArray(), Size.Width, Size.Height, 1, 4);
						if (flip) Mipmaps[0].FlipVertical();
					}
					#endif
				}
			}

			Loaded = true;
		}
		#endregion

		#region Methods
		#if !NaCl
		#if METRO
		public static async Task Save(byte[] data, int width, int height, Stream outStream)
		#else
		public static void Save(byte[] data, int width, int height, Stream outStream)
		#endif
		{
			using (var dataStream = new MemoryStream(data))
			using (var writer = new BinaryWriter(outStream))
			{
				// File Type
				int type = Streams.MakeFourCC('b', 'm', 'p', 'c');
				writer.Write(type);

				// Version
				float version = 1.0f;
				writer.Write(version);

				// Meta Data
				writer.Write(width);
				writer.Write(height);
				writer.Write(true);// Zip Compressed

				// Data
				using (var compressedDataStream = new MemoryStream())
				using (var compressedStream = new GZipStream(compressedDataStream, CompressionMode.Compress))
				{
					dataStream.CopyTo(compressedStream);
					#if !METRO
					compressedStream.Close();
					#endif

					var compressedData = compressedDataStream.ToArray();
					writer.Write(compressedData.Length);
					writer.Write(compressedData);
				}
			}
		}
		#endif
		#endregion
	}
}