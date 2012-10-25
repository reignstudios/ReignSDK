using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class VisualRectangle : RectangleShape, VisualI
	{
		#region Properties
		private UI ui;
		private GeometryI geometry;
		#endregion

		#region Constructors
		public VisualRectangle(UI ui, Vector2 location, Vector2 size, VisualFillModes fillMode)
		: base(location, size)
		{
			switch (fillMode)
			{
				case (VisualFillModes.Solid):
					geometry = ui.LoadGeometry(typeof(RectangleGeometry));
					break;

				case (VisualFillModes.Border):
					geometry = ui.LoadGeometry(typeof(RectangleBorderGeometry));
					break;
			}
		}
		#endregion
	}
}
