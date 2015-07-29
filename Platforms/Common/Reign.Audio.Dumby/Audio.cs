using Reign.Core;

namespace Reign.Audio.Dumby
{
	public class Audio : DisposableResource, IAudio
	{
		public Audio(IDisposableResource parent)
		: base(parent)
		{
			
		}

		public void Update() {}
	}
}
