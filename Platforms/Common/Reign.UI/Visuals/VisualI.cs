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

	public interface VisualI : ShapeI
	{
		void Update(Rect2 elementRect);
		void Render(Camera camera);
	}
}
