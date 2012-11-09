using Reign.Core;
using Reign_Audio_XAudio_Component;
using System;

#if METRO
using System.Threading.Tasks;
#endif

namespace Reign.Audio.XAudio
{
	public class SoundWAVInstance : Disposable, SoundInstanceI
	{
		#region Properties
		private SoundWAVInstanceCom com;
		public SoundStates State {get; private set;}
		public bool Looped {get; private set;}

		private float volume;
		public float Volume
		{
			get {return volume;}
			set
			{
				volume = value;
				com.SetVolume(value);
			}
		}
		#endregion

		#region Constructors
		public SoundWAVInstance(SoundWAV sound, bool looped)
		: base(sound)
		{
			try
			{
				State = SoundStates.Stopped;
				Looped = looped;
				volume = 1;

				com = new SoundWAVInstanceCom();
				var error = com.Init(sound.com, looped);

				switch (error)
				{
					case (SoundWAVInstanceErrors.SourceVoice): Debug.ThrowError("SoundWAVInstance", "Failed to create SourceVoice"); break;
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Update()
		{
			bool buffered = com.Update();
			if (State == SoundStates.Playing) State = buffered ? SoundStates.Playing : SoundStates.Stopped;
		}

		public void Play()
		{
			if (State == SoundStates.Playing) return;
			if (State != SoundStates.Paused) Volume = 1;
			com.Play();
			State = SoundStates.Playing;
		}

		public void Play(float volume)
		{
			if (State == SoundStates.Playing) return;
			this.volume = volume;
			com.Play(volume);
			State = SoundStates.Playing;
		}

		public void Pause()
		{
			if (State == SoundStates.Paused) return;
			com.Pause();
			State = SoundStates.Paused;
		}

		public void Stop()
		{
			if (State == SoundStates.Stopped) return;
			com.Stop();
			State = SoundStates.Stopped;
		}
		#endregion
	}

	class SoundWAVStreamLoader : StreamLoaderI
	{
		SoundWAV sound;
		private DisposableI parent;
		private string fileName;
		private int instanceCount;
		private bool looped;

		public SoundWAVStreamLoader(SoundWAV sound, DisposableI parent, string fileName, int instanceCount, bool looped)
		{
			this.sound = sound;
			this.parent = parent;
			this.fileName = fileName;
			this.instanceCount = instanceCount;
			this.looped = looped;
		}

		#if METRO
		public override async Task<bool> Load()
		{
			await sound.load(parent, fileName, instanceCount, looped);
			return true;
		}
		#else
		public override bool Load()
		{
			sound.load(parent, fileName, instanceCount, looped);
			return true;
		}
		#endif
	}

	public class SoundWAV : SoundWAVI
	{
		#region Properties
		private Audio audio;
		internal SoundWAVCom com;
		#endregion

		#region Constructors
		public SoundWAV(DisposableI parent, string fileName, int instanceCount, bool looped)
		: base(parent)
		{
			new SoundWAVStreamLoader(this, parent, fileName, instanceCount, looped);
		}

		#if METRO
		internal async Task load(DisposableI parent, string fileName, int instanceCount, bool looped) {
		#else
		internal void load(DisposableI parent, string fileName, int instanceCount, bool looped) {
		#endif
			#if METRO
			await init(parent, fileName, instanceCount, looped);
			#else
			init(parent, fileName, instanceCount, looped);
			#endif
		}

		#if METRO
		protected override async Task init(DisposableI parent, string fileName, int instanceCount, bool looped) {
			await base.init(parent, fileName, instanceCount, looped);
		#else
		protected override void init(DisposableI parent, string fileName, int instanceCount, bool looped) {
			base.init(parent, fileName, instanceCount, looped);
		#endif
			try
			{
				audio = parent.FindParentOrSelfWithException<Audio>();
				audio.UpdateCallback += Update;

				com = new SoundWAVCom();
				var error = com.Init(audio.com, data, formatCode, channels, sampleRate, formatAvgBytesPerSec, formatBlockAlign, bitDepth, formatExtraSize);
				data = null;

				for (int i = 0; i != instanceCount; ++i)
				{
					inactiveInstances.AddLast(new SoundWAVInstance(this, looped));
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (audio != null) audio.UpdateCallback -= Update;
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
		}
		#endregion
	}
}
