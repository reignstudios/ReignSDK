using System;
using Reign.Core;

namespace Reign.Input.Android
{
	public class Input : Disposable, InputI
	{
		#region Properties
		internal delegate void UpdateCallbackMethod();
		internal UpdateCallbackMethod UpdateCallback;
		
		internal delegate void UpdateEventCallbackMethod(ApplicationEvent theEvent);
		internal UpdateEventCallbackMethod UpdateEventCallback;
		
		internal Application application;
		#endregion
	
		#region Constructors
		public Input(DisposableI parent, Application application)
		: base(parent)
		{
			this.application = application;
			application.HandleEvent += updateEvent;
		}
		
		public override void Dispose()
		{
			disposeChilderen();
			application.HandleEvent -= updateEvent;
			base.Dispose();
		}
		#endregion
		
		#region Methods
		private void updateEvent(ApplicationEvent theEvent)
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

