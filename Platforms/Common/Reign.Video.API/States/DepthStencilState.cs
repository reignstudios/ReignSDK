using Reign.Core;

namespace Reign.Video.API
{
	public static class DepthStencilStateDesc
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) DepthStencilStateDescAPI.Init(Reign.Video.D3D9.DepthStencilStateDesc.New);
			#endif

			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) DepthStencilStateDescAPI.Init(Reign.Video.D3D11.DepthStencilStateDesc.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) DepthStencilStateDescAPI.Init(Reign.Video.OpenGL.DepthStencilStateDesc.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) DepthStencilStateDescAPI.Init(Reign.Video.XNA.DepthStencilStateDesc.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) DepthStencilStateDescAPI.Init(Reign.Video.Vita.DepthStencilStateDesc.New);
			#endif
		}
	}

	public static class DepthStencilState
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) DepthStencilStateAPI.Init(Reign.Video.D3D9.DepthStencilState.New);
			#endif

			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) DepthStencilStateAPI.Init(Reign.Video.D3D11.DepthStencilState.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) DepthStencilStateAPI.Init(Reign.Video.OpenGL.DepthStencilState.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) DepthStencilStateAPI.Init(Reign.Video.XNA.DepthStencilState.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) DepthStencilStateAPI.Init(Reign.Video.Vita.DepthStencilState.New);
			#endif
		}
	}
}
