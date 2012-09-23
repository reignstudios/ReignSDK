using System;
using Reign.Core;

namespace Reign.Audio.OpenAL
{
	public class SoundWAVInstance : Disposable, SoundInstanceI
	{
		#region Properties
		private uint instance;
		public SoundStates State {get; private set;}
		public bool Looped {get; private set;}
		
		private float volume;
		public float Volume
		{
			get {return volume;}
			set
			{
				volume = value;
				AL.Sourcef(instance, AL.GAIN, value);
			}
		}
		#endregion
		
		#region Constructors
		public unsafe SoundWAVInstance(SoundWAV sound, bool looped)
		: base(sound)
		{
			State = SoundStates.Stopped;
			Looped = looped;
			Volume = 1;
			
			uint instanceTEMP = 0;
			AL.GenSources(1, &instanceTEMP);
			instance = instanceTEMP;
			if (instance == 0) Debug.ThrowError("SoundWAVInstance", "Failed to create instance");
			
			AL.Sourcei(instance, AL.BUFFER, (int)sound.buffer);
			AL.Sourcef(instance, AL.PITCH, 1);
			AL.Sourcef(instance, AL.GAIN, 1);
			AL.Source3f(instance, AL.POSITION, 0, 0, 0);
			AL.Source3f(instance, AL.VELOCITY, 0, 0, 0);
			AL.Sourcei(instance, AL.LOOPING, looped ? AL.TRUE : AL.FALSE);
		}
		
		public unsafe override void Dispose ()
		{
			disposeChilderen();
			if (instance != 0)
			{
				uint instanceTEMP = instance;
				AL.DeleteSources(1, &instanceTEMP);
				instance = 0;
			}
			base.Dispose ();
		}
		#endregion

		#region Methods
		public unsafe void Update()
		{
			int playingState = 0;
			AL.GetSourcei(instance, AL.SOURCE_STATE, &playingState);
			switch (playingState)
			{
				case(AL.PLAYING): State = SoundStates.Playing; break;
				case(AL.PAUSED): State = SoundStates.Paused; break;
				case(AL.STOPPED): State = SoundStates.Stopped; break;
			}
		}
		
		public void Play()
		{
			if (State != SoundStates.Paused) Volume = 1;
			AL.SourcePlay(instance);
			State = SoundStates.Playing;
		}
		
		public void Play(float volume)
		{
			Volume = volume;
			AL.SourcePlay(instance);
			State = SoundStates.Playing;
		}
		
		public void Pause()
		{
			AL.SourcePause(instance);
			State = SoundStates.Paused;
		}
		
		public void Stop()
		{
			AL.SourceStop(instance);
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
		internal uint buffer;
		#endregion
	
		#region Constructors
		public SoundWAV (DisposableI parent, string fileName, int instanceCount, bool looped)
		: base(parent)
		{
			new SoundWAVStreamLoader(this, parent, fileName, instanceCount, looped);
		}
		
		internal void load(DisposableI parent, string fileName, int instanceCount, bool looped)
		{
			init(parent, fileName, instanceCount, looped);
		}
		
		protected unsafe override void init(DisposableI parent, string fileName, int instanceCount, bool looped)
		{
			base.init(parent, fileName, instanceCount, looped);
		
			try
			{
				audio = parent.FindParentOrSelfWithException<Audio>();
				audio.UpdateCallback += Update;
			
				// Gen buffer
				uint bufferTEMP = 0;
				AL.GenBuffers(1, &bufferTEMP);
				buffer = bufferTEMP;
				if (buffer == 0) Debug.ThrowError("SoundWAV", "Failed to create buffer");
				
				// load wav data
				int format = 0;
				if (bitDepth == 16)
				{
					if (channels == 2) format = AL.FORMAT_STEREO16;
					else format = AL.FORMAT_MONO16;
				}
				else
				{
					if (channels == 2) format = AL.FORMAT_STEREO8;
					else format = AL.FORMAT_MONO8;
				}
				fixed (byte* dataPtr = data)
				{
					AL.BufferData(buffer, format, dataPtr, data.Length, sampleRate);
				}
				data = null;
				
				// create instances
				for (int i = 0; i != instanceCount; ++i)
				{
					inactiveInstances.AddLast(new SoundWAVInstance(this, looped));
				}
			}
			catch (Exception e)
			{
				
			}
		}
		
		public unsafe override void Dispose()
		{
			disposeChilderen();
			if (audio != null) audio.UpdateCallback -= Update;
			if (buffer != 0)
			{
				uint bufferTEMP = buffer;
				AL.DeleteBuffers(1, &bufferTEMP);
				buffer = 0;
			}
			base.Dispose();
		}
		#endregion
	}
}

