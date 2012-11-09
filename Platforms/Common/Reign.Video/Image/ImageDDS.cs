/*
 * Insparation and credit for getting the DDS loader working goes to.
 * 1) OpenTK.
 * 2) Kevin Harris.
 * 3) Popescu Alexandru Cristian.
 */

using System.Runtime.InteropServices;
using System.IO;
using System;
using Reign.Core;

using DWORD = System.UInt32;
using WORD = System.Int16;
using LONG = System.Int32;
using LPVOID = System.Int32;

namespace Reign.Video
{
	[StructLayout(LayoutKind.Sequential)]
	struct DDCOLORKEY
	{
		public DWORD dwColorSpaceLowValue;
		public DWORD dwColorSpaceHighValue;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct DDSCAPS2
	{
		public DWORD dwCaps;
		public DWORD dwCaps2;
		public DWORD dwCaps3;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN
		{
			[FieldOffset(0)] public DWORD dwCaps4;
			[FieldOffset(0)] public DWORD dwVolumeDepth;
		}
		public DUMMYUNIONNAMEN Union;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct DDPIXELFORMAT
	{
		public DWORD dwSize;
		public DWORD dwFlags;
		public DWORD dwFourCC;
		
		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN1
		{
			[FieldOffset(0)] public DWORD dwRGBBitCount;
			[FieldOffset(0)] public DWORD dwYUVBitCount;
			[FieldOffset(0)] public DWORD dwZBufferBitDepth;
			[FieldOffset(0)] public DWORD dwAlphaBitDepth;
			[FieldOffset(0)] public DWORD dwLuminanceBitCount;
			[FieldOffset(0)] public DWORD dwBumpBitCount;   
			[FieldOffset(0)] public DWORD dwPrivateFormatBitCount;
		}
		public DUMMYUNIONNAMEN1 Union1;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN2
		{
			[FieldOffset(0)] public DWORD dwRBitMask;
			[FieldOffset(0)] public DWORD dwYBitMask;
			[FieldOffset(0)] public DWORD dwStencilBitDepth;
			[FieldOffset(0)] public DWORD dwLuminanceBitMask;
			[FieldOffset(0)] public DWORD dwBumpDuBitMask;
			[FieldOffset(0)] public DWORD dwOperations;
		}
		public DUMMYUNIONNAMEN2 Union2;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN3
		{
			[FieldOffset(0)] public DWORD dwGBitMask;
			[FieldOffset(0)] public DWORD dwUBitMask;
			[FieldOffset(0)] public DWORD dwZBitMask;
			[FieldOffset(0)] public DWORD dwBumpDvBitMask;

			[StructLayout(LayoutKind.Sequential)]
			public struct Caps
			{
				public WORD wFlipMSTypes;
				public WORD wBltMSTypes;
			}
			[FieldOffset(0)] public Caps MultiSampleCaps;
		}
		public DUMMYUNIONNAMEN3 Union3;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN4
		{
			[FieldOffset(0)] public DWORD dwBBitMask;
			[FieldOffset(0)] public DWORD dwVBitMask;
			[FieldOffset(0)] public DWORD dwStencilBitMask;
			[FieldOffset(0)] public DWORD dwBumpLuminanceBitMask;
		}
		public DUMMYUNIONNAMEN4 Union4;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN5
		{
			[FieldOffset(0)] public DWORD dwRGBAlphaBitMask;
			[FieldOffset(0)] public DWORD dwYUVAlphaBitMask;
			[FieldOffset(0)] public DWORD dwLuminanceAlphaBitMask;
			[FieldOffset(0)] public DWORD dwRGBZBitMask;
			[FieldOffset(0)] public DWORD dwYUVZBitMask;
		}
		public DUMMYUNIONNAMEN5 Union5;
	}

	[StructLayout(LayoutKind.Sequential)]
	struct DDSURFACEDESC2
	{
		public uint MagicNumber;
		public DWORD dwSize;
		public DWORD dwFlags;
		public DWORD dwHeight;
		public DWORD dwWidth;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN1
		{
			[FieldOffset(0)] public LONG lPitch;
			[FieldOffset(0)] public DWORD dwLinearSize;
		}
		public DUMMYUNIONNAMEN1 Union1;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN5
		{
			[FieldOffset(0)] public DWORD dwBackBufferCount;
			[FieldOffset(0)] public DWORD dwDepth;
		}
		public DUMMYUNIONNAMEN5 Union5;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN2
		{
			[FieldOffset(0)] public DWORD dwMipMapCount;
			[FieldOffset(0)] public DWORD dwRefreshRate;
			[FieldOffset(0)] public DWORD dwSrcVBHandle;
		}
		public DUMMYUNIONNAMEN2 Union2;

		public DWORD dwAlphaBitDepth;
		public DWORD dwReserved;
		public LPVOID lpSurface;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN3
		{
			[FieldOffset(0)] public DDCOLORKEY ddckCKDestOverlay;
			[FieldOffset(0)] public DWORD dwEmptyFaceColor;
		}
		public DUMMYUNIONNAMEN3 Union3;

		public DDCOLORKEY ddckCKDestBlt;
		public DDCOLORKEY ddckCKSrcOverlay;
		public DDCOLORKEY ddckCKSrcBlt;

		[StructLayout(LayoutKind.Explicit)]
		public struct DUMMYUNIONNAMEN4
		{
			[FieldOffset(0)] public DDPIXELFORMAT ddpfPixelFormat;
			[FieldOffset(0)] public DWORD dwFVF;
		}
		public DUMMYUNIONNAMEN4 Union4;

		public DDSCAPS2 ddsCaps;
		public DWORD dwTextureStage;
	}

	public class ImageDDS : Image
	{
		#region Properties
		private const uint FOURCC_DXT1 = 0x31545844u;
		private const uint FOURCC_DXT3 = 0x33545844u;
		private const uint FOURCC_DXT5 = 0x35545844u;
		private const uint FOURCC_ATC_RGB = 0x20435441u;
		private const uint FOURCC_ATC_RGBA_EXPLICIT = 0x41435441u;
		private const uint FOURCC_ATC_RGBA_INTERPOLATED = 0x49435441u;
		private const uint GL_COMPRESSED_RGBA_S3TC_DXT1_EXT = 0x83F1u;
		private const uint GL_COMPRESSED_RGBA_S3TC_DXT3_EXT = 0x83F2u;
		private const uint GL_COMPRESSED_RGBA_S3TC_DXT5_EXT = 0x83F3u;
		private const uint ATC_RGB_AMD = 0x8C92u;
		private const uint ATC_RGBA_EXPLICIT_ALPHA_AMD = 0x8C93u;
		private const uint ATC_RGBA_INTERPOLATED_ALPHA_AMD = 0x87EEu;

		private const uint DDSCAPS2_VOLUME = 0x00200000u;
		private const uint DDSCAPS_COMPLEX = 0x00000008u;
		private const uint DDSCAPS2_CUBEMAP = 0x00000200u;
		private const uint DDPF_FOURCC = 0x00000004u;
		private const uint DDPF_ALPHAPIXELS = 0x00000001u;

		public uint FormatD3D {get; private set;}
		public uint FormatGL {get; private set;}
		#endregion

		#region Constructors
		public ImageDDS(string fileName, bool flip)
		{
			new ImageStreamLoader(this, fileName, flip);
		}

		public ImageDDS(Stream stream, bool flip)
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
			// Load Desc
			DDSURFACEDESC2 desc = new DDSURFACEDESC2();
			var reader = new BinaryReader(stream);
			desc.MagicNumber = reader.ReadUInt32();
			desc.dwSize = reader.ReadUInt32();
			desc.dwFlags = reader.ReadUInt32();
			desc.dwHeight = reader.ReadUInt32();
			desc.dwWidth = reader.ReadUInt32();

			desc.Union1 = new DDSURFACEDESC2.DUMMYUNIONNAMEN1();
			desc.Union1.lPitch = reader.ReadInt32();

			desc.Union5 = new DDSURFACEDESC2.DUMMYUNIONNAMEN5();
			desc.Union5.dwBackBufferCount = reader.ReadUInt32();

			desc.Union2 = new DDSURFACEDESC2.DUMMYUNIONNAMEN2();
			desc.Union2.dwMipMapCount = reader.ReadUInt32();

			desc.dwAlphaBitDepth = reader.ReadUInt32();
			desc.dwReserved = reader.ReadUInt32();
			desc.lpSurface = reader.ReadInt32();

			desc.Union3 = new DDSURFACEDESC2.DUMMYUNIONNAMEN3();
			desc.Union3.ddckCKDestOverlay = new DDCOLORKEY();
			desc.Union3.ddckCKDestOverlay.dwColorSpaceLowValue = reader.ReadUInt32();
			desc.Union3.ddckCKDestOverlay.dwColorSpaceHighValue = reader.ReadUInt32();

			desc.ddckCKDestBlt = new DDCOLORKEY();
			desc.ddckCKDestBlt.dwColorSpaceLowValue = reader.ReadUInt32();
			desc.ddckCKDestBlt.dwColorSpaceHighValue = reader.ReadUInt32();
			desc.ddckCKSrcOverlay = new DDCOLORKEY();
			desc.ddckCKSrcOverlay.dwColorSpaceLowValue = reader.ReadUInt32();
			desc.ddckCKSrcOverlay.dwColorSpaceHighValue = reader.ReadUInt32();
			desc.ddckCKSrcBlt = new DDCOLORKEY();
			desc.ddckCKSrcBlt.dwColorSpaceLowValue = reader.ReadUInt32();
			desc.ddckCKSrcBlt.dwColorSpaceHighValue = reader.ReadUInt32();


			desc.Union4 = new DDSURFACEDESC2.DUMMYUNIONNAMEN4();
			desc.Union4.ddpfPixelFormat = new DDPIXELFORMAT();
			desc.Union4.ddpfPixelFormat.dwSize = reader.ReadUInt32();
			desc.Union4.ddpfPixelFormat.dwFlags = reader.ReadUInt32();
			desc.Union4.ddpfPixelFormat.dwFourCC = reader.ReadUInt32();
			{
				desc.Union4.ddpfPixelFormat.Union1 = new DDPIXELFORMAT.DUMMYUNIONNAMEN1();
				desc.Union4.ddpfPixelFormat.Union1.dwRGBBitCount = reader.ReadUInt32();

				desc.Union4.ddpfPixelFormat.Union2 = new DDPIXELFORMAT.DUMMYUNIONNAMEN2();
				desc.Union4.ddpfPixelFormat.Union2.dwRBitMask = reader.ReadUInt32();

				desc.Union4.ddpfPixelFormat.Union3 = new DDPIXELFORMAT.DUMMYUNIONNAMEN3();
				desc.Union4.ddpfPixelFormat.Union3.MultiSampleCaps = new DDPIXELFORMAT.DUMMYUNIONNAMEN3.Caps();
				desc.Union4.ddpfPixelFormat.Union3.MultiSampleCaps.wFlipMSTypes = reader.ReadInt16();
				desc.Union4.ddpfPixelFormat.Union3.MultiSampleCaps.wBltMSTypes = reader.ReadInt16();

				desc.Union4.ddpfPixelFormat.Union4 = new DDPIXELFORMAT.DUMMYUNIONNAMEN4();
				desc.Union4.ddpfPixelFormat.Union4.dwBBitMask = reader.ReadUInt32();

				desc.Union4.ddpfPixelFormat.Union5 = new DDPIXELFORMAT.DUMMYUNIONNAMEN5();
				desc.Union4.ddpfPixelFormat.Union5.dwRGBAlphaBitMask = reader.ReadUInt32();
			}

			desc.ddsCaps = new DDSCAPS2();
			desc.ddsCaps.dwCaps = reader.ReadUInt32();
			desc.ddsCaps.dwCaps2 = reader.ReadUInt32();
			desc.ddsCaps.dwCaps3 = reader.ReadUInt32();
			desc.ddsCaps.Union = new DDSCAPS2.DUMMYUNIONNAMEN();
			desc.ddsCaps.Union.dwCaps4 = reader.ReadUInt32();

			desc.dwTextureStage = reader.ReadUInt32();
			
			// Check file ext
			if (desc.MagicNumber != 0x20534444u)
			{
				Debug.ThrowError("ImageDDS", "Not a DDS file");
			}

			// Get file caps
			bool isCubemap = ((desc.ddsCaps.dwCaps & DDSCAPS_COMPLEX) != 0) && ((desc.ddsCaps.dwCaps2 & DDSCAPS2_CUBEMAP) != 0);
			bool isVolumeTexture = ((desc.ddsCaps.dwCaps2 & DDSCAPS2_VOLUME) != 0);
			bool hasAlphaChannel = ((desc.Union4.ddpfPixelFormat.dwFlags & DDPF_ALPHAPIXELS) != 0);
			Compressed = ((desc.Union4.ddpfPixelFormat.dwFlags & DDPF_FOURCC) != 0);

			// Get pixel format
			Size = new Size2((int)desc.dwWidth, (int)desc.dwHeight);
			int blockSize = 0, blockDev = 1;
			if (Compressed)
			{
				FormatD3D = desc.Union4.ddpfPixelFormat.dwFourCC;
				switch (desc.Union4.ddpfPixelFormat.dwFourCC)
				{
					case FOURCC_DXT1:
						FormatGL = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT;
						blockSize = 8;
						blockDev = 2;
						SurfaceFormat = SurfaceFormats.DXT1;
						break;

					case FOURCC_DXT3:
						FormatGL = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT;
						blockSize = 16;
						SurfaceFormat = SurfaceFormats.DXT3;
						break;

					case FOURCC_DXT5:
						FormatGL = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT;
						blockSize = 16;
						SurfaceFormat = SurfaceFormats.DXT5;
						break;

					case FOURCC_ATC_RGB:
						FormatGL = ATC_RGB_AMD;
						blockSize = 8;
						blockDev = 2;
						break;

					case FOURCC_ATC_RGBA_EXPLICIT:
						FormatGL = ATC_RGBA_EXPLICIT_ALPHA_AMD;
						blockSize = 16;
						break;

					case FOURCC_ATC_RGBA_INTERPOLATED:
						FormatGL = ATC_RGBA_INTERPOLATED_ALPHA_AMD;
						blockSize = 16;
						break;

					default:
						Debug.ThrowError("ImageDDS", "Unsuported DDS Format");
						break;
				}
			}
			else
			{
				Debug.ThrowError("ImageDDS", "Uncompressed textures not supported yet");
			}

			if (isCubemap || isVolumeTexture) Debug.ThrowError("ImageDDS", "Cubemap and VolumeTextures not supported yet");

			// Create Mipmaps
			Mipmaps = new Mipmap[desc.Union2.dwMipMapCount == 0 ? 1 : desc.Union2.dwMipMapCount];
			var size = Size;
			for (int i = 0; i < Mipmaps.Length; ++i)
			{
				int dataSize = (((size.Width+3)/4) * ((size.Height+3)/4)) * blockSize;
				var data = new byte[dataSize];
				stream.Read(data, 0, dataSize);

				if (flip && (FormatD3D == FOURCC_DXT1 || FormatD3D == FOURCC_DXT3 || FormatD3D == FOURCC_DXT5))
				{
					data = flipCompressedData(data, size.Width, size.Height, blockSize);
				}
				Mipmaps[i] = new Mipmap(data, size.Width, size.Height, blockDev, 4);

				size /= 2;
			}

			Loaded = true;
		}

		private byte[] flipCompressedData(byte[] data, int width, int height, int blockSize)
		{
			int rowCount = (width + 3) / 4;
			int columnCount = (height + 3) / 4;

			var dataOut = new byte[data.Length];
			for (int column = 0; column < columnCount; ++column)
			{
				int targetColumn = columnCount - column - 1;
				for (int row = 0; row < rowCount; ++row)
				{
					int dst = (targetColumn * rowCount + row) * blockSize;
					int src = (column * rowCount + row) * blockSize;
					switch (FormatD3D)
					{
						case (FOURCC_DXT1):
							// Color only
							dataOut[dst + 0] = data[src + 0];
							dataOut[dst + 1] = data[src + 1];
							dataOut[dst + 2] = data[src + 2];
							dataOut[dst + 3] = data[src + 3];
							dataOut[dst + 4] = data[src + 7];
							dataOut[dst + 5] = data[src + 6];
							dataOut[dst + 6] = data[src + 5];
							dataOut[dst + 7] = data[src + 4];
							break;
						case (FOURCC_DXT3):
							// Alpha
							dataOut[dst + 0] = data[src + 6];
							dataOut[dst + 1] = data[src + 7];
							dataOut[dst + 2] = data[src + 4];
							dataOut[dst + 3] = data[src + 5];
							dataOut[dst + 4] = data[src + 2];
							dataOut[dst + 5] = data[src + 3];
							dataOut[dst + 6] = data[src + 0];
							dataOut[dst + 7] = data[src + 1];

							// Color
							dataOut[dst + 8] = data[src + 8];
							dataOut[dst + 9] = data[src + 9];
							dataOut[dst + 10] = data[src + 10];
							dataOut[dst + 11] = data[src + 11];
							dataOut[dst + 12] = data[src + 15];
							dataOut[dst + 13] = data[src + 14];
							dataOut[dst + 14] = data[src + 13];
							dataOut[dst + 15] = data[src + 12];
							break;
						case (FOURCC_DXT5):
							// Alpha, the first 2 bytes remain 
							dataOut[dst + 0] = data[src + 0];
							dataOut[dst + 1] = data[src + 1];

							// extract 3 bits each and flip them
							GetBytesFromUInt24(ref dataOut, (uint)dst + 5, FlipUInt24(GetUInt24(ref data, (uint)src + 2)));
							GetBytesFromUInt24(ref dataOut, (uint)dst + 2, FlipUInt24(GetUInt24(ref data, (uint)src + 5)));

							// Color
							dataOut[dst + 8] = data[src + 8];
							dataOut[dst + 9] = data[src + 9];
							dataOut[dst + 10] = data[src + 10];
							dataOut[dst + 11] = data[src + 11];
							dataOut[dst + 12] = data[src + 15];
							dataOut[dst + 13] = data[src + 14];
							dataOut[dst + 14] = data[src + 13];
							dataOut[dst + 15] = data[src + 12];
							break;
					}
				}
			}

			return dataOut;
		}

		private uint GetUInt24( ref byte[] input, uint offset )
		{
			return (uint)((input[offset + 2] * 256 + input[offset + 1]) * 256 + input[offset + 0]);
		}

		private uint FlipUInt24(uint inputUInt24)
		{
			const uint BitMask = 0x00000007;

			byte[][] ThreeBits = new byte[2][];
			for ( int i = 0; i < 2; i++ )
			{
				ThreeBits[i] = new byte[4];
			}

			// extract 3 bits each into the array
			ThreeBits[0][0] = (byte)(inputUInt24 & BitMask);
			inputUInt24 >>= 3;
			ThreeBits[0][1] = (byte)(inputUInt24 & BitMask);
			inputUInt24 >>= 3;
			ThreeBits[0][2] = (byte)(inputUInt24 & BitMask);
			inputUInt24 >>= 3;
			ThreeBits[0][3] = (byte)(inputUInt24 & BitMask);
			inputUInt24 >>= 3;
			ThreeBits[1][0] = (byte)(inputUInt24 & BitMask);
			inputUInt24 >>= 3;
			ThreeBits[1][1] = (byte)(inputUInt24 & BitMask);
			inputUInt24 >>= 3;
			ThreeBits[1][2] = (byte)(inputUInt24 & BitMask);
			inputUInt24 >>= 3;
			ThreeBits[1][3] = (byte)(inputUInt24 & BitMask);

			// stuff 8x 3bits into 3 bytes
			uint Result = 0;
			Result = Result | (uint)(ThreeBits[1][0] << 0);
			Result = Result | (uint)(ThreeBits[1][1] << 3);
			Result = Result | (uint)(ThreeBits[1][2] << 6);
			Result = Result | (uint)(ThreeBits[1][3] << 9);
			Result = Result | (uint)(ThreeBits[0][0] << 12);
			Result = Result | (uint)(ThreeBits[0][1] << 15);
			Result = Result | (uint)(ThreeBits[0][2] << 18);
			Result = Result | (uint)(ThreeBits[0][3] << 21);
			return Result;
		}

		private void GetBytesFromUInt24(ref byte[] input, uint offset, uint splitme)
		{
			input[offset + 0] = (byte)(splitme & 0x000000ff);
			input[offset + 1] = (byte)((splitme & 0x0000ff00 ) >> 8);
			input[offset + 2] = (byte)((splitme & 0x00ff0000 ) >> 16);
			return;
		}
		#endregion
	}
}