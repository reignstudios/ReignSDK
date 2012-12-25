using System;
using Reign.Core;

namespace Reign.Audio.Android
{
	public class Audio : Disposable, AudioI
	{
		#region Properties
		internal delegate void UpdateCallbackFunc();
		internal UpdateCallbackFunc updateCallback;
		
		private const int maxInstances = 30;
		private int runningInstances;
		#endregion

		#region Constructors
		public Audio(DisposableI parent)
		: base(parent)
		{
			
		}
		#endregion

		#region Methods
		public void Update()
		{
			if (updateCallback != null) updateCallback();
		}
		
		internal bool addInstance()
		{
			if (runningInstances >= maxInstances) return false;
			++runningInstances;
			return true;
		}
		
		internal void removeinstance()
		{
			--runningInstances;
			if (runningInstances < 0) runningInstances = 0;
		}
		#endregion
	}
}
