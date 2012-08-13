using System;
using Reign.Core;

#if OSX
using MonoMac.AudioUnit;
#endif

#if iOS
using MonoTouch.AudioUnit;
#endif

namespace Reign.Audio.Cocoa
{
	public class Audio : Disposable, AudioI
	{
		#region Properties
		internal delegate void UpdateCallbackFunc();
		internal UpdateCallbackFunc UpdateCallback;
		
		internal AudioComponent component;
		#endregion

		#region Constructors
		public Audio(DisposableI parent)
		: base(parent)
		{
			#if OSX
			component = AudioComponent.FindComponent(AudioTypeOutput.Default);
			#else
			component = AudioComponent.FindComponent(AudioTypeOutput.Remote);
			#endif
			
			if (component == null) Debug.ThrowError("Audio", "Failed to find AudioComponent");
		}
		#endregion

		#region Methods
		public void Update()
		{
			if (UpdateCallback != null) UpdateCallback();
		}
		#endregion
	}
}
