using Reign.Core;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

#if OSX
using MonoMac.AudioUnit;
using MonoMac.AudioToolbox;
#endif

#if iOS
using MonoTouch.AudioUnit;
using MonoTouch.AudioToolbox;
#endif

namespace Reign.Audio.Cocoa
{
	public class SoundWAVInstance : Disposable, SoundInstanceI
	{
		#region Properties
		private SoundWAV sound;
		internal AudioUnit instance;
		private int dataOffset;
		public SoundStates State {get; private set;}
		
		private bool looped;
		public bool Looped {get{return looped;}}

		private float volume;
		public float Volume
		{
			get {return volume;}
			set {volume = value;}
		}
		#endregion

		#region Constructors
		public SoundWAVInstance(SoundWAV sound, bool looped)
		: base(sound)
		{
			this.sound = sound;
			this.looped = looped;
			State = SoundStates.Stopped;
			volume = 1;
			
			instance = new AudioUnit(sound.audio.component);
			instance.SetAudioFormat(sound.desc, AudioUnitScopeType.Input, 0);
			if (sound.channels == 2)
			{
				switch (sound.bitDepth)
				{
					case (8): instance.RenderCallback += render2Channel8BitCallback; break;
					case (16): instance.RenderCallback += render2Channel16BitCallback; break;
					default: Debug.ThrowError("SoundWAVInstance", "Unsuported WAV bit depth"); break;
				}
			}
			else
			{
				switch (sound.bitDepth)
				{
					case (8): instance.RenderCallback += render1Channel8BitCallback; break;
					case (16): instance.RenderCallback += render1Channel16BitCallback; break;
					default: Debug.ThrowError("SoundWAVInstance", "Unsuported WAV bit depth"); break;
				}
			}
			instance.Initialize();
			
			#if iOS
			Application.PauseCallback += Pause;
			Application.ResumeCallback += resume;
			#endif
		}

		public override void Dispose()
		{
			disposeChilderen();
			#if iOS
			Application.PauseCallback -= Pause;
			Application.ResumeCallback -= resume;
			#endif
			if (instance != null)
			{
				Stop();
				instance.RenderCallback -= render1Channel8BitCallback;
				instance.RenderCallback -= render2Channel16BitCallback;
				instance.RenderCallback -= render1Channel8BitCallback;
				instance.RenderCallback -= render2Channel16BitCallback;
				#if OSX
				instance.Dispose();// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< FAILS ON iOS
				#endif
				instance = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		#if OSX
		[DllImport(MonoMac.Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileRead")]
		static extern int ExtAudioFileRead(IntPtr  inExtAudioFile, ref int ioNumberFrames, AudioBufferList ioData);
		#endif
		
		#if iOS
		[DllImport(MonoTouch.Constants.AudioToolboxLibrary, EntryPoint = "ExtAudioFileRead")]
		static extern int ExtAudioFileRead(IntPtr  inExtAudioFile, ref int ioNumberFrames, AudioBufferList ioData);
		#endif
		
		private void render1Channel8BitCallback(object sender, AudioUnitEventArgs e)
		{
			int dataFrameSize = e.NumberFrames;
			int readSize = dataFrameSize;
			int nextOffset = readSize + dataOffset;
			if (nextOffset > sound.data.Length) readSize -= nextOffset - sound.data.Length;
			
			unsafe
			{
				int* channel1 = (int*)e.Data.Buffers[0].Data.ToPointer();
				fixed (byte* data = sound.data)
				{
					int i2 = dataOffset;
					e.Data.Buffers[0].DataByteSize = readSize;
					for (int i = 0; i != readSize; ++i)
					{
						int value = 0;
						short* valueData = (short*)&value;
						valueData[1] = (short)((data[i2]*0.00392156862745f) * short.MaxValue * volume);// '* 0.00392156862745f' = '/ 255f'
						#if iOS
						channel1[i] = value;// / 128;
						#else
						channel1[i] = value;
						#endif
						
						++i2;
					}
				}
			}
			
			dataOffset += readSize;
			if (readSize != dataFrameSize)
			{
				dataOffset = 0;
				if (!looped) Stop();
			}
		}
		
		private void render1Channel16BitCallback(object sender, AudioUnitEventArgs e)
		{
			int dataFrameSize = e.NumberFrames;
			int readSize = dataFrameSize;
			int nextOffset = readSize + dataOffset;
			int dataSize = sound.data.Length >> 1;
			if (nextOffset > dataSize) readSize -= nextOffset - dataSize;
			
			unsafe
			{
				float* channel1 = (float*)e.Data.Buffers[0].Data.ToPointer();
				fixed (byte* data = sound.data)
				{
					short* dataS = (short*)data;
					int i2 = dataOffset;
					e.Data.Buffers[0].DataByteSize = readSize * 2;
					for (int i = 0; i != readSize; ++i)
					{
						float value = 0;
						short* valueData = (short*)&value;
						valueData[1] = (short)(dataS[i2] * volume);
						#if iOS
						channel1[i] = value;// / 128;
						#else
						channel1[i] = value;
						#endif
						
						++i2;
					}
				}
			}
			
			dataOffset += readSize;
			if (readSize != dataFrameSize)
			{
				dataOffset = 0;
				if (!looped) Stop();
			}
		}
		
		private void render2Channel8BitCallback(object sender, AudioUnitEventArgs e)
		{
			int dataFrameSize = e.NumberFrames * 2;
			int readSize = dataFrameSize;
			int nextOffset = readSize + dataOffset;
			if (nextOffset > sound.data.Length) readSize -= nextOffset - sound.data.Length;
			
			unsafe
			{
				int* channel1 = (int*)e.Data.Buffers[0].Data.ToPointer();
				int* channel2 = (int*)e.Data.Buffers[1].Data.ToPointer();
				fixed (byte* data = sound.data)
				{
					int i2 = dataOffset, readSizeLoop = readSize / 2;
					e.Data.Buffers[0].DataByteSize = readSizeLoop;
					e.Data.Buffers[1].DataByteSize = readSizeLoop;
					for (int i = 0; i != readSizeLoop; ++i)
					{
						int valueL = 0, valueR = 0;
						short* valueDataL = (short*)&valueL;
						short* valueDataR = (short*)&valueR;
						valueDataL[1] = (short)((data[i2] * 0.00392156862745f) * short.MaxValue * volume);// '* 0.00392156862745f' = '/ 255f'
						valueDataR[1] = (short)((data[i2+1] * 0.00392156862745f) * short.MaxValue * volume);
						#if iOS
						channel1[i] = valueL;// / 128;
						channel2[i] = valueR;// / 128;
						#else
						channel1[i] = valueL;
						channel2[i] = valueR;
						#endif
						
						i2 += 2;
					}
				}
			}
			
			dataOffset += readSize;
			if (readSize != dataFrameSize)
			{
				dataOffset = 0;
				if (!looped) Stop();
			}
		}
		
		private void render2Channel16BitCallback(object sender, AudioUnitEventArgs e)
		{
			int dataFrameSize = e.NumberFrames * 2;
			int readSize = dataFrameSize;
			int nextOffset = readSize + dataOffset;
			int dataSize = sound.data.Length >> 1;
			if (nextOffset > dataSize) readSize -= nextOffset - dataSize;
			
			unsafe
			{
				int* channel1 = (int*)e.Data.Buffers[0].Data.ToPointer();
				int* channel2 = (int*)e.Data.Buffers[1].Data.ToPointer();
				fixed (byte* data = sound.data)
				{
					short* dataS = (short*)data;
					int i2 = dataOffset, readSizeLoop = readSize / 2;
					e.Data.Buffers[0].DataByteSize = readSize;
					e.Data.Buffers[1].DataByteSize = readSize;
					for (int i = 0; i != readSizeLoop; ++i)
					{
						int valueL = 0, valueR = 0;
						short* valueDataL = (short*)&valueL;
						short* valueDataR = (short*)&valueR;
						valueDataL[1] = (short)(dataS[i2] * volume);
						valueDataR[1] = (short)(dataS[i2+1] * volume);
						#if iOS
						channel1[i] = valueL;// / 128;
						channel2[i] = valueR;// / 128;
						#else
						channel1[i] = valueL;
						channel2[i] = valueR;
						#endif
						
						i2 += 2;
					}
				}
			}
			
			dataOffset += readSize;
			if (readSize != dataFrameSize)
			{
				dataOffset = 0;
				if (!looped) Stop();
			}
		}
		
		public void Update()
		{
			
		}
		
		#if iOS
		private void resume()
		{
			if (State == SoundStates.Paused) Play();
		}
		#endif

		public void Play()
		{
			lock (this)
			{
				volume = 1;
				instance.Start();
				State = SoundStates.Playing;
			}
		}
		
		public void Play(float volume)
		{
			lock (this)
			{
				this.volume = volume;
				instance.Start();
				State = SoundStates.Playing;
			}
		}

		public void Pause()
		{
			lock (this)
			{
				instance.Stop();
				State = SoundStates.Paused;
			}
		}

		public void Stop()
		{
			lock (this)
			{
				instance.Stop();
				State = SoundStates.Stopped;
				dataOffset = 0;
			}
		}
		#endregion
	}

	public class SoundWAV : SoundWAVI
	{
		#region Properties
		internal Audio audio;
		internal AudioStreamBasicDescription desc;
		internal new byte[] data;
		internal new short channels, bitDepth;
		#endregion

		#region Constructors
		public SoundWAV(DisposableI parent, string fileName, int instanceCount, bool looped)
		: base(parent, fileName)
		{
			audio = parent.FindParentOrSelfWithException<Audio>();
			audio.UpdateCallback += Update;
			this.data = base.data;
			this.channels = base.channels;
			this.bitDepth = base.bitDepth;
			
			desc = AudioUnitUtils.AUCanonicalASBD(sampleRate, channels);
			desc.FormatFlags = (AudioFormatFlags)((int)AudioFormatFlags.IsSignedInteger | (int)AudioFormatFlags.IsPacked | (int)AudioFormatFlags.IsNonInterleaved);

			for (int i = 0; i != instanceCount; ++i)
			{
				inactiveInstances.AddLast(new SoundWAVInstance(this, looped));
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (audio != null) audio.UpdateCallback -= Update;
			base.Dispose();
		}
		#endregion
	}
}
