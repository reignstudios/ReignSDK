using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class IndexBufferAPI
	{
		public static IIndexBuffer New(IDisposableResource parent, BufferUsages usage, int[] indices)
		{
			return New(VideoAPI.DefaultAPI, parent, usage, indices);
		}

		public static IIndexBuffer New(VideoTypes videoType, IDisposableResource parent, BufferUsages usage, int[] indices)
		{
			IIndexBuffer api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.IndexBuffer(parent, usage, indices);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.IndexBuffer(parent, usage, indices);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.IndexBuffer(parent, usage, indices);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.IndexBuffer(parent, usage, indices);
			#endif

			if (api == null) Debug.ThrowError("IndexBufferAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
