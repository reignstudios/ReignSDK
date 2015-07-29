using Reign.Core;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace Reign.Video
{
	public enum VertexBufferTopologys
	{
		None,
		Point,
		Line,
		Triangle
	}

	public abstract class IVertexBuffer : DisposableResource
	{
		#region Properties
		protected BufferUsages usage;
		protected int vertexByteSize, vertexFloatArraySize, vertexCount;
		public int VertexCount {get{return vertexCount;}}
		public int VertexByteSize {get{return vertexByteSize;}}
		public int VertexFloatArraySize {get{return vertexFloatArraySize;}}
		public abstract VertexBufferTopologys Topology {get; set;}
		public abstract IIndexBuffer IndexBuffer {get;}
		#endregion

		#region Constructors
		protected IVertexBuffer(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages usage, float[] vertices)
		: base(parent)
		{
			this.usage = usage;
			vertexByteSize = bufferLayoutDesc.ByteSize;
			vertexFloatArraySize = bufferLayoutDesc.FloatCount;
			vertexCount = vertices.Length / vertexFloatArraySize;
		}
		#endregion

		#region Methods
		// TODO: Implement contructors for these and normal Vector types
		/*public void Init(List<float[]> vertices)
		{
			float[] verts = new float[vertices.Count * vertexFloatArraySize];
			int vertSeg = 0;
			foreach (var vert in vertices)
			{
				if (vertexFloatArraySize != vert.Length)
				{
					Debug.ThrowError("IVertexBuffer", "A Vertex size does not match the buffer layout");
				}

				foreach (var seg in vert)
				{
					verts[vertSeg] = seg;
					++vertSeg;
				}
			}

			Init(verts);
		}

		public void Init(List<object> vertices)
		{
			#if !XNA
			float[] verts = new float[vertices.Count * vertexFloatArraySize];
			int vertSeg = 0;
			foreach (var vertex in vertices)
			{
				int size = Marshal.SizeOf(vertex);
				int size4 = size / 4;
				float[] newVertex = new float[size4];
				IntPtr buffer = Marshal.AllocHGlobal(size);
				Marshal.StructureToPtr(vertex, buffer, false);
				Marshal.Copy(buffer, newVertex, 0, size4);
				Marshal.FreeHGlobal(buffer);

				if (vertexFloatArraySize != newVertex.Length)
				{
					Debug.ThrowError("IVertexBuffer", "A Vertex size does not match the buffer layout");
				}

				for (int i = 0; i != newVertex.Length; ++i)
				{
					verts[vertSeg] = newVertex[i];
					++vertSeg;
				}
			}

			Init(verts);
			#else
			Debug.ThrowError("IVertexBuffer", "Does not work on Xbox360.");
			#endif
		}*/

		public abstract void Update(float[] vertices, int updateCount);
		public abstract void Enable();
		public abstract void Enable(IIndexBuffer indexBuffer);
		public abstract void Enable(IVertexBuffer instanceBuffer);
		public abstract void Enable(IIndexBuffer indexBuffer, IVertexBuffer instanceBuffer);
		public abstract void Draw();
		public abstract void Draw(int drawCount);
		public abstract void DrawInstanced(int drawCount);
		public abstract void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount);
		#endregion
	}
}