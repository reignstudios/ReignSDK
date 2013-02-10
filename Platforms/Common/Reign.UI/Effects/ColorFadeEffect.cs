using Reign.Core;
using System;

namespace Reign.UI
{
	public class ColorFadeEffect : EffectI
	{
		#region Properties
		public float BackgroundColorSpeed, ForegroundColorSpeed, BorderColorSpeed;
		public Vector4 BackgroundColor, BackgroundColorIdle, BackgroundColorRollover, BackgroundColorPressed;
		public Vector4 ForegroundColor, ForegroundColorIdle, ForegroundColorRollover, ForegroundColorPressed;
		public Vector4 BorderColor, BorderColorIdle, BorderColorRollover, BorderColorPressed;
		#endregion

		#region Constructors
		public ColorFadeEffect(Vector4 BackgroundColorIdle, Vector4 BackgroundColorRollover, Vector4 BackgroundColorPressed, Vector4 ForegroundColorIdle, Vector4 ForegroundColorRollover, Vector4 ForegroundColorPressed, Vector4 BorderColorIdle, Vector4 BorderColorRollover, Vector4 BorderColorPressed)
		{
			BackgroundColorSpeed = .1f;
			ForegroundColorSpeed = .1f;
			BorderColorSpeed = .1f;

			this.BackgroundColor = BackgroundColorIdle;
			this.BackgroundColorIdle = BackgroundColorIdle;
			this.BackgroundColorRollover = BackgroundColorRollover;
			this.BackgroundColorPressed = BackgroundColorPressed;

			this.ForegroundColor = ForegroundColorIdle;
			this.ForegroundColorIdle = ForegroundColorIdle;
			this.ForegroundColorRollover = ForegroundColorRollover;
			this.ForegroundColorPressed = ForegroundColorPressed;

			this.BorderColor = BorderColorIdle;
			this.BorderColorIdle = BorderColorIdle;
			this.BorderColorRollover = BorderColorRollover;
			this.BorderColorPressed = BorderColorPressed;
		}
		#endregion

		#region Methods
		public void Update(Rect2 elementRect, VisualI[] visuals, ElementStates state, out Rect2 visualRect)
		{
			switch (state)
			{
				case ElementStates.None:
					BackgroundColor += (BackgroundColorIdle - BackgroundColor) * BackgroundColorSpeed;
					ForegroundColor += (ForegroundColorIdle - ForegroundColor) * ForegroundColorSpeed;
					BorderColor += (BorderColorIdle - BorderColor) * BorderColorSpeed;
					break;

				case ElementStates.Over:
					BackgroundColor += (BackgroundColorRollover - BackgroundColor) * BackgroundColorSpeed;
					ForegroundColor += (ForegroundColorRollover - ForegroundColor) * ForegroundColorSpeed;
					BorderColor += (BorderColorRollover - BorderColor) * BorderColorSpeed;
					break;

				case ElementStates.Pressed:
					BackgroundColor += (BackgroundColorPressed - BackgroundColor) * BackgroundColorSpeed;
					ForegroundColor += (ForegroundColorPressed - ForegroundColor) * ForegroundColorSpeed;
					BorderColor += (BorderColorPressed - BorderColor) * BorderColorSpeed;
					break;
			}

			foreach (var visual in visuals)
			{
				switch (visual.Layout)
				{
					case VisualLayers.Background: visual.Color = BackgroundColor; break;
					case VisualLayers.Forground: visual.Color = ForegroundColor; break;
					case VisualLayers.Border: visual.Color = BorderColor; break;
				}
			}

			visualRect = elementRect;
		}
		#endregion
	}
}
