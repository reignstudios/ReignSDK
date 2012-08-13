using Reign.Core;

namespace Reign.Audio.Dumby
{
	public static class Sound
	{
		public static SoundI Load(DisposableI parent, string fileName, int instanceCount, bool looped)
		{
			return new SoundWAV(parent, fileName, instanceCount, looped);
		}
	}
}
