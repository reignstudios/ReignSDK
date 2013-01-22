using Reign.Core;

namespace Reign.Video.API
{
	public static class SamplerStateDesc
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) SamplerStateDescAPI.Init(Reign.Video.D3D9.SamplerStateDesc.New);
			#endif

			#if WINDOWS || METRO || WP8
			if (type == VideoTypes.D3D11) SamplerStateDescAPI.Init(Reign.Video.D3D11.SamplerStateDesc.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) SamplerStateDescAPI.Init(Reign.Video.OpenGL.SamplerStateDesc.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) SamplerStateDescAPI.Init(Reign.Video.XNA.SamplerStateDesc.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) SamplerStateDescAPI.Init(Reign.Video.Vita.SamplerStateDesc.New);
			#endif
		}
	}

	public static class SamplerState
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) SamplerStateAPI.Init(Reign.Video.D3D9.SamplerState.New);
			#endif

			#if WINDOWS || METRO || WP8
			if (type == VideoTypes.D3D11) SamplerStateAPI.Init(Reign.Video.D3D11.SamplerState.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) SamplerStateAPI.Init(Reign.Video.OpenGL.SamplerState.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) SamplerStateAPI.Init(Reign.Video.XNA.SamplerState.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) SamplerStateAPI.Init(Reign.Video.Vita.SamplerState.New);
			#endif
		}
	}
}
