using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class ViewPortAPI
	{
		public static IViewPort New(IDisposableResource parent, Point2 location, Size2 size)
		{
			return New(VideoAPI.DefaultAPI, parent, location, size);
		}

		public static IViewPort New(VideoTypes videoType, IDisposableResource parent, Point2 location, Size2 size)
		{
			IViewPort api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.ViewPort(parent, location, size);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.ViewPort(parent, location, size);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.ViewPort(parent, location, size);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.ViewPort(parent, location, size);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.ViewPort(parent, location, size);
			#endif

			if (api == null) Debug.ThrowError("ViewPortAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
