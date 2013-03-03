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

	public interface SoundInstanceI : DisposableI
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

	public abstract class SoundI : Disposable
	{
		#region Properites
		protected LinkedList<SoundInstanceI> activeInstances, inactiveInstances;
		#endregion

		#region Constructors
		public static SoundI New(DisposableI parent, string fileName, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		{
			string ext = Streams.GetFileExt(fileName);
			switch (ext.ToLower())
			{
				case ".wav": return SoundWAVAPI.New(parent, fileName, instanceCount, looped, loadedCallback);
				default:
					Debug.ThrowError("SoundI", string.Format("File 'ext' {0} not supported.", ext));
					return null;
			}
		}

		public SoundI(DisposableI parent)
		: base(parent)
		{
			activeInstances = new LinkedList<SoundInstanceI>();
			inactiveInstances = new LinkedList<SoundInstanceI>();
		}
		#endregion

		#region Methods
		public void Update()
		{
			var stoppedInstances = new Stack<SoundInstanceI>();
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

		public SoundInstanceI Play()
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

		public virtual SoundInstanceI Play(float volume)
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

	public static class SoundAPI
	{
		public static SoundI New(DisposableI parent, string fileName, int instanceCount, bool looped, Loader.LoadedCallbackMethod loadedCallback)
		{
			return SoundI.New(parent, fileName, instanceCount, looped, loadedCallback);
		}
	}
}
