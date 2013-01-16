using Reign.Core;
using System;
using G = Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class VertexBuffer : VertexBufferI
	{
		#region Properties
		private Video video;
		private bool initialized;
		private G.VertexBuffer vertexBuffer;
		private G.DrawMode primitiveTopology;
		private IndexBuffer indexBuffer;
		private float[] vertices;
		private BufferLayoutDesc bufferLayoutDesc;

		private VertexBufferTopologys topology;
		public override VertexBufferTopologys Topology
		{
			get {return topology;}
			set
			{
				switch (value)
				{
					case (VertexBufferTopologys.None): primitiveTopology = G.DrawMode.Points; break;
					case (VertexBufferTopologys.Point): primitiveTopology = G.DrawMode.Points; break;
					case (VertexBufferTopologys.Line): primitiveTopology = G.DrawMode.Lines; break;
					case (VertexBufferTopologys.Triangle): primitiveTopology = G.DrawMode.Triangles; break;
				}
				topology = value;
			}
		}
		#endregion

		#region Constructors
		public static VertexBuffer New(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices)
		{
			return new VertexBuffer(parent, bufferLayoutDesc, usage, topology, vertices);
		}

		public VertexBuffer(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, float[] vertices)
		: base(parent, bufferLayoutDesc, bufferUsage)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				Topology = vertexBufferTopology;
				this.bufferLayoutDesc = (BufferLayoutDesc)bufferLayoutDesc;

				if (vertices != null) Init(vertices);
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
		public override void Init(float[] vertices)
		{
			base.Init(vertices);
			if (vertexBuffer != null)
			{
				vertexBuffer.Dispose();
				vertexBuffer = null;
				indexBuffer = null;
				initialized = false;
			}
			
			this.vertices = vertices;
		}

		public override void Update(float[] vertices, int updateCount)
		{
			vertexBuffer.SetVertices(vertices, 0, 0, updateCount);
		}

		public override void Enable()
		{
			if (!initialized) initialize(null);
			video.context.SetVertexBuffer(0, vertexBuffer);
		}
		
		private void initialize(IndexBuffer indexBuffer)
		{
			var format = new G.VertexFormat[bufferLayoutDesc.Elements.Count];
			for (int i = 0; i != bufferLayoutDesc.Elements.Count; ++i)
			{
				switch (bufferLayoutDesc.Elements[i].FloatCount)
				{
					case (1): format[i] = G.VertexFormat.Float; break;
					case (2): format[i] = G.VertexFormat.Float2; break;
					case (3): format[i] = G.VertexFormat.Float3; break;
					case (4): format[i] = G.VertexFormat.Float4; break;
				}
			}
		
			if (indexBuffer != null)
			{
				vertexBuffer = new G.VertexBuffer(vertices.Length, indexBuffer.indices.Length, format);
				vertexBuffer.SetIndices(indexBuffer.indices);
			}
			else
			{
				vertexBuffer = new G.VertexBuffer(vertices.Length, format);
			}
			vertexBuffer.SetVertices(vertices);
			vertices = null;
			this.indexBuffer = indexBuffer;
			initialized = true;
		}

		public override void Enable(IndexBufferI indexBuffer)
		{
			// check if index buffer is usable
			var i = (IndexBuffer)indexBuffer;
			if (i.Used && !i.Updateable) Debug.ThrowError("VertexBuffer", "IndexBuffer has already been used. IndexBuffer can only be used with one VertexBuffer");
			
			// init
			if (!initialized) initialize(i);
			else if (this.indexBuffer == null) Debug.ThrowError("VertexBuffer", "VertexBuffer has already been used without an index buffer");
			
			if (!i.Updateable)
			{
				i.Used = true;
				i.indices = null;
			}
			else if (this.indexBuffer != i)
			{
				vertexBuffer.SetIndices(i.indices);
			}
			
			video.context.SetVertexBuffer(0, vertexBuffer);
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
			video.context.DrawArrays(primitiveTopology, 0, vertexCount);
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
			video.context.DrawArrays(primitiveTopology, 0, meshVertexCount * drawCount);
		}
		#endregion
	}
}