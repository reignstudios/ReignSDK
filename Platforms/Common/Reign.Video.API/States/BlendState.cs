using Reign.Core;

namespace Reign.Video.API
{
	public static class BlendStateDesc
	{
		public static void Init(VideoTypes type)
		{
			#if WIN32
			if (type == VideoTypes.D3D9) BlendStateDescAPI.Init(Reign.Video.D3D9.BlendStateDesc.New);
			#endif

			#if WIN32 || WINRT || WP8
			if (type == VideoTypes.D3D11) BlendStateDescAPI.Init(Reign.Video.D3D11.BlendStateDesc.New);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) BlendStateDescAPI.Init(Reign.Video.OpenGL.BlendStateDesc.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) BlendStateDescAPI.Init(Reign.Video.XNA.BlendStateDesc.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) BlendStateDescAPI.Init(Reign.Video.Vita.BlendStateDesc.New);
			#endif
		}
	}

	public static class BlendState
	{
		public static void Init(VideoTypes type)
		{
			#if WIN32
			if (type == VideoTypes.D3D9) BlendStateAPI.Init(Reign.Video.D3D9.BlendState.New);
			#endif

			#if WIN32 || WINRT || WP8
			if (type == VideoTypes.D3D11) BlendStateAPI.Init(Reign.Video.D3D11.BlendState.New);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) BlendStateAPI.Init(Reign.Video.OpenGL.BlendState.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) BlendStateAPI.Init(Reign.Video.XNA.BlendState.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) BlendStateAPI.Init(Reign.Video.Vita.BlendState.New);
			#endif
		}
	}
}
