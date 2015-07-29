using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class DepthStencilAPI
	{
		public static IDepthStencil New(IDisposableResource parent, int width, int height, DepthStencilFormats format)
		{
			return New(VideoAPI.DefaultAPI, parent, width, height, format);
		}

		public static IDepthStencil New(VideoTypes videoType, IDisposableResource parent, int width, int height, DepthStencilFormats format)
		{
			IDepthStencil api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.DepthStencil(parent, width, height, format);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.DepthStencil(parent, width, height, format);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.DepthStencil(parent, width, height, format);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.DepthStencil(parent, width, height, format);
			#endif

			if (api == null) Debug.ThrowError("DepthStencilAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
