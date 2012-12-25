using System;
using Reign.Core;

namespace Reign.Video.XNA
{
	public class QuickDraw : QuickDrawI
	{
		#region Properties
		private Video video;
		private VertexBuffer vertexBuffer;
		#endregion

		#region Constructors
		public static QuickDraw New(DisposableI parent, BufferLayoutDescI bufferLayoutDesc)
		{
			return new QuickDraw(parent, bufferLayoutDesc);
		}

		public QuickDraw(DisposableI parent, BufferLayoutDescI bufferLayoutDesc)
		: base(parent, bufferLayoutDesc)
		{
			init(parent, bufferLayoutDesc);
		}

		private void init(DisposableI parent, BufferLayoutDescI bufferLayoutDesc)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
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
			video.DisableVertexBuffer();
			vertexBuffer.Update(vertices, vertexNext);
			vertexBuffer.Enable();
			vertexBuffer.Draw(vertexNext);
		}

		#if XBOX360
		public override void Color(float r, float g, float b, float a)
		{
			base.Color(a, b, g, r);
		}
		#endif
		#endregion
	}
}