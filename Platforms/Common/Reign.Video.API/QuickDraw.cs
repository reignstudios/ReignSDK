using Reign.Core;

namespace Reign.Video.API
{
	public static class QuickDraw
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) QuickDrawAPI.Init(Reign.Video.D3D11.QuickDraw.New);
			#endif

			#if WINDOWS || OSX || LINUX
			if (type == VideoTypes.OpenGL) QuickDrawAPI.Init(Reign.Video.OpenGL.QuickDraw.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) QuickDrawAPI.Init(Reign.Video.XNA.QuickDraw.New);
			#endif
		}
	}
}
