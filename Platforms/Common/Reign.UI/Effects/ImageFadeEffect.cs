using Reign.Core;
using Reign.Video;
using System;

namespace Reign.UI
{
	public class ImageFadeEffect : EffectI
	{
		#region Properties
		public float IdleSpeed, RolloverSpeed, PressedSpeed;
		private float idleTextureFade, rolloverTextureFade, pressedTextureFade;
		public Texture2DI IdleTexture, RolloverTexture, PressedTexture;
		#endregion

		#region Constructors
		public ImageFadeEffect(Texture2DI idleTexture, Texture2DI rolloverTexture, Texture2DI pressedTexture)
		{
			IdleSpeed = .1f;
			RolloverSpeed = .1f;
			PressedSpeed = .1f;
			idleTextureFade = 1;

			this.IdleTexture = idleTexture;
			this.RolloverTexture = rolloverTexture;
			this.PressedTexture = pressedTexture;
		}
		#endregion

		#region Methods
		public void Update(Rect2 elementRect, VisualI[] visuals, ElementStates state, out Rect2 visualRect)
		{
			switch (state)
			{
				case ElementStates.None:
					idleTextureFade += (1 - idleTextureFade) * IdleSpeed;
					rolloverTextureFade += (0 - rolloverTextureFade) * IdleSpeed;
					pressedTextureFade += (0 - pressedTextureFade) * IdleSpeed;
					break;

				case ElementStates.Over:
					idleTextureFade += (0 - idleTextureFade) * RolloverSpeed;
					rolloverTextureFade += (1 - rolloverTextureFade) * RolloverSpeed;
					pressedTextureFade += (0 - pressedTextureFade) * RolloverSpeed;
					break;

				case ElementStates.Pressed:
					idleTextureFade += (0 - idleTextureFade) * PressedSpeed;
					rolloverTextureFade += (0 - rolloverTextureFade) * PressedSpeed;
					pressedTextureFade += (1 - pressedTextureFade) * PressedSpeed;
					break;
			}
			
			foreach (var visual in visuals)
			{
				visual.Fade = rolloverTextureFade;
				visual.Fade2 = pressedTextureFade;
			}

			visualRect = elementRect;
		}
		#endregion
	}
}
