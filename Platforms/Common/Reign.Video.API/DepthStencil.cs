using Reign.Core;

namespace Reign.Video.API
{
	public static class DepthStencil
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) DepthStencilAPI.Init(Reign.Video.D3D9.DepthStencil.New);
			#endif

			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) DepthStencilAPI.Init(Reign.Video.D3D11.DepthStencil.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) DepthStencilAPI.Init(Reign.Video.OpenGL.DepthStencil.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) DepthStencilAPI.Init(Reign.Video.XNA.DepthStencil.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) DepthStencilAPI.Init(Reign.Video.Vita.DepthStencil.New);
			#endif
		}
	}
}
