using Reign.Core;
using Reign_Audio_XAudio_Component;
using System;

namespace Reign.Audio.XAudio
{
	public class Audio : Disposable, AudioI
	{
		#region Properties
		internal delegate void UpdateCallbackFunc();
		internal UpdateCallbackFunc UpdateCallback;

		internal AudioCom com;
		#endregion

		#region Constructors
		public Audio(DisposableI parent)
		: base(parent)
		{
			try
			{
				com = new AudioCom();
				var error = com.Init();

				switch (error)
				{
					case AudioErrors.XAudio2: Debug.ThrowError("Audio", "Failed to create XAudio2"); break;
					case AudioErrors.MasteringVoice: Debug.ThrowError("Audio", "Failed to create MasteringVoice"); break;
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
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
