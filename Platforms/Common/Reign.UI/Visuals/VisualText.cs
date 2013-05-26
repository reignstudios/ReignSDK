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
		public float Fade {get; set;}
		public float Fade2 {get; set;}
		public Texture2DI Texture {get; set;}
		public Texture2DI Texture2 {get; set;}
		public Texture2DI Texture3 {get; set;}

		private Font font;
		private float fontSize;
		public string Caption;
		public bool CenterX, CenterY;
		#endregion

		#region Constructors
		public VisualText(Font font, float fontSize, Vector4 color, string caption, VisualLayers layer, bool centerX, bool centerY)
		{
			Layout = layer;
			this.font = font;
			this.fontSize = fontSize;
			Color = color;
			Caption = caption;
			CenterX = centerX;
			CenterY = centerY;
		}
		#endregion

		#region Methods
		public void Update(Rect2 elementRect)
		{
			rect = elementRect;
		}

		public void Render(UI ui)
		{
			font.DrawStart(ui.camera);
			var pos = Position.ToVector2() + (Size.ToVector2() * .5f);
			if (!CenterX) pos.X = Position.X;
			if (!CenterY) pos.Y = Position.Y;
			font.Draw(Caption, pos, Color, fontSize * ui.AutoScale, CenterX, CenterY);
		}
		#endregion
	}
}
