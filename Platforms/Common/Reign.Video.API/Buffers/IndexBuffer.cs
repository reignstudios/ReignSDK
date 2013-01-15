using Reign.Core;

namespace Reign.Video.API
{
	public static class IndexBuffer
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) IndexBufferAPI.Init(Reign.Video.D3D9.IndexBuffer.New, Reign.Video.D3D9.IndexBuffer.New);
			#endif

			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) IndexBufferAPI.Init(Reign.Video.D3D11.IndexBuffer.New, Reign.Video.D3D11.IndexBuffer.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) IndexBufferAPI.Init(Reign.Video.OpenGL.IndexBuffer.New, Reign.Video.OpenGL.IndexBuffer.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) IndexBufferAPI.Init(Reign.Video.XNA.IndexBuffer.New, Reign.Video.XNA.IndexBuffer.New);
			#endif
		}
	}
}
