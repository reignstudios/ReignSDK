using Reign.Core;

namespace Reign.Video.API
{
	public static class ViewPort
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) ViewPortAPI.Init(Reign.Video.D3D9.ViewPort.New, Reign.Video.D3D9.ViewPort.New);
			#endif

			#if WINDOWS || METRO || WP8
			if (type == VideoTypes.D3D11) ViewPortAPI.Init(Reign.Video.D3D11.ViewPort.New, Reign.Video.D3D11.ViewPort.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) ViewPortAPI.Init(Reign.Video.OpenGL.ViewPort.New, Reign.Video.OpenGL.ViewPort.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) ViewPortAPI.Init(Reign.Video.XNA.ViewPort.New, Reign.Video.XNA.ViewPort.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) ViewPortAPI.Init(Reign.Video.Vita.ViewPort.New, Reign.Video.Vita.ViewPort.New);
			#endif
		}
	}
}
