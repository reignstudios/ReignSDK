using System;
using Reign.Core;

namespace Reign.UI
{
	public interface EffectI
	{
		void Update(Rect2 elementRect, VisualI[] visuals, ElementStates state, out Rect2 visualRect);
	}
}
