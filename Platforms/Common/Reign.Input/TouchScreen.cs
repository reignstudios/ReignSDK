using Reign.Core;

namespace Reign.Input
{
	public interface ITouchScreen : IDisposableResource
	{
		Touch[] Touches {get;}
	}
}
