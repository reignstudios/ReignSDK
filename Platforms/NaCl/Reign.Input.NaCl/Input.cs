using System;
using Reign.Core;

namespace Reign.Input.NaCl
{
	public class Input : Disposable, InputI
	{
		#region Properties
		internal delegate void UpdateCallbackMethod();
		internal UpdateCallbackMethod UpdateCallback;
		
		internal delegate void UpdateEventCallbackMethod(WindowEvent theEvent);
		internal UpdateEventCallbackMethod UpdateEventCallback;
		
		internal Window window;
		#endregion
	
		#region Constructors
		public Input(RootDisposable rootDisposable, Window window)
		: base(rootDisposable)
		{
			this.window = window;
			window.HandleEvent += updateEvent;
		}
		
		public override void Dispose()
		{
			disposeChilderen();
			window.HandleEvent -= updateEvent;
			base.Dispose();
		}
		#endregion
		
		#region Methods
		private void updateEvent(WindowEvent theEvent)
		{
			if (theEvent == null) return;
			if (UpdateEventCallback != null) UpdateEventCallback(theEvent);
		}
		
		public void Update()
		{
			if (UpdateCallback != null) UpdateCallback();
		}
		#endregion
	}
}
