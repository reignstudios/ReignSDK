using Reign.Core;
using System;
using G = Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class VertexBuffer : VertexBufferI
	{
		#region Properties
		private Video video;
		private G.VertexBuffer vertexBuffer;
		private G.DrawMode primitiveTopology;
		private int indexCount = -1;

		private VertexBufferTopologys topology;
		public override VertexBufferTopologys Topology
		{
			get {return topology;}
			set
			{
				switch (value)
				{
					case VertexBufferTopologys.None: primitiveTopology = G.DrawMode.Points; break;
					case VertexBufferTopologys.Point: primitiveTopology = G.DrawMode.Points; break;
					case VertexBufferTopologys.Line: primitiveTopology = G.DrawMode.Lines; break;
					case VertexBufferTopologys.Triangle: primitiveTopology = G.DrawMode.Triangles; break;
				}
				topology = value;
			}
		}
		
		public override IndexBufferI IndexBuffer
		{
			get {return null;}
		}
		#endregion

		#region Constructors
		public static VertexBuffer New(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices)
		{
			return new VertexBuffer(parent, bufferLayoutDesc, usage, topology, vertices);
		}
		
		public static VertexBuffer New(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices, int[] indices)
		{
			return new VertexBuffer(parent, bufferLayoutDesc, usage, topology, vertices, indices);
		}

		public VertexBuffer(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, float[] vertices)
		: base(parent, bufferLayoutDesc, bufferUsage, vertices)
		{
			init(parent, bufferLayoutDesc, bufferUsage, vertexBufferTopology, vertices, null);
		}
		
		public VertexBuffer(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, float[] vertices, int[] indices)
		: base(parent, bufferLayoutDesc, bufferUsage, vertices)
		{
			init(parent, bufferLayoutDesc, bufferUsage, vertexBufferTopology, vertices, indices);
		}
		
		private void init(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, float[] vertices, int[] indices)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				Topology = vertexBufferTopology;
				
				var format = new G.VertexFormat[bufferLayoutDesc.Elements.Count];
				for (int i = 0; i != bufferLayoutDesc.Elements.Count; ++i)
				{
					switch (bufferLayoutDesc.Elements[i].FloatCount)
					{
						case 1: format[i] = G.VertexFormat.Float; break;
						case 2: format[i] = G.VertexFormat.Float2; break;
						case 3: format[i] = G.VertexFormat.Float3; break;
						case 4: format[i] = G.VertexFormat.Float4; break;
					}
				}
			
				if (indices != null && indices.Length != 0)
				{
					vertexBuffer = new G.VertexBuffer(vertexCount, indices.Length, format);
					
					indexCount = indices.Length;
					var indicesShort = new ushort[indexCount];
					for (int i = 0; i != indexCount; ++i)
					{
						indicesShort[i] = (ushort)indices[i];
					}
					vertexBuffer.SetIndices(indicesShort);
				}
				else
				{
					vertexBuffer = new G.VertexBuffer(vertexCount, format);
				}
				vertexBuffer.SetVertices(vertices);
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (vertexBuffer != null)
			{
				vertexBuffer.Dispose();
				vertexBuffer = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Update(float[] vertices, int updateCount)
		{
			vertexBuffer.SetVertices(vertices, 0, 0, updateCount);
		}

		public override void Enable()
		{
			video.context.SetVertexBuffer(0, vertexBuffer);
		}

		public override void Enable(IndexBufferI indexBuffer)
		{
			Debug.ThrowError("VertexBuffer", "IndexBuffer not supported. Create VertexBuffer with indices instead");
		}

		public override void Enable(VertexBufferI instanceBuffer)
		{
			Debug.ThrowError("VertexBuffer", "InstanceBuffer not supported. Use classic instancing instead");
		}

		public override void Enable(IndexBufferI indexBuffer, VertexBufferI instanceBuffer)
		{
			Debug.ThrowError("VertexBuffer", "InstanceBuffer not supported. Use classic instancing instead.");
		}

		public override void Draw()
		{
			if (indexCount != -1) video.context.DrawArrays(primitiveTopology, 0, indexCount);
			else video.context.DrawArrays(primitiveTopology, 0, vertexCount);
		}

		public override void Draw(int drawCount)
		{
			video.context.DrawArrays(primitiveTopology, 0, drawCount);
		}

		public override void DrawInstanced(int drawCount)
		{
			Debug.ThrowError("VertexBuffer", "Normal instancing not supported. Use classic instancing instead");
		}

		public override void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount)
		{
			if (indexCount != -1) video.context.DrawArrays(primitiveTopology, 0, meshIndexCount * drawCount);
			else video.context.DrawArrays(primitiveTopology, 0, meshVertexCount * drawCount);
		}
		#endregion
	}
}