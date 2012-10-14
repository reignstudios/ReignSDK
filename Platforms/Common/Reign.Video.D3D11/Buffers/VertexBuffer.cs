using Reign.Core;
using Reign_Video_D3D11_Component;
using System;

namespace Reign.Video.D3D11
{
	public class VertexBuffer : VertexBufferI
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
					case (VertexBufferTopologys.Triangle): com.SetTopology(REIGN_D3D_PRIMITIVE_TOPOLOGY.TRIANGLELIST); break;
					case (VertexBufferTopologys.Line): com.SetTopology(REIGN_D3D_PRIMITIVE_TOPOLOGY.LINELIST); break;
					case (VertexBufferTopologys.Point): com.SetTopology(REIGN_D3D_PRIMITIVE_TOPOLOGY.POINTLIST); break;
				}
				topology = value;
			}
		}
		#endregion

		#region Constructors
		public VertexBuffer(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices)
		: base(parent, bufferLayoutDesc, usage)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				this.topology = topology;
				REIGN_D3D_PRIMITIVE_TOPOLOGY topologyType = REIGN_D3D_PRIMITIVE_TOPOLOGY.TRIANGLELIST;
				switch (topology)
				{
					case (VertexBufferTopologys.Triangle): topologyType = REIGN_D3D_PRIMITIVE_TOPOLOGY.TRIANGLELIST; break;
					case (VertexBufferTopologys.Line): topologyType = REIGN_D3D_PRIMITIVE_TOPOLOGY.LINELIST; break;
					case (VertexBufferTopologys.Point): topologyType = REIGN_D3D_PRIMITIVE_TOPOLOGY.POINTLIST; break;
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

			var bufferUsage = (usage == BufferUsages.Write) ? REIGN_D3D11_USAGE.DYNAMIC : REIGN_D3D11_USAGE.DEFAULT;
			var cpuUsage = (usage == BufferUsages.Write) ? REIGN_D3D11_CPU_ACCESS_FLAG.WRITE : (REIGN_D3D11_CPU_ACCESS_FLAG)0;
			var error = com.Init(vertices, vertexCount, vertexByteSize, bufferUsage, cpuUsage);

			if (error == VertexBufferErrors.Buffer) Debug.ThrowError("VertexBuffer", "Failed to create Buffer");
		}

		public override void Update(float[] vertices, int updateCount)
		{
			com.Update(vertices, updateCount);
		}

		public override void Enable()
		{
			com.Enable();
		}

		public override void Enable(IndexBufferI indexBuffer)
		{
			com.Enable(((IndexBuffer)indexBuffer).com);
		}

		public override void Enable(VertexBufferI instanceBuffer)
		{
			com.Enable(((VertexBuffer)instanceBuffer).com);
		}

		public override void Enable(IndexBufferI indexBuffer, VertexBufferI instanceBuffer)
		{
			com.Enable(((IndexBuffer)indexBuffer).com, ((VertexBuffer)instanceBuffer).com);
		}

		public override void Draw()
		{
			com.Draw();
		}

		public override void Draw(int drawCount)
		{
			com.Draw(drawCount);
		}

		public override void DrawInstanced(int drawCount)
		{
			com.DrawInstanced(drawCount);
		}

		public override void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount)
		{
			com.DrawInstancedClassic(drawCount, meshVertexCount, meshIndexCount);
		}
		#endregion
	}
}