using System;
using Reign.Core;

namespace Reign.Audio.OpenAL
{
	public class Audio : Disposable, AudioI
	{
		#region Properties
		internal delegate void UpdateCallbackFunc();
		internal UpdateCallbackFunc UpdateCallback;
		
		private IntPtr device, context;
		#endregion

		#region Constructors
		public unsafe Audio(DisposableI parent)
		: base(parent)
		{
			device = AL.OpenDevice((byte*)0);
			context = AL.CreateContext(device, new int[]{});
			AL.MakeContextCurrent(context);
			
			AL.Listener3f(AL.POSITION, 0, 0, 0);
			AL.Listener3f(AL.VELOCITY, 0, 0, 0);
			AL.Listener3f(AL.ORIENTATION, 0, 0, -1);
		}
		
		public override void Dispose ()
		{
			disposeChilderen();
			if (context != IntPtr.Zero)
			{
				AL.DestroyContext(context);
				context = IntPtr.Zero;
			}
			if (device != IntPtr.Zero)
			{
				AL.CloseDevice(device);
				device = IntPtr.Zero;
			}
			base.Dispose ();
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