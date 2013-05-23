using Reign.Core;
using Reign.Video;
using System;

namespace Reign.UI
{
	public enum VisualFillModes
	{
		Solid,
		Border
	}

	public enum VisualLayers
	{
		Background,
		Forground,
		Border
	}

	public interface VisualI : ShapeI
	{
		VisualLayers Layout {get;}
		Vector4 Color {get; set;}
		float Fade {get; set;}
		float Fade2 {get; set;}
		Texture2DI Texture {get; set;}
		Texture2DI Texture2 {get; set;}
		Texture2DI Texture3 {get; set;}

		void Update(Rect2 elementRect);
		void Render(UI ui);
	}
}
