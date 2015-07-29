using Reign.Core;

namespace Reign.Audio.Abstraction
{
	enum SoundFormats
	{
		None,
		WAV
	}

	public static class SoundAPI
	{
		public static ISound New(IDisposableResource parent, string filename, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		{
			return New(AudioAPI.DefaultAPI, parent, filename, instanceCount, looped, loadedCallback);
		}

		public static ISound New(AudioTypes audioType, IDisposableResource parent, string filename, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		{
			ISound api = null;

			var soundFormat = SoundFormats.None;
			string ext = Streams.GetFileExt(filename);
			switch (ext.ToLower())
			{
				case ".wav":
					soundFormat = SoundFormats.WAV;
					break;

				default:
					Debug.ThrowError("SoundAPI", string.Format("File 'ext' {0} not supported.", ext));
					return null;
			}

			#if WIN32 || WINRT
			if (soundFormat == SoundFormats.WAV)
			{
				if (audioType == AudioTypes.XAudio) api = new XAudio.SoundWAV(parent, filename, instanceCount, looped, loadedCallback);
			}
			#endif

			#if XNA
			if (soundFormat == SoundFormats.WAV)
			{
				if (audioType == AudioTypes.XNA) api = new XNA.SoundWAV(parent, filename, instanceCount, looped, loadedCallback);
			}
			#endif
			
			#if OSX || iOS
			if (soundFormat == SoundFormats.WAV)
			{
				if (audioType == AudioTypes.Cocoa) api = new Cocoa.SoundWAV(parent, filename, instanceCount, looped, loadedCallback);
			}
			#endif
			
			#if LINUX
			if (soundFormat == SoundFormats.WAV)
			{
				if (audioType == AudioTypes.OpenAL) api = new OpenAL.SoundWAV(parent, filename, instanceCount, looped, loadedCallback);
			}
			#endif
			
			#if ANDROID
			if (soundFormat == SoundFormats.WAV)
			{
				if (audioType == AudioTypes.Android) api = new Android.SoundWAV(parent, filename, instanceCount, looped, loadedCallback);
			}
			#endif

			if (audioType == AudioTypes.Dumby) api = new Dumby.SoundWAV(parent, filename, instanceCount, looped, loadedCallback);

			if (api == null) Debug.ThrowError("SoundAPI", "Unsuported InputType: " + audioType);
			return api;
		}
	}
}