using Reign.Core;

namespace Reign.Input
{
	public interface TouchScreenI : DisposableI
	{
		Touch[] Touches {get;}
	}
}
