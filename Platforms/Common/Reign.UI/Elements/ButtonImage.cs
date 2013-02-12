using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class ButtonImage : Element
	{
		#region Constructors
		public ButtonImage(UI ui, Texture2DI idleTexture, Texture2DI rolloverTexture, Texture2DI pressedTexture, int x, int y, int width, int height)
		: base(ui)
		{
			Effects = new EffectI[1];
			Effects[0] = new ImageFadeEffect(idleTexture, rolloverTexture, pressedTexture);

			var visuals = new VisualI[]
			{
				new VisualRectangle(ui, Vector4.One, idleTexture, rolloverTexture, pressedTexture, VisualLayers.Background, VisualFillModes.Solid)
			};

			init(visuals, new RectangleShape(new Point2(x, y), new Size2(width, height)));
		}
		#endregion
	}
}
