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
			if (State == SoundStates.Playing) return;
			instance.Play();
			State = SoundStates.Playing;
		}

		public void Play(float volume)
		{
			if (State == SoundStates.Playing) return;
			instance.Volume = volume;
			instance.Play();
			State = SoundStates.Playing;
		}

		public void Pause()
		{
			if (State == SoundStates.Paused) return;
			instance.Pause();
			State = SoundStates.Paused;
		}

		public void Stop()
		{
			if (State == SoundStates.Stopped) return;
			instance.Stop();
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

		public override bool Load()
		{
			sound.load(parent, fileName, instanceCount, looped);
			return true;
		}
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
			new SoundWAVStreamLoader(this, parent, fileName, instanceCount, looped);
		}

		internal void load(DisposableI parent, string fileName, int instanceCount, bool looped)
		{
			init(parent, fileName, instanceCount, looped);
		}

		protected override void init(DisposableI parent, string fileName, int instanceCount, bool looped)
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
