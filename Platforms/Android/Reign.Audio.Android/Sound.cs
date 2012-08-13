using System;
using System.IO;
using Reign.Core;

namespace Reign.Audio.Android
{
	public static class Sound
	{
		public static SoundI Load(DisposableI parent, string fileName, int instanceCount, bool looped)
		{
			string ext = Streams.GetFileExt(fileName);
			if (ext == ".wav") return new SoundWAV(parent, fileName, instanceCount, looped);
			
			Debug.ThrowError("Sound", string.Format("File 'ext' {0} not supported", ext));
			return null;
		}
	}
}