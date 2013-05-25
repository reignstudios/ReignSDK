using System;
using System.Collections.Generic;
using Reign.Core;

namespace Reign.UI
{
	public class Label : Element
	{
		#region Properties
		private VisualText visualText;

		public string Text
		{
			get {return visualText.Caption;}
			set {visualText.Caption = value;}
		}
		#endregion

		#region Constructors
		public Label(UI ui, string text, int x, int y, int width, int height, bool hasBackground, bool hasBorder)
		: base(ui)
		{
			visualText = new VisualText(ui.font, ui.fontSize, ui.ForegroundColorIdle, text, VisualLayers.Forground);
			var visuals = new List<VisualI>();
			if (hasBackground) visuals.Add(new VisualRectangle(ui, ui.BackgroundColorIdle, null, null, null, VisualLayers.Background, VisualFillModes.Solid));
			if (hasBorder) visuals.Add(new VisualRectangle(ui, ui.BorderColorIdle, null, null, null, VisualLayers.Border, VisualFillModes.Border));
			visuals.Add(visualText);

			init(visuals.ToArray(), new RectangleShape(new Point2(x, y), new Size2(width, height)));
		}
		#endregion
	}
}
