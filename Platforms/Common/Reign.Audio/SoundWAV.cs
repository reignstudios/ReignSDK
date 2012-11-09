using Reign.Core;
using System.Collections.Generic;
using System.IO;
using System;

#if METRO
using System.Threading.Tasks;
#endif

namespace Reign.Audio
{
	public class SoundWAVI : SoundI
	{
		#region Properites
		protected int chunkID, chunkSize, riffType, formatID, formatSize, sampleRate, formatAvgBytesPerSec, dataID, dataSize;
		protected short formatCode, channels, formatBlockAlign, bitDepth, formatExtraSize;
		protected byte[] data;
		
		public TimeSpan TotalTime {get; private set;}
		#endregion

		#region Constructors
		public SoundWAVI(DisposableI parent)
		: base(parent)
		{
			// WAV will be handled by implomenting class
		}
		
		protected void readMetaData(Stream stream)
		{
			using (var reader = new BinaryReader(stream))
			{
				readMetaData(stream, reader);
			}
		}
		
		protected void readMetaData(Stream stream, BinaryReader reader)
		{
			// check to make sure is WAV file
			chunkID = reader.ReadInt32();
			if (chunkID != Streams.MakeFourCC('R', 'I', 'F', 'F'))
			{
				Debug.ThrowError("SoundWAVI", "Not a valid WAV file - No RIFF ID");
			}
			chunkSize = reader.ReadInt32();

			riffType = reader.ReadInt32();
			if (riffType != Streams.MakeFourCC('W', 'A', 'V', 'E'))
			{
				Debug.ThrowError("SoundWAVI", "Not a WAV file - No WAVE ID");
			}

			// navigate to 'fmt' chunk
			while (stream.Position <= stream.Length)
			{
				if (stream.Position + sizeof(int) > stream.Length) Debug.ThrowError("SoundWAVI", "No fmt ID");
				formatID = reader.ReadInt32();
				if (formatID == Streams.MakeFourCC('f', 'm', 't', ' ')) break;
			}
			formatSize = reader.ReadInt32();
			formatCode = reader.ReadInt16();
			channels = reader.ReadInt16();
			sampleRate = reader.ReadInt32();
			formatAvgBytesPerSec = reader.ReadInt32();
			formatBlockAlign = reader.ReadInt16();
			bitDepth = reader.ReadInt16();

			formatExtraSize = 0;
			if (formatSize == 18)
			{
				// Read any extra values
				formatExtraSize = reader.ReadInt16();
				reader.ReadBytes(formatExtraSize);
			}
			
			// navigate to 'data' chunk
			while (stream.Position <= stream.Length)
			{
				if (stream.Position + sizeof(int) > stream.Length) Debug.ThrowError("SoundWAVI", "No data ID");
				dataID = reader.ReadInt32();
				if (dataID == Streams.MakeFourCC('d', 'a', 't', 'a')) break;
			}
			dataSize = reader.ReadInt32();
			
			TotalTime = TimeSpan.FromSeconds(dataSize / formatAvgBytesPerSec);
		}

		#if METRO
		protected virtual async Task init(DisposableI parent, string fileName, int instanceCount, bool looped) {
			using (var stream = await Streams.OpenFile(fileName))
		#else
		protected virtual void init(DisposableI parent, string fileName, int instanceCount, bool looped) {
			using (var stream = Streams.OpenFile(fileName))
		#endif
			using (var reader = new BinaryReader(stream))
			{
				readMetaData(stream, reader);
				data = reader.ReadBytes(dataSize);
			}

			Loaded = true;
		}
		#endregion
	}
}
