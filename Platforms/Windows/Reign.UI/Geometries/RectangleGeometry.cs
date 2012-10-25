using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class RectangleGeometry : Disposable, GeometryI
	{
		#region Properties
		private VertexBufferI vertexBuffer;
		private IndexBufferI indexBuffer;
		#endregion

		#region Constructors
		public RectangleGeometry(DisposableI parent)
		: base(parent)
		{
			
		}
		#endregion

		#region Methods
		public void Render()
		{
			
		}
		#endregion
	}
}
