using Reign.Core;
using Reign_Audio_XAudio_Component;
using System;
using System.IO;

namespace Reign.Audio.XAudio
{
	public class SoundWAVInstance : DisposableResource, ISoundInstance
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
					case SoundWAVInstanceErrors.SourceVoice: Debug.ThrowError("SoundWAVInstance", "Failed to create SourceVoice"); break;
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

	public class SoundWAV : ISoundWAV
	{
		#region Properties
		private Audio audio;
		internal SoundWAVCom com;
		#endregion

		#region Constructors
		public SoundWAV(IDisposableResource parent, string filename, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			new StreamLoader(filename,
			delegate(object sender,  bool succeeded)
			{
				if (succeeded)
				{
					init(parent, ((StreamLoader)sender).LoadedStream, instanceCount, looped, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		protected override void init(IDisposableResource parent, Stream stream, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		{
			base.init(parent, stream, instanceCount, looped, loadedCallback);

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
				if (loadedCallback != null) loadedCallback(this, false);
				return;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
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
