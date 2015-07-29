using Reign.Core;
using System.Collections.Generic;

namespace Reign.Audio
{
	public enum SoundStates
	{
		Playing,
		Paused,
		Stopped
	}

	public interface ISoundInstance : IDisposableResource
	{
		SoundStates State {get;}
		bool Looped {get;}
		float Volume {get; set;}

		void Update();
		void Play();
		void Play(float volume);
		void Pause();
		void Stop();
	}

	public abstract class ISound : DisposableResource
	{
		#region Properites
		protected LinkedList<ISoundInstance> activeInstances, inactiveInstances;
		#endregion

		#region Constructors
		public ISound(IDisposableResource parent)
		: base(parent)
		{
			activeInstances = new LinkedList<ISoundInstance>();
			inactiveInstances = new LinkedList<ISoundInstance>();
		}
		#endregion

		#region Methods
		public void Update()
		{
			var stoppedInstances = new Stack<ISoundInstance>();
			foreach (var active in activeInstances)
			{
				active.Update();

				if (active.State == SoundStates.Stopped)
				{
					stoppedInstances.Push(active);
				}
			}

			foreach (var inactive in stoppedInstances)
			{
				activeInstances.Remove(inactive);
				inactiveInstances.AddLast(inactive);
			}
		}

		public ISoundInstance Play()
		{
			if (inactiveInstances.Count != 0)
			{
				var instance = inactiveInstances.Last;
				inactiveInstances.Remove(instance);
				activeInstances.AddLast(instance);

				instance.Value.Play();
				return instance.Value;
			}

			return null;
		}

		public virtual ISoundInstance Play(float volume)
		{
			if (inactiveInstances.Count != 0)
			{
				var instance = inactiveInstances.Last;
				inactiveInstances.Remove(instance);
				activeInstances.AddLast(instance);

				instance.Value.Play(volume);
				return instance.Value;
			}

			return null;
		}

		public void Pause()
		{
			foreach (var active in activeInstances)
			{
				active.Pause();
			}
		}

		public void Stop()
		{
			foreach (var active in activeInstances)
			{
				active.Stop();
			}
		}
		#endregion
	}
}
