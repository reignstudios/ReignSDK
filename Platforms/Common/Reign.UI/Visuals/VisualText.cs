using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class VisualText : RectangleShape, VisualI
	{
		#region Properties
		public VisualLayers Layout {get; private set;}
		public Vector4 Color {get; set;}

		private Font font;
		private float fontSize;
		public string Caption;
		#endregion

		#region Constructors
		public VisualText(Font font, float fontSize, Vector4 color, string caption, VisualLayers layer)
		{
			Layout = layer;
			this.font = font;
			this.fontSize = fontSize;
			Color = color;
			Caption = caption;
		}
		#endregion

		#region Methods
		public void Update(Rect2 elementRect)
		{
			rect = elementRect;
		}

		public void Render(Camera camera)
		{
			font.DrawStart(camera);
			font.Draw(Caption, Position.ToVector2() + (Size.ToVector2() * .5f), Color, fontSize, true, true);
		}
		#endregion
	}
}
