using Reign.Core;

namespace Reign.Video.API
{
	public static class BufferLayoutDesc
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) BufferLayoutDescAPI.Init(Reign.Video.D3D11.BufferLayoutDesc.New, Reign.Video.D3D11.BufferLayoutDesc.New);
			#endif

			#if WINDOWS || OSX || LINUX
			if (type == VideoTypes.OpenGL) BufferLayoutDescAPI.Init(Reign.Video.OpenGL.BufferLayoutDesc.New, Reign.Video.OpenGL.BufferLayoutDesc.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) BufferLayoutDescAPI.Init(Reign.Video.XNA.BufferLayoutDesc.New, Reign.Video.XNA.BufferLayoutDesc.New);
			#endif
		}
	}

	public static class BufferLayout
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) BufferLayoutAPI.Init(Reign.Video.D3D11.BufferLayout.New);
			#endif

			#if WINDOWS || OSX || LINUX
			if (type == VideoTypes.OpenGL) BufferLayoutAPI.Init(Reign.Video.OpenGL.BufferLayout.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) BufferLayoutAPI.Init(Reign.Video.XNA.BufferLayout.New);
			#endif
		}
	}
}
