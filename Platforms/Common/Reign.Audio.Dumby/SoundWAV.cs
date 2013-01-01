using Reign.Core;

namespace Reign.Audio.Dumby
{
	public class SoundWAVInstance : Disposable, SoundInstanceI
	{
		#region Properties
		public SoundStates State {get{return SoundStates.Stopped;}}
		public bool Looped {get; private set;}
		public float Volume {get; set;}
		#endregion

		#region Constructors
		public SoundWAVInstance(SoundWAV sound, bool looped)
		: base(sound)
		{
			Looped = looped;
		}
		#endregion

		#region Methods
		public void Update() {}
		public void Play() {}
		public void Play(float volume) {Volume = volume;}
		public void Pause() {}
		public void Stop() {}
		#endregion
	}

	public class SoundWAV : SoundWAVI
	{
		#region Constructors
		public static new SoundWAV New(DisposableI parent, string fileName, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new SoundWAV(parent, fileName, instanceCount, looped, loadedCallback);
		}

		public SoundWAV(DisposableI parent, string fileName, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			for (int i = 0; i != instanceCount; ++i)
			{
				inactiveInstances.AddLast(new SoundWAVInstance(this, looped));
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
		}
		#endregion
	}
}
