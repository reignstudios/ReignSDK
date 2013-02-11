using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class ButtonImage : Element
	{
		#region Constructors
		public ButtonImage(UI ui, Texture2DI idleTexture, int x, int y, int width, int height)
		: base(ui)
		{
			//Effect = new ColorFadeEffect(ui.BackgroundColorIdle, ui.BackgroundColorRollover, ui.BackgroundColorPressed, ui.ForegroundColorIdle, ui.ForegroundColorRollover, ui.ForegroundColorPressed, ui.BorderColorIdle, ui.BorderColorRollover, ui.BorderColorPressed);

			var visuals = new VisualI[]
			{
				new VisualRectangle(ui, Vector4.One, idleTexture, VisualLayers.Background, VisualFillModes.Solid)
			};

			init(visuals, new RectangleShape(new Point2(x, y), new Size2(width, height)));
		}
		#endregion
	}
}
