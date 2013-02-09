using System;
using Reign.Core;

namespace Reign.UI
{
	public class Button : Element
	{
		#region Properties
		public string Caption;
		#endregion

		#region Constructors
		public Button(UI ui, string caption, int x, int y, int width, int height)
		: base(ui, x, y, width, height)
		{
			Caption = caption;

			var visuals = new VisualI[1]
			{
				new VisualRectangle(ui, ui.BackgroundColor, null, VisualFillModes.Solid)
			};
			init(visuals, visuals[0]);
		}
		#endregion

		#region Methods
		public override void Render()
		{
			base.Render();

			// TODO: make this part a visual
			ui.font.DrawStart(ui.camera);
			ui.font.Draw(Caption, Position.ToVector2(), new Vector4(1, 1, 1, 1), 32, false, false);
		}
		#endregion
	}
}
