using Reign.Core;

namespace Reign.Audio.API
{
	public static class Sound
	{
		public static void Init(AudioTypes type)
		{
			#if WINDOWS || METRO
			if (type == AudioTypes.XAudio) SoundWAVAPI.Init(Reign.Audio.XAudio.SoundWAV.New);
			#endif

			#if XNA
			if (type == AudioTypes.XNA) SoundWAVAPI.Init(Reign.Audio.XNA.SoundWAV.New);
			#endif
			
			#if OSX || iOS
			if (type == AudioTypes.Cocoa) SoundWAVAPI.Init(Reign.Audio.Cocoa.SoundWAV.New);
			#endif

			if (type == AudioTypes.Dumby) SoundWAVAPI.Init(Reign.Audio.Dumby.SoundWAV.New);
		}
	}
}