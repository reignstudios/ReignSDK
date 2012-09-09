using Reign.Core;

namespace Reign.Audio.Dumby
{
	public class SoundWAVInstance : Disposable, SoundInstanceI
	{
		public SoundStates State {get{return SoundStates.Stopped;}}
		public bool Looped {get; private set;}
		public float Volume {get; set;}

		public SoundWAVInstance(SoundWAV sound, bool looped)
		: base(sound)
		{
			Looped = looped;
		}

		public void Update() {}
		public void Play() {}
		public void Play(float volume) {Volume = volume;}
		public void Pause() {}
		public void Stop() {}
	}

	public class SoundWAV : SoundWAVI
	{
		public SoundWAV(DisposableI parent, string fileName, int instanceCount, bool looped)
		: base(parent)
		{
			for (int i = 0; i != instanceCount; ++i)
			{
				inactiveInstances.AddLast(new SoundWAVInstance(this, looped));
			}

			Loaded = true;
		}
	}
}
