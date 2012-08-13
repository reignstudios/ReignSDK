using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class QuickDraw : QuickDrawI
	{
		#region Properties
		private VertexBuffer vertexBuffer;
		#endregion

		#region Constructors
		public QuickDraw(DisposableI parent, BufferLayoutDescI bufferLayoutDesc)
		: base(parent, bufferLayoutDesc)
		{
			try
			{
				vertexBuffer = new VertexBuffer(this, bufferLayoutDesc, BufferUsages.Write, VertexBufferTopologys.Triangle, vertices);
			}
			catch (Exception ex)
			{
				Dispose();
				throw ex;
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
		#endregion
	}
}