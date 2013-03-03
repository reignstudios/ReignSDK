using System;
using Reign_Video_D3D9_Component;
using Reign.Video;
using Reign.Core;

namespace Reign.Video.D3D9
{
	public class VertexBuffer : VertexBufferI
	{
		#region Properties
		private VertexBufferCom com;
		private IndexBuffer indexBuffer;

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
		#endregion

		#region Constructors
		public static VertexBuffer New(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices)
		{
			return new VertexBuffer(parent, bufferLayoutDesc, usage, topology, vertices);
		}

		public VertexBuffer(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices)
		: base(parent, bufferLayoutDesc, usage)
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
				Init(vertices);
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
		public override void Init(float[] vertices)
		{
			base.Init(vertices);

			var error = com.Init(vertices, REIGN_D3DUSAGE.WRITEONLY, vertexCount, vertexByteSize);
			if (error == VertexBufferErrors.VertexBuffer) Debug.ThrowError("VertexBuffer", "Failed to create VertexBuffer");
		}

		public override void Update(float[] vertices, int updateCount)
		{
			com.Update(vertices, updateCount, vertexByteSize);
		}

		public override void Enable()
		{
			indexBuffer = null;
			com.Enable(null, null, vertexByteSize, 0);
		}

		public override void Enable(IndexBufferI indexBuffer)
		{
			this.indexBuffer = (IndexBuffer)indexBuffer;
			com.Enable(((IndexBuffer)indexBuffer).com, null, vertexByteSize, 0);
		}

		public override void Enable(VertexBufferI instanceBuffer)
		{
			indexBuffer = null;
			var ib = ((VertexBuffer)instanceBuffer);
			com.Enable(null, ib.com, vertexByteSize, ib.vertexByteSize);
		}

		public override void Enable(IndexBufferI indexBuffer, VertexBufferI instanceBuffer)
		{
			this.indexBuffer = (IndexBuffer)indexBuffer;
			var ib = ((VertexBuffer)instanceBuffer);
			com.Enable(((IndexBuffer)indexBuffer).com, ib.com, vertexByteSize, ib.vertexByteSize);
		}

		public override void Draw()
		{
			com.Draw(vertexCount, vertexCount, indexBuffer.IndexCount);
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
