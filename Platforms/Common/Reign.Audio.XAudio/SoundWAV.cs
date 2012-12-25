using Reign.Core;
using Reign_Audio_XAudio_Component;
using System;
using System.IO;

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

	public class SoundWAV : SoundWAVI
	{
		#region Properties
		private Audio audio;
		internal SoundWAVCom com;
		#endregion

		#region Constructors
		public static new SoundWAV New(DisposableI parent, string fileName, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			return new SoundWAV(parent, fileName, instanceCount, looped, loadedCallback, failedToLoadCallback);
		}

		public SoundWAV(DisposableI parent, string fileName, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		: base(parent)
		{
			new StreamLoader(fileName,
			delegate(object sender)
			{
				init(parent, ((StreamLoader)sender).LoadedStream, instanceCount, looped, loadedCallback, failedToLoadCallback);
			},
			delegate
			{
				FailedToLoad = true;
				Dispose();
				if (failedToLoadCallback != null) failedToLoadCallback();
			});
		}

		protected override void init(DisposableI parent, Stream stream, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			base.init(parent, stream, instanceCount, looped, loadedCallback, failedToLoadCallback);

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
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (failedToLoadCallback != null) failedToLoadCallback();
				return;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this);
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
