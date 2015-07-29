using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class BlendStateDescAPI
	{
		public static IBlendStateDesc New(VideoTypes videoType, BlendStateTypes type)
		{
			IBlendStateDesc api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.BlendStateDesc(type);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.BlendStateDesc(type);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.BlendStateDesc(type);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.BlendStateDesc(type);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.BlendStateDesc(type);
			#endif

			if (api == null) Debug.ThrowError("BlendStateDescAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}

	public static class BlendStateAPI
	{
		public static IBlendState New(VideoTypes videoType, IDisposableResource parent, IBlendStateDesc desc)
		{
			IBlendState api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.BlendState(parent, desc);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.BlendState(parent, desc);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.BlendState(parent, desc);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.BlendState(parent, desc);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.BlendState(parent, desc);
			#endif

			if (api == null) Debug.ThrowError("BlendStateAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
