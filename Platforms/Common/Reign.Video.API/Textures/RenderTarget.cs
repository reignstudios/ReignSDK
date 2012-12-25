using Reign.Core;

namespace Reign.Video.API
{
	public static class RenderTarget
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) RenderTargetAPI.Init(Reign.Video.D3D11.RenderTarget.New, Reign.Video.D3D11.RenderTarget.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) RenderTargetAPI.Init(Reign.Video.OpenGL.RenderTarget.New, Reign.Video.OpenGL.RenderTarget.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) RenderTargetAPI.Init(Reign.Video.XNA.RenderTarget.New, Reign.Video.XNA.RenderTarget.New);
			#endif
		}
	}
}
