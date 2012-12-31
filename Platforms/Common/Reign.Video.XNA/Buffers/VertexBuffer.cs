using X = Microsoft.Xna.Framework.Graphics;
using System;
using Reign.Core;

namespace Reign.Video.XNA
{
	public class VertexBuffer : VertexBufferI
	{
		#region Properties
		private Video video;
		private X.VertexBuffer vertexBuffer;
		private BufferLayout bufferLayout;
		private X.PrimitiveType primitiveTopology;
		private int primitiveVertexCount;
		private IndexBuffer indexBuffer;
		private VertexBuffer instanceBuffer;

		private VertexBufferTopologys topology;
		public override VertexBufferTopologys Topology
		{
			get {return topology;}
			set
			{
				switch (value)
				{
					case (VertexBufferTopologys.None): primitiveTopology = X.PrimitiveType.LineList; primitiveVertexCount = 0; break;
					case (VertexBufferTopologys.Point): Debug.ThrowError("VertexBuffer", "Point VertexBufferTopology NOT Supported in XNA"); break;
					case (VertexBufferTopologys.Line): primitiveTopology = X.PrimitiveType.LineList; primitiveVertexCount = 2; break;
					case (VertexBufferTopologys.Triangle): primitiveTopology = X.PrimitiveType.TriangleList; primitiveVertexCount = 3; break;
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
				bufferLayout = new BufferLayout(this, null, bufferLayoutDesc, true);

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
			}

			vertexBuffer = new X.VertexBuffer(video.Device, bufferLayout.layout, vertexCount, X.BufferUsage.WriteOnly);
			Update(vertices, vertexCount);
		}

		public override void Update(float[] vertices, int updateCount)
		{
			vertexBuffer.SetData<float>(vertices, 0, (vertexFloatArraySize * updateCount));
		}

		private void enable(IndexBufferI indexBuffer, VertexBufferI instanceBuffer)
		{
			if (instanceBuffer == null)
			{
				video.Device.SetVertexBuffer(vertexBuffer);
				instanceBuffer = null;
			}
			else
			{
				this.instanceBuffer = (VertexBuffer)instanceBuffer;
			}

			if (indexBuffer == null)
			{
				video.Device.Indices = null;
				indexBuffer = null;
			}
			else
			{
				this.indexBuffer = (IndexBuffer)indexBuffer;
				this.indexBuffer.enable();
			}
		}

		public override void Enable()
		{
			enable(null, null);
		}

		public override void Enable(IndexBufferI indexBuffer)
		{
			enable(indexBuffer, null);
		}

		public override void Enable(VertexBufferI instanceBuffer)
		{
			enable(null, instanceBuffer);
		}

		public override void Enable(IndexBufferI indexBuffer, VertexBufferI instanceBuffer)
		{
			enable(indexBuffer, instanceBuffer);
		}

		public override void Draw()
		{
			if (indexBuffer == null) video.Device.DrawPrimitives(primitiveTopology, 0, (vertexCount/primitiveVertexCount));
			else video.Device.DrawIndexedPrimitives(primitiveTopology, 0, 0, vertexCount, 0, (indexBuffer.IndexCount/primitiveVertexCount));
		}

		public override void Draw(int drawCount)
		{
			if (indexBuffer == null) video.Device.DrawPrimitives(primitiveTopology, 0, (drawCount/primitiveVertexCount));
			else video.Device.DrawIndexedPrimitives(primitiveTopology, 0, 0, vertexCount, 0, (drawCount/primitiveVertexCount));
		}

		public override void DrawInstanced(int drawCount)
		{
			#if SILVERLIGHT
			Debug.ThrowError("VertexBuffer", "DrawInstanced is not supported, use DrawInstancedClassic instead");
			#else
			var buffers = new X.VertexBufferBinding[2]
			{
			    new X.VertexBufferBinding(vertexBuffer, 0, 0),
			    new X.VertexBufferBinding(instanceBuffer.vertexBuffer, 0, 1)
			};
			video.Device.SetVertexBuffers(buffers);

			int primitiveCount;
			if (indexBuffer == null) primitiveCount = (vertexCount/primitiveVertexCount);
			else primitiveCount = (indexBuffer.IndexCount/primitiveVertexCount);
			video.Device.DrawInstancedPrimitives(primitiveTopology, 0, 0, vertexCount, 0, primitiveCount, drawCount);
			#endif
		}

		public override void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount)
		{
			if (indexBuffer == null) video.Device.DrawPrimitives(primitiveTopology, 0, drawCount * (meshVertexCount/primitiveVertexCount));
			else video.Device.DrawIndexedPrimitives(primitiveTopology, 0, 0, drawCount * meshVertexCount, 0, drawCount * (meshIndexCount/primitiveVertexCount));
		}
		#endregion
	}
}