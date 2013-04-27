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

	public abstract class VertexBufferI : Disposable
	{
		#region Properties
		protected BufferUsages usage;
		protected int vertexByteSize, vertexFloatArraySize, vertexCount;
		public int VertexCount {get{return vertexCount;}}
		public int VertexByteSize {get{return vertexByteSize;}}
		public int VertexFloatArraySize {get{return vertexFloatArraySize;}}
		public abstract VertexBufferTopologys Topology {get; set;}
		public abstract IndexBufferI IndexBuffer {get;}
		#endregion

		#region Constructors
		protected VertexBufferI(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, float[] vertices)
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
					Debug.ThrowError("VertexBufferI", "A Vertex size does not match the buffer layout");
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
					Debug.ThrowError("VertexBufferI", "A Vertex size does not match the buffer layout");
				}

				for (int i = 0; i != newVertex.Length; ++i)
				{
					verts[vertSeg] = newVertex[i];
					++vertSeg;
				}
			}

			Init(verts);
			#else
			Debug.ThrowError("VertexBufferI", "Does not work on Xbox360.");
			#endif
		}*/

		public abstract void Update(float[] vertices, int updateCount);
		public abstract void Enable();
		public abstract void Enable(IndexBufferI indexBuffer);
		public abstract void Enable(VertexBufferI instanceBuffer);
		public abstract void Enable(IndexBufferI indexBuffer, VertexBufferI instanceBuffer);
		public abstract void Draw();
		public abstract void Draw(int drawCount);
		public abstract void DrawInstanced(int drawCount);
		public abstract void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount);
		#endregion
	}

	public static class VertexBufferAPI
	{
		public static void Init(NewPtrMethod newPtr, NewPtrMethod2 newPtr2)
		{
			VertexBufferAPI.newPtr = newPtr;
			VertexBufferAPI.newPtr2 = newPtr2;
		}

		public delegate VertexBufferI NewPtrMethod(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices);
		private static NewPtrMethod newPtr;
		public static VertexBufferI New(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices)
		{
			return newPtr(parent, bufferLayoutDesc, usage, topology, vertices);
		}

		public delegate VertexBufferI NewPtrMethod2(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices, int[] indices);
		private static NewPtrMethod2 newPtr2;
		public static VertexBufferI New(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices, int[] indices)
		{
			return newPtr2(parent, bufferLayoutDesc, usage, topology, vertices, indices);
		}
	}
}