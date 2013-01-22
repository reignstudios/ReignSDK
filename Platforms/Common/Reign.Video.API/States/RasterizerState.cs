using Reign.Core;

namespace Reign.Video.API
{
	public static class RasterizerStateDesc
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) RasterizerStateDescAPI.Init(Reign.Video.D3D9.RasterizerStateDesc.New);
			#endif

			#if WINDOWS || METRO || WP8
			if (type == VideoTypes.D3D11) RasterizerStateDescAPI.Init(Reign.Video.D3D11.RasterizerStateDesc.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) RasterizerStateDescAPI.Init(Reign.Video.OpenGL.RasterizerStateDesc.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) RasterizerStateDescAPI.Init(Reign.Video.XNA.RasterizerStateDesc.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) RasterizerStateDescAPI.Init(Reign.Video.Vita.RasterizerStateDesc.New);
			#endif
		}
	}

	public static class RasterizerState
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) RasterizerStateAPI.Init(Reign.Video.D3D9.RasterizerState.New);
			#endif

			#if WINDOWS || METRO || WP8
			if (type == VideoTypes.D3D11) RasterizerStateAPI.Init(Reign.Video.D3D11.RasterizerState.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) RasterizerStateAPI.Init(Reign.Video.OpenGL.RasterizerState.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) RasterizerStateAPI.Init(Reign.Video.XNA.RasterizerState.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) RasterizerStateAPI.Init(Reign.Video.Vita.RasterizerState.New);
			#endif
		}
	}
}
