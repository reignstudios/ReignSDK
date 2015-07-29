using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class VertexBufferAPI
	{
		public static IVertexBuffer New(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices)
		{
			return New(VideoAPI.DefaultAPI, parent, bufferLayoutDesc, usage, topology, vertices, null);
		}

		public static IVertexBuffer New(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices, int[] indices)
		{
			return New(VideoAPI.DefaultAPI, parent, bufferLayoutDesc, usage, topology, vertices, indices);
		}

		public static IVertexBuffer New(VideoTypes videoType, IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices, int[] indices)
		{
			IVertexBuffer api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.VertexBuffer(parent, bufferLayoutDesc, usage, topology, vertices, indices);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.VertexBuffer(parent, bufferLayoutDesc, usage, topology, vertices, indices);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.VertexBuffer(parent, bufferLayoutDesc, usage, topology, vertices, indices);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.VertexBuffer(parent, bufferLayoutDesc, usage, topology, vertices, indices);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.VertexBuffer(parent, bufferLayoutDesc, usage, topology, vertices, indices);
			#endif

			if (api == null) Debug.ThrowError("VertexBufferAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
