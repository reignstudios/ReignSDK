using Reign.Core;

namespace Reign.Audio
{
	public enum MusicStates
	{
		Playing,
		Paused,
		Stopped
	}

	public interface MusicI : DisposableI
	{
		MusicStates State {get;}

		void Update();
		void Play();
		void Stop();
	}

	public static class MusicAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			MusicAPI.newPtr = newPtr;
		}

		public delegate MusicI NewPtrMethod(DisposableI parent, string fileName);
		private static NewPtrMethod newPtr;
		public static MusicI New(DisposableI parent, string fileName)
		{
			return newPtr(parent, fileName);
		}
	}
}
