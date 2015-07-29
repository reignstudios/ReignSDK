using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class QuickDrawAPI
	{
		public static IQuickDraw New(IDisposableResource parent, IBufferLayoutDesc desc)
		{
			return New(parent, desc);
		}

		public static IQuickDraw New(VideoTypes videoType, IDisposableResource parent, IBufferLayoutDesc desc)
		{
			IQuickDraw api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.QuickDraw(parent, desc);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.QuickDraw(parent, desc);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.QuickDraw(parent, desc);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.QuickDraw(parent, desc);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.QuickDraw(parent, desc);
			#endif

			if (api == null) Debug.ThrowError("QuickDrawAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
