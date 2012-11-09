using System;
using Reign.Core;
using Reign.Audio;
using System.Reflection;

namespace Reign.Audio.API
{
	public enum AudioTypes
	{
		None,
		Dumby,
		XAudio,
		XNA,
		Cocoa,
		OpenAL,
		Android
	}

	public static class Audio
	{
		internal const string Dumby = "Reign.Audio.Dumby";
		internal const string XAudio = "Reign.Audio.XAudio";
		internal const string XNA = "Reign.Audio.XNA";
		internal const string Cocoa = "Reign.Audio.Cocoa";
		internal const string OpenAL = "Reign.Audio.OpenAL";
		internal const string Android = "Reign.Audio.Android";

		public static AudioI Create(AudioTypes typeFlags, out AudioTypes type, params object[] args)
		{
			bool dumby = (typeFlags & AudioTypes.Dumby) != 0;
			#if WINDOWS || METRO
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

			Exception lastException = null;
			while (true)
			{
				try
				{
					#if WINDOWS || METRO
					if (xAudio)
					{
						xAudio = false;
						type = AudioTypes.XAudio;
						return (AudioI)OS.CreateInstance(typeof(Reign.Audio.XAudio.Audio), args);
					}
					else
					#endif

					#if XNA
					if (xna)
					{
						xna = false;
						type = AudioTypes.XNA;
						return (AudioI)OS.CreateInstance(typeof(Reign.Audio.XNA.Audio), args);
					}
					else
					#endif
					
					#if OSX || iOS
					if (cocoa)
					{
						cocoa = false;
						type = AudioTypes.Cocoa;
						return (AudioI)OS.CreateInstance(typeof(Reign.Audio.Cocoa.Audio), args);
					}
					else
					#endif
					
					#if LINUX
					if (openAL)
					{
						openAL = false;
						type = AudioTypes.OpenAL;
						return (AudioI)OS.CreateInstance(typeof(Reign.Audio.OpenAL.Audio), args);
					}
					else
					#endif
					
					#if ANDROID
					if (android)
					{
						android = false;
						type = AudioTypes.Android;
						return (AudioI)OS.CreateInstance(typeof(Reign.Audio.Android.Audio), args);
					}
					else
					#endif

					if (dumby)
					{
						dumby = false;
						type = AudioTypes.Dumby;
						return (AudioI)OS.CreateInstance(typeof(Reign.Audio.Dumby.Audio), args);
					}

					else break;
				}
				catch (TargetInvocationException e)
				{
					if (e.InnerException != null) lastException = e.InnerException;
					else lastException = e;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			string ex = lastException == null ? "" : " - Exception: " + lastException.Message;
			Debug.ThrowError("Audio", "Failed to create Audio API" + ex);
			type = AudioTypes.None;
			return null;
		}
	}
}
