using System;
using Reign_Video_D3D9_Component;
using Reign.Video;
using Reign.Core;

namespace Reign.Video.D3D9
{
	public class VertexBuffer : IVertexBuffer
	{
		#region Properties
		private VertexBufferCom com;

		private VertexBufferTopologys topology;
		public override VertexBufferTopologys Topology
		{
			get {return topology;}
			set
			{
				switch (value)
				{
					case VertexBufferTopologys.Triangle: com.SetTopology(REIGN_D3DPRIMITIVETYPE.TRIANGLELIST); break;
					case VertexBufferTopologys.Line: com.SetTopology(REIGN_D3DPRIMITIVETYPE.LINELIST); break;
					case VertexBufferTopologys.Point: com.SetTopology(REIGN_D3DPRIMITIVETYPE.POINTLIST); break;
				}
				topology = value;
			}
		}

		private IndexBuffer indexBuffer, currentIndexBuffer;
		public override IIndexBuffer IndexBuffer
		{
			get {return indexBuffer;}
		}
		#endregion

		#region Constructors
		public VertexBuffer(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices)
		: base(parent, bufferLayoutDesc, usage, vertices)
		{
			init(parent, bufferLayoutDesc, usage, topology, vertices, null);
		}

		public VertexBuffer(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices, int[] indices)
		: base(parent, bufferLayoutDesc, usage, vertices)
		{
			init(parent, bufferLayoutDesc, usage, topology, vertices, indices);
		}

		private void init(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices, int[] indices)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				this.topology = topology;
				REIGN_D3DPRIMITIVETYPE topologyType = REIGN_D3DPRIMITIVETYPE.TRIANGLELIST;
				switch (topology)
				{
					case VertexBufferTopologys.Triangle: topologyType = REIGN_D3DPRIMITIVETYPE.TRIANGLELIST; break;
					case VertexBufferTopologys.Line: topologyType = REIGN_D3DPRIMITIVETYPE.LINELIST; break;
					case VertexBufferTopologys.Point: topologyType = REIGN_D3DPRIMITIVETYPE.POINTLIST; break;
				}

				com = new VertexBufferCom(video.com, topologyType);
				var error = com.Init(vertices, REIGN_D3DUSAGE.WRITEONLY, vertexCount, vertexByteSize);
				if (error == VertexBufferErrors.VertexBuffer) Debug.ThrowError("VertexBuffer", "Failed to create VertexBuffer");
				
				if (indices != null && indices.Length != 0) indexBuffer = new IndexBuffer(this, usage, indices);
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
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Update(float[] vertices, int updateCount)
		{
			com.Update(vertices, updateCount, vertexByteSize);
		}

		public override void Enable()
		{
			currentIndexBuffer = indexBuffer;
			if (indexBuffer != null) com.Enable(indexBuffer.com, null, vertexByteSize, 0);
			else com.Enable(null, null, vertexByteSize, 0);
		}

		public override void Enable(IIndexBuffer indexBuffer)
		{
			this.currentIndexBuffer = (IndexBuffer)indexBuffer;
			com.Enable(((IndexBuffer)indexBuffer).com, null, vertexByteSize, 0);
		}

		public override void Enable(IVertexBuffer instanceBuffer)
		{
			currentIndexBuffer = indexBuffer;
			var ib = ((VertexBuffer)instanceBuffer);
			if (indexBuffer != null) com.Enable(indexBuffer.com, ib.com, vertexByteSize, ib.vertexByteSize);
			else com.Enable(null, ib.com, vertexByteSize, ib.vertexByteSize);
		}

		public override void Enable(IIndexBuffer indexBuffer, IVertexBuffer instanceBuffer)
		{
			this.currentIndexBuffer = (IndexBuffer)indexBuffer;
			var ib = ((VertexBuffer)instanceBuffer);
			com.Enable(((IndexBuffer)indexBuffer).com, ib.com, vertexByteSize, ib.vertexByteSize);
		}

		public override void Draw()
		{
			com.Draw(vertexCount, vertexCount, currentIndexBuffer.IndexCount);
		}

		public override void Draw(int drawCount)
		{
			com.Draw(drawCount, vertexCount);
		}

		public override void DrawInstanced(int drawCount)
		{
			com.DrawInstanced(drawCount, vertexCount);
		}

		public override void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount)
		{
			com.DrawInstancedClassic(drawCount, meshVertexCount, meshIndexCount);
		}
		#endregion
	}
}
