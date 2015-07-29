using Reign.Core;

namespace Reign.Audio.Dumby
{
	public class Music : DisposableResource, IMusic
	{
		#region Properties
		public MusicStates State {get; private set;}
		#endregion

		#region Constructors
		public Music(IDisposableResource parent, string filename)
		: base(parent)
		{
			State = MusicStates.Stopped;
		}
		#endregion

		#region Methods
		public void Update()
		{
			
		}

		public void Play()
		{
			
		}

		public void Stop()
		{
			
		}
		#endregion
	}
}
