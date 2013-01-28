using Reign.Core;
using System.Collections.Generic;
using System;
using Android.Media;

namespace Reign.Audio.Android
{
	public class SoundWAVInstance : Disposable, SoundInstanceI
	{
		#region Properties
		private Audio audio;
		private SoundWAV sound;
		private AudioTrack instance;
		private int totalSamples;
		public SoundStates State {get; private set;}
		
		private bool looped;
		public bool Looped {get{return looped;}}

		private float volume;
		public float Volume
		{
			get {return volume;}
			set
			{
				if (instance == null) return;
				volume = value;
				instance.SetStereoVolume(value, value);
			}
		}
		#endregion

		#region Constructors
		public SoundWAVInstance(SoundWAV sound, bool looped)
		: base(sound)
		{
			this.sound = sound;
			audio = sound.audio;
			this.looped = looped;
			State = SoundStates.Stopped;
			volume = 1;
			
			OS.CurrentApplication.PauseCallback += Pause;
			OS.CurrentApplication.ResumeCallback += resume;
		}

		public override void Dispose()
		{
			disposeChilderen();
			OS.CurrentApplication.PauseCallback -= Pause;
			OS.CurrentApplication.ResumeCallback -= resume;
			if (instance != null) Stop();
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Update()
		{
			if (instance == null) return;
			if (instance.PlaybackHeadPosition >= totalSamples) State = SoundStates.Stopped;
			if (State == SoundStates.Stopped) Stop();
		}
		
		private void resume()
		{
			if (State == SoundStates.Paused) Play();
		}
		
		private void createPlayInstance()
		{
			var encoding = sound.bitDepth == 16 ? Encoding.Pcm16bit : Encoding.Pcm8bit;
			totalSamples = sound.data.Length;
			if (sound.bitDepth == 16) totalSamples /= 2;
			if (sound.channels == 2) totalSamples /= 2;
	
			instance = new AudioTrack(Stream.Music, sound.sampleRate, sound.channels == 2 ? ChannelConfiguration.Stereo : ChannelConfiguration.Mono, encoding, sound.data.Length, AudioTrackMode.Static);
			instance.Write(sound.data, 0, sound.data.Length);
			if (looped) instance.SetLoopPoints(0, totalSamples, -1);
		}

		public void Play()
		{
			if (State == SoundStates.Playing) return;
			if (instance == null) createPlayInstance();
			if (State != SoundStates.Paused) volume = 1;
			instance.Play();
			State = SoundStates.Playing;
		}
		
		public void Play(float volume)
		{
			if (State == SoundStates.Playing) return;
			if (instance == null) createPlayInstance();
			this.volume = volume;
			instance.Play();
			State = SoundStates.Playing;
		}

		public void Pause()
		{
			if (State == SoundStates.Paused) return;
			if (instance == null) return;
			instance.Pause();
			State = SoundStates.Paused;
		}

		public void Stop()
		{
			if (State == SoundStates.Stopped) return;
			if (instance == null) return;
			instance.Stop();
			instance.Release();
			instance.Dispose();
			instance = null;
			audio.removeinstance();
			State = SoundStates.Stopped;
		}
		#endregion
	}

	public class SoundWAV : SoundWAVI
	{
		#region Properties
		internal Audio audio;
		internal int soundID;
		internal new byte[] data;
		internal new short channels, bitDepth;
		internal new int sampleRate;
		#endregion

		#region Constructors
		public static new SoundWAV New(DisposableI parent, string fileName, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new SoundWAV(parent, fileName, instanceCount, looped, loadedCallback);
		}
		
		public SoundWAV(DisposableI parent, string fileName, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			new StreamLoader(fileName,
			delegate(object sender, bool succeeded)
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
		
		protected override void init(DisposableI parent, System.IO.Stream stream, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		{
			base.init(parent, stream, instanceCount, looped, loadedCallback);
		
			try
			{
				audio = parent.FindParentOrSelfWithException<Audio>();
				audio.updateCallback += Update;
				this.data = base.data;
				this.channels = base.channels;
				this.sampleRate = base.sampleRate;
				this.bitDepth = base.bitDepth;
	
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
			if (audio != null) audio.updateCallback -= Update;
			base.Dispose();
		}
		#endregion
		
		#region Methods
		public override SoundInstanceI Play (float volume)
		{
			if (!audio.addInstance()) return null;
			return base.Play (volume);
		}
		#endregion
	}
}
