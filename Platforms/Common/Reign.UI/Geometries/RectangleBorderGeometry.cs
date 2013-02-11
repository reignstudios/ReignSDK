﻿using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class RectangleBorderGeometry : Disposable, GeometryI
	{
		#region Properties
		private UI ui;
		private VertexBufferI vertexBuffer;
		private IndexBufferI indexBuffer;
		#endregion

		#region Constructors
		public RectangleBorderGeometry(DisposableI parent)
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
				vertexBuffer = VertexBufferAPI.New(ui.video, BufferLayoutDescAPI.New(BufferLayoutTypes.Position2), BufferUsages.Default, VertexBufferTopologys.Line, verts);

				var indices = new int[]
				{
					0, 1,
					1, 2,
					2, 3,
					3, 0
				};
				indexBuffer = IndexBufferAPI.New(ui.video, BufferUsages.Default, indices);
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

			if (indexBuffer != null)
			{
				indexBuffer.Dispose();
				indexBuffer = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Render()
		{
			if (ui.lastGeometryRendered != this) vertexBuffer.Enable(indexBuffer);
			ui.lastGeometryRendered = this;
			vertexBuffer.Draw();
		}
		#endregion
	}
}
