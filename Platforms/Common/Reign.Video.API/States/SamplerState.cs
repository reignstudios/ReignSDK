using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class SamplerStateDescAPI
	{
		public static ISamplerStateDesc New(VideoTypes videoType, SamplerStateTypes type)
		{
			ISamplerStateDesc api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.SamplerStateDesc(type);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.SamplerStateDesc(type);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.SamplerStateDesc(type);
			#endif

			#if videoType
			if (videoType == VideoTypes.XNA) api = new XNA.SamplerStateDesc(type);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.SamplerStateDesc(type);
			#endif

			if (api == null) Debug.ThrowError("SamplerStateDescAPI", "Unsuported InputType: " + type);
			return api;
		}
	}

	public static class SamplerStateAPI
	{
		public static ISamplerState Init(VideoTypes videoType, IDisposableResource parent, ISamplerStateDesc desc)
		{
			ISamplerState api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.SamplerState(parent, desc);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.SamplerState(parent, desc);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.SamplerState(parent, desc);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.SamplerState(parent, desc);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.SamplerState(parent, desc);
			#endif

			if (api == null) Debug.ThrowError("SamplerStateAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
