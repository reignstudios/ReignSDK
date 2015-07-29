using Reign.Core;

namespace Reign.Audio.Abstraction
{
	public static class MusicAPI
	{
		public static IMusic New(IDisposableResource parent, string filename)
		{
			return New(AudioAPI.DefaultAPI, parent, filename);
		}

		public static IMusic New(AudioTypes audioType, IDisposableResource parent, string filename)
		{
			IMusic api = null;

			#if XNA
			if (audioType == AudioTypes.XNA) api = new XNA.Music(parent, filename);
			#endif

			if (audioType == AudioTypes.Dumby) api = new Dumby.Music(parent, filename);

			if (api == null) Debug.ThrowError("MusicAPI", "Unsuported InputType: " + audioType);
			return api;
		}
	}
}
