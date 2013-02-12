using System;
using Reign.Core;

namespace Reign.UI
{
	public class Button : Element
	{
		#region Properties
		private VisualText visualText;

		public string Caption
		{
			get {return visualText.Caption;}
			set {visualText.Caption = value;}
		}
		#endregion

		#region Constructors
		public Button(UI ui, string caption, int x, int y, int width, int height)
		: base(ui)
		{
			Effects = new EffectI[1];
			Effects[0] = new ColorFadeEffect(ui.BackgroundColorIdle, ui.BackgroundColorRollover, ui.BackgroundColorPressed, ui.ForegroundColorIdle, ui.ForegroundColorRollover, ui.ForegroundColorPressed, ui.BorderColorIdle, ui.BorderColorRollover, ui.BorderColorPressed);

			visualText = new VisualText(ui.font, ui.fontSize, ui.ForegroundColorIdle, caption, VisualLayers.Forground);
			var visuals = new VisualI[]
			{
				new VisualRectangle(ui, ui.BackgroundColorIdle, null, null, null, VisualLayers.Background, VisualFillModes.Solid),
				new VisualRectangle(ui, ui.BorderColorIdle, null, null, null, VisualLayers.Border, VisualFillModes.Border),
				visualText
			};

			init(visuals, new RectangleShape(new Point2(x, y), new Size2(width, height)));
		}
		#endregion
	}
}
