using Reign.Core;
using Reign.Core.MathF32;

namespace Reign.Input
{
	public interface TouchScreenI : DisposableI
	{
		Touch[] Touches {get;}
	}
}
