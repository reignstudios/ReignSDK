using Reign.Core;

namespace Reign.Audio.API
{
	public static class Music
	{
		public static void Init(AudioTypes type)
		{
			#if XNA
			if (type == AudioTypes.XNA) MusicAPI.Init(Reign.Audio.XNA.Music.New);
			#endif

			if (type == AudioTypes.Dumby) MusicAPI.Init(Reign.Audio.Dumby.Music.New);
		}
	}
}
