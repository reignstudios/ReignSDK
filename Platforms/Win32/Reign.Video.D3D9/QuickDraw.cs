using System;
using Reign.Core;

namespace Reign.Video.D3D9
{
	public class QuickDraw : IQuickDraw
	{
		#region Properties
		private VertexBuffer vertexBuffer;
		#endregion

		#region Constructors
		public QuickDraw(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc)
		: base(parent, bufferLayoutDesc)
		{
			try
			{
				vertexBuffer = new VertexBuffer(this, bufferLayoutDesc, BufferUsages.Write, VertexBufferTopologys.Triangle, vertices);
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}
		#endregion

		#region Methods
		public override void StartTriangles()
		{
			base.StartTriangles();
			vertexBuffer.Topology = VertexBufferTopologys.Triangle;
			vertexBuffer.Enable();
		}

		public override void StartLines()
		{
			base.StartLines();
			vertexBuffer.Topology = VertexBufferTopologys.Line;
			vertexBuffer.Enable();
		}

		public override void StartPoints()
		{
			base.StartPoints();
			vertexBuffer.Topology = VertexBufferTopologys.Point;
			vertexBuffer.Enable();
		}

		public override void Draw()
		{
			vertexBuffer.Update(vertices, vertexNext);
			vertexBuffer.Draw(vertexNext);
		}

		public override void Color(float r, float g, float b, float a)
		{
			base.Color(b, g, r, a);
		}
		#endregion
	}
}