﻿using Reign.Core;

namespace Reign.Video.API
{
	public static class VertexBuffer
	{
		public static void Init(VideoTypes type)
		{
			#if WIN32
			if (type == VideoTypes.D3D9) VertexBufferAPI.Init(Reign.Video.D3D9.VertexBuffer.New, Reign.Video.D3D9.VertexBuffer.New);
			#endif

			#if WIN32 || WINRT || WP8
			if (type == VideoTypes.D3D11) VertexBufferAPI.Init(Reign.Video.D3D11.VertexBuffer.New, Reign.Video.D3D11.VertexBuffer.New);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) VertexBufferAPI.Init(Reign.Video.OpenGL.VertexBuffer.New, Reign.Video.OpenGL.VertexBuffer.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) VertexBufferAPI.Init(Reign.Video.XNA.VertexBuffer.New, Reign.Video.XNA.VertexBuffer.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) VertexBufferAPI.Init(Reign.Video.Vita.VertexBuffer.New, Reign.Video.Vita.VertexBuffer.New);
			#endif
		}
	}
}
