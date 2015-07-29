using System;
using Reign.Core;

namespace Reign.Audio.Abstraction
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

	public static class AudioAPI
	{
		public static AudioTypes DefaultAPI = AudioTypes.None;

		public static IAudio New(AudioTypes audioTypeFlags, out AudioTypes audioType, IDisposableResource parent)
		{
			bool dumby = (audioTypeFlags & AudioTypes.Dumby) != 0;
			bool xAudio = (audioTypeFlags & AudioTypes.XAudio) != 0;
			bool xna = (audioTypeFlags & AudioTypes.XNA) != 0;
			bool cocoa = (audioTypeFlags & AudioTypes.Cocoa) != 0;
			bool openAL = (audioTypeFlags & AudioTypes.OpenAL) != 0;
			bool android = (audioTypeFlags & AudioTypes.Android) != 0;
			bool nacl = (audioTypeFlags & AudioTypes.NaCl) != 0;

			audioType = AudioTypes.None;
			Exception lastException = null;
			IAudio audio = null;
			while (true)
			{
				try
				{
					#if WIN32 || WINRT
					if (xAudio)
					{
						xAudio = false;
						audioType = AudioTypes.XAudio;
						audio = new Reign.Audio.XAudio.Audio(parent);
						break;
					}
					else
					#endif

					#if XNA
					if (xna)
					{
						xna = false;
						audioType = AudioTypes.XNA;
						audio = new Reign.Audio.XNA.Audio(parent);
						break;
					}
					else
					#endif
					
					#if OSX || iOS
					if (cocoa)
					{
						cocoa = false;
						audioType = AudioTypes.Cocoa;
						audio = new Reign.Audio.Cocoa.Audio(parent);
						break;
					}
					else
					#endif
					
					#if LINUX
					if (openAL)
					{
						openAL = false;
						audioType = AudioTypes.OpenAL;
						audio = new Reign.Audio.OpenAL.Audio(parent);
						break;
					}
					else
					#endif
					
					#if ANDROID
					if (android)
					{
						android = false;
						audioType = AudioTypes.Android;
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
						audioType = AudioTypes.Dumby;
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
				Debug.ThrowError("AudioAPI", "Failed to create Audio API" + ex);
				audioType = AudioTypes.None;
			}

			if (audioType != AudioTypes.None) DefaultAPI = audioType;
			return audio;
		}
	}
}
