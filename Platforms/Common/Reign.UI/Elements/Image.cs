using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class Image : Element
	{
		#region Constructors
		public Image(UI ui, Texture2DI texture, int x, int y, int width, int height)
		: base(ui)
		{
			var visuals = new VisualI[]
			{
				new VisualRectangle(ui, Vector4.One, texture, null, null, VisualLayers.Background, VisualFillModes.Solid)
			};

			init(visuals, new RectangleShape(new Point2(x, y), new Size2(width, height)));
		}
		#endregion
	}
}
