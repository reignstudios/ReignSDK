using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class RectangleGeometry : Disposable, GeometryI
	{
		#region Properties
		private UI ui;
		private VertexBufferI vertexBuffer;
		#endregion

		#region Constructors
		public RectangleGeometry(DisposableI parent)
		: base(parent)
		{
			try
			{
				this.ui = parent.FindParentOrSelfWithException<UI>();

				var verts = new float[]
				{
					0, 0,
					0, 1,
					1, 1,
					1, 0
				};

				var indices = new int[]
				{
					0, 1, 2,
					0, 2, 3
				};

				vertexBuffer = VertexBufferAPI.New(ui.video, BufferLayoutDescAPI.New(BufferLayoutTypes.Position2), BufferUsages.Default, VertexBufferTopologys.Triangle, verts, indices);
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
		public void Render()
		{
			if (ui.lastGeometryRendered != this) vertexBuffer.Enable();
			ui.lastGeometryRendered = this;
			vertexBuffer.Draw();
		}
		#endregion
	}
}
