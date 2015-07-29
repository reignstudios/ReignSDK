using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.Abstraction
{
	public static class BufferLayoutDescAPI
	{
		public static IBufferLayoutDesc New(BufferLayoutTypes type)
		{
			return New(VideoAPI.DefaultAPI, type);
		}

		public static IBufferLayoutDesc New(VideoTypes videoType, BufferLayoutTypes type)
		{
			IBufferLayoutDesc api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.BufferLayoutDesc(type);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.BufferLayoutDesc(type);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.BufferLayoutDesc(type);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.BufferLayoutDesc(type);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.BufferLayoutDesc(type);
			#endif

			if (api == null) Debug.ThrowError("BufferLayoutDescAPI", "Unsuported InputType: " + videoType);
			return api;
		}

		public static IBufferLayoutDesc New(List<BufferLayoutElement> elements)
		{
			return New(VideoAPI.DefaultAPI, elements);
		}

		public static IBufferLayoutDesc New(VideoTypes videoType, List<BufferLayoutElement> elements)
		{
			IBufferLayoutDesc api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.BufferLayoutDesc(elements);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.BufferLayoutDesc(elements);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.BufferLayoutDesc(elements);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.BufferLayoutDesc(elements);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.BufferLayoutDesc(elements);
			#endif

			if (api == null) Debug.ThrowError("BufferLayoutDescAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}

	public static class BufferLayoutAPI
	{
		public static IBufferLayout New(IDisposableResource parent, IShader shader, IBufferLayoutDesc desc)
		{
			return New(VideoAPI.DefaultAPI, parent, shader, desc);
		}

		public static IBufferLayout New(VideoTypes videoType, IDisposableResource parent, IShader shader, IBufferLayoutDesc desc)
		{
			IBufferLayout api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.BufferLayout(parent, shader, desc);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.BufferLayout(parent, shader, desc);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.BufferLayout(parent, shader, desc);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.BufferLayout(parent, shader, desc);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.BufferLayout(parent, shader, desc);
			#endif

			if (api == null) Debug.ThrowError("BufferLayoutAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
