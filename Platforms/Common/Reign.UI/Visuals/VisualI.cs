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

		void Update(Rect2 elementRect);
		void Render(Camera camera);
	}
}
