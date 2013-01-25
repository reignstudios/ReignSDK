using System;
using Reign.Core;
using Reign.Audio;

namespace Reign.Audio.API
{
	[Flags]
	public enum AudioTypes
	{
		None = 0,
		Dumby = 1,
		XAudio = 2,
		XNA = 4,
		Cocoa = 8,
		OpenAL = 16,
		Android = 32,
		NaCl = 64
	}

	public static class Audio
	{
		public static AudioI Init(AudioTypes typeFlags, out AudioTypes type, DisposableI parent)
		{
			bool dumby = (typeFlags & AudioTypes.Dumby) != 0;

			#if WIN32 || WINRT
			bool xAudio = (typeFlags & AudioTypes.XAudio) != 0;
			#endif

			#if XNA
			bool xna = (typeFlags & AudioTypes.XNA) != 0;
			#endif

			#if OSX || iOS
			bool cocoa = (typeFlags & AudioTypes.Cocoa) != 0;
			#endif

			#if LINUX
			bool openAL = (typeFlags & AudioTypes.OpenAL) != 0;
			#endif

			#if ANDROID
			bool android = (typeFlags & AudioTypes.Android) != 0;
			#endif
			
			#if NaCl
			bool nacl = (typeFlags & AudioTypes.NaCl) != 0;
			#endif

			type = AudioTypes.None;
			Exception lastException = null;
			AudioI audio = null;
			while (true)
			{
				try
				{
					#if WIN32 || WINRT
					if (xAudio)
					{
						xAudio = false;
						type = AudioTypes.XAudio;
						audio = new Reign.Audio.XAudio.Audio(parent);
						break;
					}
					else
					#endif

					#if XNA
					if (xna)
					{
						xna = false;
						type = AudioTypes.XNA;
						audio = new Reign.Audio.XNA.Audio(parent);
						break;
					}
					else
					#endif
					
					#if OSX || iOS
					if (cocoa)
					{
						cocoa = false;
						type = AudioTypes.Cocoa;
						audio = new Reign.Audio.Cocoa.Audio(parent);
						break;
					}
					else
					#endif
					
					#if LINUX
					if (openAL)
					{
						openAL = false;
						type = AudioTypes.OpenAL;
						audio = new Reign.Audio.OpenAL.Audio(parent);
						break;
					}
					else
					#endif
					
					#if ANDROID
					if (android)
					{
						android = false;
						type = AudioTypes.Android;
						audio = new Reign.Audio.Android.Audio(parent);
						break;
					}
					else
					#endif
					
					#if NaCl
					if (nacl)
					{
						throw new NotImplementedException();
					}
					else
					#endif

					if (dumby)
					{
						dumby = false;
						type = AudioTypes.Dumby;
						audio = new Reign.Audio.Dumby.Audio(parent);
						break;
					}

					else break;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			// check for error
			if (lastException != null)
			{
				string ex = lastException == null ? "" : " - Exception: " + lastException.Message;
				Debug.ThrowError("Audio", "Failed to create Audio API" + ex);
				type = AudioTypes.None;
			}

			// init api methods
			Sound.Init(type);
			Music.Init(type);

			return audio;
		}
	}
}
