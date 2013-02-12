using Reign.Core;
using System;

namespace Reign.UI
{
	public class ColorFadeEffect : EffectI
	{
		#region Properties
		public float IdleSpeed, RolloverSpeed, PressedSpeed;
		public Vector4 BackgroundColor, BackgroundColorIdle, BackgroundColorRollover, BackgroundColorPressed;
		public Vector4 ForegroundColor, ForegroundColorIdle, ForegroundColorRollover, ForegroundColorPressed;
		public Vector4 BorderColor, BorderColorIdle, BorderColorRollover, BorderColorPressed;
		#endregion

		#region Constructors
		public ColorFadeEffect(Vector4 backgroundColorIdle, Vector4 backgroundColorRollover, Vector4 backgroundColorPressed, Vector4 foregroundColorIdle, Vector4 foregroundColorRollover, Vector4 foregroundColorPressed, Vector4 borderColorIdle, Vector4 borderColorRollover, Vector4 borderColorPressed)
		{
			IdleSpeed = .1f;
			RolloverSpeed = .1f;
			PressedSpeed = .1f;

			this.BackgroundColor = backgroundColorIdle;
			this.BackgroundColorIdle = backgroundColorIdle;
			this.BackgroundColorRollover = backgroundColorRollover;
			this.BackgroundColorPressed = backgroundColorPressed;

			this.ForegroundColor = foregroundColorIdle;
			this.ForegroundColorIdle = foregroundColorIdle;
			this.ForegroundColorRollover = foregroundColorRollover;
			this.ForegroundColorPressed = foregroundColorPressed;

			this.BorderColor = borderColorIdle;
			this.BorderColorIdle = borderColorIdle;
			this.BorderColorRollover = borderColorRollover;
			this.BorderColorPressed = borderColorPressed;
		}
		#endregion

		#region Methods
		public void Update(Rect2 elementRect, VisualI[] visuals, ElementStates state, out Rect2 visualRect)
		{
			switch (state)
			{
				case ElementStates.None:
					BackgroundColor += (BackgroundColorIdle - BackgroundColor) * IdleSpeed;
					ForegroundColor += (ForegroundColorIdle - ForegroundColor) * IdleSpeed;
					BorderColor += (BorderColorIdle - BorderColor) * IdleSpeed;
					break;

				case ElementStates.Over:
					BackgroundColor += (BackgroundColorRollover - BackgroundColor) * RolloverSpeed;
					ForegroundColor += (ForegroundColorRollover - ForegroundColor) * RolloverSpeed;
					BorderColor += (BorderColorRollover - BorderColor) * RolloverSpeed;
					break;

				case ElementStates.Pressed:
					BackgroundColor += (BackgroundColorPressed - BackgroundColor) * PressedSpeed;
					ForegroundColor += (ForegroundColorPressed - ForegroundColor) * PressedSpeed;
					BorderColor += (BorderColorPressed - BorderColor) * PressedSpeed;
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
