using Reign.Core;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Reign.Audio.XNA
{
	public class SoundWAVInstance : Disposable, SoundInstanceI
	{
		#region Properties
		internal SoundEffectInstance instance;
		public SoundStates State {get; private set;}
		public bool Looped {get{return instance.IsLooped;}}

		public float Volume
		{
			get {return instance.Volume;}
			set {instance.Volume = value;}
		}
		#endregion

		#region Constructors
		public SoundWAVInstance(SoundWAV sound, bool looped)
		: base(sound)
		{
			instance = sound.sound.CreateInstance();
			instance.IsLooped = looped;
			State = SoundStates.Stopped;
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (instance != null && !instance.IsDisposed)
			{
				instance.Dispose();
				instance = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Update()
		{
			switch (instance.State)
			{
				case (SoundState.Playing):
					State = SoundStates.Playing;
					break;
				case (SoundState.Paused):
					State = SoundStates.Paused;
					break;
				case (SoundState.Stopped):
					State = SoundStates.Stopped;
					break;
			}
		}

		public void Play()
		{
			instance.Play();
			State = SoundStates.Playing;
		}

		public void Play(float volume)
		{
			instance.Volume = volume;
			instance.Play();
			State = SoundStates.Playing;
		}

		public void Pause()
		{
			instance.Pause();
			State = SoundStates.Paused;
		}

		public void Stop()
		{
			instance.Stop();
			State = SoundStates.Stopped;
		}
		#endregion
	}

	public class SoundWAV : SoundWAVI
	{
		#region Properties
		private Audio audio;
		bool loadedFromContent;
		internal SoundEffect sound;
		#endregion

		#region Constructors
		public SoundWAV(DisposableI parent, string fileName, int instanceCount, bool looped)
		: base(parent)
		{
			audio = parent.FindParentOrSelfWithException<Audio>();
			audio.UpdateCallback += Update;

			loadedFromContent = true;
			sound = parent.FindParentOrSelfWithException<RootDisposable>().Content.Load<SoundEffect>(Streams.StripFileExt(fileName));

			for (int i = 0; i != instanceCount; ++i)
			{
				inactiveInstances.AddLast(new SoundWAVInstance(this, looped));
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (audio != null) audio.UpdateCallback -= Update;
			if (!loadedFromContent && sound != null && !sound.IsDisposed)
			{
				sound.Dispose();
				sound = null;
			}
			base.Dispose();
		}
		#endregion
	}
}
