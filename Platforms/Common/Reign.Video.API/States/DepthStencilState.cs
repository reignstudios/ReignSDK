using Reign.Core;

namespace Reign.Video.API
{
	public static class DepthStencilStateDesc
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) DepthStencilStateDescAPI.Init(Reign.Video.D3D11.DepthStencilStateDesc.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) DepthStencilStateDescAPI.Init(Reign.Video.OpenGL.DepthStencilStateDesc.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) DepthStencilStateDescAPI.Init(Reign.Video.XNA.DepthStencilStateDesc.New);
			#endif
		}
	}

	public static class DepthStencilState
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) DepthStencilStateAPI.Init(Reign.Video.D3D11.DepthStencilState.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) DepthStencilStateAPI.Init(Reign.Video.OpenGL.DepthStencilState.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) DepthStencilStateAPI.Init(Reign.Video.XNA.DepthStencilState.New);
			#endif
		}
	}
}
