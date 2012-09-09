using System;
using Reign.Core;
using Reign.Audio;
using System.Reflection;

namespace Reign.Audio.API
{
	public static class Sound
	{
		public static SoundI Create(AudioTypes apiType, params object[] args)
		{
			try
			{
				if (apiType == AudioTypes.Dumby)
				{
					return (SoundI)OS.InvokeStaticMethod(Audio.Dumby, Audio.Dumby, "Sound", "Load", args);
				}

				#if WINDOWS || METRO
				if (apiType == AudioTypes.XAudio)
				{
					return (SoundI)OS.InvokeStaticMethod(Audio.XAudio, Audio.XAudio, "Sound", "Load", args);
				}
				#endif

				#if XNA
				if (apiType == AudioTypes.XNA)
				{
					return (SoundI)OS.InvokeStaticMethod(Audio.XNA, Audio.XNA, "Sound", "Load", args);
				}
				#endif
				
				#if OSX || iOS
				if (apiType == AudioTypes.Cocoa)
				{
					return (SoundI)OS.InvokeStaticMethod(Audio.Cocoa, Audio.Cocoa, "Sound", "Load", args);
				}
				#endif
				
				#if LINUX
				if (apiType == AudioTypes.OpenAL)
				{
					return (SoundI)OS.InvokeStaticMethod(Audio.OpenAL, Audio.OpenAL, "Sound", "Load", args);
				}
				#endif
				
				#if ANDROID
				if (apiType == AudioTypes.Android)
				{
					return (SoundI)OS.InvokeStaticMethod(Audio.Android, Audio.Android, "Sound", "Load", args);
				}
				#endif
			}
			catch (TargetInvocationException e)
			{
				throw (e.InnerException != null) ? e.InnerException : e;
			}
			catch (Exception e)
			{
				throw e;
			}

			return null;
		}
	}
}
