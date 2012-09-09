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
			
			Application.PauseCallback += Pause;
			Application.ResumeCallback += resume;
		}

		public override void Dispose()
		{
			disposeChilderen();
			Application.PauseCallback -= Pause;
			Application.ResumeCallback -= resume;
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
			lock (this)
			{
				if (instance == null) createPlayInstance();
				if (State != SoundStates.Paused) volume = 1;
				instance.Play();
				State = SoundStates.Playing;
			}
		}
		
		public void Play(float volume)
		{
			lock (this)
			{
				if (instance == null) createPlayInstance();
				this.volume = volume;
				instance.Play();
				State = SoundStates.Playing;
			}
		}

		public void Pause()
		{
			lock (this)
			{
				if (instance == null) return;
				instance.Pause();
				State = SoundStates.Paused;
			}
		}

		public void Stop()
		{
			lock (this)
			{
				if (instance == null) return;
				instance.Stop();
				instance.Release();
				instance.Dispose();
				instance = null;
				audio.removeinstance();
				State = SoundStates.Stopped;
			}
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
		public SoundWAV(DisposableI parent, string fileName, int instanceCount, bool looped)
		: base(parent, fileName)
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
