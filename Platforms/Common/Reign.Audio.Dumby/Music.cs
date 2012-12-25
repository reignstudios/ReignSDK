using Reign.Core;

namespace Reign.Audio.Dumby
{
	public class Music : Disposable, MusicI
	{
		#region Properties
		public MusicStates State {get; private set;}
		#endregion

		#region Constructors
		public static Music New(DisposableI parent, string fileName)
		{
			return new Music(parent, fileName);
		}

		public Music(DisposableI parent, string fileName)
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
