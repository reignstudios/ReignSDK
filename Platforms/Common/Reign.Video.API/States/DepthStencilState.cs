using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class DepthStencilStateDescAPI
	{
		public static IDepthStencilStateDesc New(VideoTypes videoType, DepthStencilStateTypes type)
		{
			IDepthStencilStateDesc api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.DepthStencilStateDesc(type);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.DepthStencilStateDesc(type);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.DepthStencilStateDesc(type);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.DepthStencilStateDesc(type);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.DepthStencilStateDesc(type);
			#endif

			if (api == null) Debug.ThrowError("DepthStencilStateDescAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}

	public static class DepthStencilStateAPI
	{
		public static IDepthStencilState New(VideoTypes videoType, IDisposableResource parent, IDepthStencilStateDesc desc)
		{
			IDepthStencilState api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.DepthStencilState(parent, desc);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.DepthStencilState(parent, desc);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.DepthStencilState(parent, desc);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.DepthStencilState(parent, desc);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.DepthStencilState(parent, desc);
			#endif

			if (api == null) Debug.ThrowError("DepthStencilStateAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
