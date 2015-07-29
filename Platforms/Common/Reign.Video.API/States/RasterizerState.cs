using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class RasterizerStateDescAPI
	{
		public static IRasterizerStateDesc New(VideoTypes videoType, RasterizerStateTypes type)
		{
			IRasterizerStateDesc api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.RasterizerStateDesc(type);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.RasterizerStateDesc(type);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.RasterizerStateDesc(type);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.RasterizerStateDesc(type);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.RasterizerStateDesc(type);
			#endif

			if (api == null) Debug.ThrowError("RasterizerStateDescAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}

	public static class RasterizerStateAPI
	{
		public static IRasterizerState New(VideoTypes videoType, IDisposableResource parent, IRasterizerStateDesc desc)
		{
			IRasterizerState api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.RasterizerState(parent, desc);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.RasterizerState(parent, desc);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.RasterizerState(parent, desc);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.RasterizerState(parent, desc);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.RasterizerState(parent, desc);
			#endif

			if (api == null) Debug.ThrowError("RasterizerStateAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
