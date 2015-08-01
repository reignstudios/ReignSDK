using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class ShaderAPI
	{
		public static IShader New(IDisposableResource parent, string filename, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback)
		{
			return New(VideoAPI.DefaultAPI, parent, filename, shaderVersion, ShaderFloatingPointQuality.High, ShaderFloatingPointQuality.Low, loadedCallback);
		}

		public static IShader New(IDisposableResource parent, string filename, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		{
			return New(VideoAPI.DefaultAPI, parent, filename, shaderVersion, vsQuality, psQuality, loadedCallback);
		}

		public static IShader New(VideoTypes videoType, IDisposableResource parent, string filename, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		{
			IShader api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.Shader(parent, filename, shaderVersion, vsQuality, psQuality, loadedCallback);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.Shader(parent, filename, shaderVersion, vsQuality, psQuality, loadedCallback);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.Shader(parent, filename, shaderVersion, vsQuality, psQuality, loadedCallback);
            #endif

            #if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.Shader(parent, filename, shaderVersion, vsQuality, psQuality, loadedCallback);
            #endif

            #if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.Shader(parent, filename, shaderVersion, vsQuality, psQuality, loadedCallback);
            #endif

            if (api == null) Debug.ThrowError("ShaderAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
