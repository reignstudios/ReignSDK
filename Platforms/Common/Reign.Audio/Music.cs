﻿using Reign.Core;

namespace Reign.Audio
{
	public enum MusicStates
	{
		Playing,
		Paused,
		Stopped
	}

	public interface IMusic : IDisposableResource
	{
		MusicStates State {get;}

		void Update();
		void Play();
		void Stop();
	}
}
