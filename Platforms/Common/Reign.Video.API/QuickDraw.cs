using Reign.Core;

namespace Reign.Video.API
{
	public static class QuickDraw
	{
		public static void Init(VideoTypes type)
		{
			#if WIN32
			if (type == VideoTypes.D3D9) QuickDrawAPI.Init(Reign.Video.D3D9.QuickDraw.New);
			#endif

			#if WIN32 || WINRT || WP8
			if (type == VideoTypes.D3D11) QuickDrawAPI.Init(Reign.Video.D3D11.QuickDraw.New);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) QuickDrawAPI.Init(Reign.Video.OpenGL.QuickDraw.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) QuickDrawAPI.Init(Reign.Video.XNA.QuickDraw.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) QuickDrawAPI.Init(Reign.Video.Vita.QuickDraw.New);
			#endif
		}
	}
}
