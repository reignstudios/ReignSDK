using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class Texture2DAPI
	{
		public static ITexture2D New(IDisposableResource parent, string filename, Loader.LoadedCallbackMethod loadedCallback)
		{
			return New(parent, filename, true, BufferUsages.Default, loadedCallback);
		}

		public static ITexture2D New(IDisposableResource parent, string filename, bool generateMipmaps, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return New(parent, filename, generateMipmaps, usage, loadedCallback);
		}
	
		public static ITexture2D New(VideoTypes videoType, IDisposableResource parent, string filename, bool generateMipmaps, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		{
			ITexture2D api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.Texture2D(parent, filename, generateMipmaps, usage, loadedCallback);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.Texture2D(parent, filename, generateMipmaps, usage, loadedCallback);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.Texture2D(parent, filename, generateMipmaps, usage, loadedCallback);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.Texture2D(parent, filename, generateMipmaps, usage, loadedCallback);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.Texture2D(parent, filename, generateMipmaps, usage, loadedCallback);
			#endif

			if (api == null) Debug.ThrowError("Texture2DAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
