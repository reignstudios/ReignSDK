using System.Runtime.InteropServices;
using System.IO;
using System;
using Reign.Core;

namespace Reign.Video
{
	// Header version 2
	[StructLayout(LayoutKind.Sequential)]
	struct PVRHeader
	{
		public uint HeaderLength;
		public uint Height;
		public uint Width;
		public uint NumMipmaps;
		public uint Flags;
		public uint DataLength;
		public uint BPP;
		public uint BitmaskRed;
		public uint BitmaskGreen;
		public uint BitmaskBlue;
		public uint BitmaskAlpha;
		public uint PVRTag;
		public uint NumSurfs;
	}
	
	// Header version 3
	/*[StructLayout(LayoutKind.Sequential)]
	struct PVRHeader
	{
		public uint Version;
		public uint Flags;
		public ulong PixelFormat;
		public uint ColorSpace;
		public uint ChannelType;
		public uint Height;
		public uint Width;
		public uint Depth;
		public uint NumSurfaces;
		public uint NumFaces;
		public uint MIPMapCount;
		public uint MetaDataSize;
	}*/

	public class ImagePVR : Image
	{
		#region Properties
		public const uint PVR_TEXTURE_FLAG_TYPE_MASK = 0xffu;
		// version 2
		private const uint FOURCC_2BPP_RGB  = uint.MaxValue;
		private const uint FOURCC_2BPP_RGBA  = 24;
		private const uint FOURCC_4BPP_RGB  = uint.MaxValue-1;
		private const uint FOURCC_4BPP_RGBA  = 25;
		// version 3
		/*private const uint FOURCC_2BPP_RGB  = 0;
		private const uint FOURCC_2BPP_RGBA  = 1;
		private const uint FOURCC_4BPP_RGB  = 2;
		private const uint FOURCC_4BPP_RGBA  = 3;*/
		private const uint GL_COMPRESSED_RGB_PVRTC_4BPPV1_IMG = 0x8C00u;
		private const uint GL_COMPRESSED_RGB_PVRTC_2BPPV1_IMG = 0x8C01u;
		private const uint GL_COMPRESSED_RGBA_PVRTC_4BPPV1_IMG = 0x8C02u;
		private const uint GL_COMPRESSED_RGBA_PVRTC_2BPPV1_IMG = 0x8C03u;

		public uint FormatGL {get; private set;}
		#endregion

		#region Constructors
		public ImagePVR(string fileName, bool flip)
		{
			new ImageStreamLoader(this, fileName, flip);
		}

		public ImagePVR(Stream stream, bool flip)
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
			// Load Header
			var header = new PVRHeader();
			var reader = new BinaryReader(stream);
			
			// Read header version 2
			header.HeaderLength = reader.ReadUInt32();
			header.Height = reader.ReadUInt32();
			header.Width = reader.ReadUInt32();
			header.NumMipmaps = reader.ReadUInt32();
			header.Flags = reader.ReadUInt32();
			header.DataLength = reader.ReadUInt32();
			header.BPP = reader.ReadUInt32();
			header.BitmaskRed = reader.ReadUInt32();
			header.BitmaskGreen = reader.ReadUInt32();
			header.BitmaskBlue = reader.ReadUInt32();
			header.BitmaskAlpha = reader.ReadUInt32();
			header.PVRTag = reader.ReadUInt32();
			header.NumSurfs = reader.ReadUInt32();
			
			// Read header version 3
			/*header.Version = reader.ReadUInt32();
			header.Flags = reader.ReadUInt32();
			header.PixelFormat = reader.ReadUInt64();
			header.ColorSpace = reader.ReadUInt32();
			header.ChannelType = reader.ReadUInt32();
			header.Height = reader.ReadUInt32();
			header.Width = reader.ReadUInt32();
			header.Depth = reader.ReadUInt32();
			header.NumSurfaces = reader.ReadUInt32();
			header.NumFaces = reader.ReadUInt32();
			header.MIPMapCount = reader.ReadUInt32();
			header.MetaDataSize = reader.ReadUInt32();
			
			while (stream.Position <= stream.Length)
			{
				if (stream.Position + sizeof(int) > stream.Length) Debug.ThrowError("ImagePVR", "No data ID");
				if (reader.ReadInt32() == Streams.MakeFourCC('P', 'V', 'R', (char)3)) break;
			}
			
			var key = reader.ReadUInt32();
			var dSize = reader.ReadUInt32();*/
			
			// Get Caps
			Compressed = true;
			
			// Get pixel format
			Size = new Size2((int)header.Width, (int)header.Height);
			int blockSize = 0, bpp = 0, blockWidth = 0, blockHeight = 0, blockDev = 1;
			//switch (header.PixelFormat)// version 3
			switch (header.Flags & PVR_TEXTURE_FLAG_TYPE_MASK)
			{
				case FOURCC_2BPP_RGB:
					FormatGL = GL_COMPRESSED_RGB_PVRTC_2BPPV1_IMG;
					blockSize = 8*4;
					blockWidth = 8;
					blockHeight = 4;
					bpp = 2;
					blockDev = 2;
					break;

				case FOURCC_2BPP_RGBA:
					FormatGL = GL_COMPRESSED_RGBA_PVRTC_2BPPV1_IMG;
					blockSize = 8*4;
					blockWidth = 8;
					blockHeight = 4;
					bpp = 2;
					blockDev = 2;
					break;

				case FOURCC_4BPP_RGB:
					FormatGL = GL_COMPRESSED_RGB_PVRTC_4BPPV1_IMG;
					blockSize = 4*4;
					blockWidth = 4;
					blockHeight = 4;
					bpp = 4;
					break;
					
				case FOURCC_4BPP_RGBA:
					FormatGL = GL_COMPRESSED_RGBA_PVRTC_4BPPV1_IMG;
					blockSize = 4*4;
					blockWidth = 4;
					blockHeight = 4;
					bpp = 4;
					break;

				default:
					Debug.ThrowError("ImagePVR", "Unsuported PVR Format");
					break;
			}
			
			// Create Mipmaps
			header.NumMipmaps++;
			//Mipmaps = new Mipmap[header.MIPMapCount == 0 ? 1 : header.MIPMapCount];// version 3
			Mipmaps = new Mipmap[header.NumMipmaps == 0 ? 1 : header.NumMipmaps];
			var size = Size;
			for (int i = 0; i < Mipmaps.Length; ++i)
			{
				int width = (size.Width/blockWidth), height = (size.Height/blockHeight);
				if (width < 2) width = 2;
				if (height < 2) height = 2;
				int dataSize = (width * height) * ((blockSize * bpp) / 8);
				var data = new byte[dataSize];
				stream.Read(data, 0, dataSize);

				Mipmaps[i] = new Mipmap(data, size.Width, size.Height, blockDev, 4);

				size /= 2;
			}

			Loaded = true;
		}
		#endregion
	}
}