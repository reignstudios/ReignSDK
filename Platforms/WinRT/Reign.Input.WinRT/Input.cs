using System;
using Reign.Core;

namespace Reign.Input.WinRT
{
	public class Input : Disposable, InputI
	{
		#region Properties
		internal delegate void UpdateCallbackMethod();
		internal UpdateCallbackMethod UpdateCallback;
		
		internal delegate void UpdateEventCallbackMethod(ApplicationEvent theEvent);
		internal UpdateEventCallbackMethod UpdateEventCallback;

		internal ApplicationI applicationI;
		private ApplicationI application;
		private XAMLApplication pageApplication;
		#endregion

		#region Constructors
		public Input(DisposableI parent, ApplicationI application)
		: base(parent)
		{
			this.applicationI = application;
			this.application = application;
			application.HandleEvent += updateEvent;
		}

		public Input(DisposableI parent, XAMLApplication application)
		: base(parent)
		{
			this.applicationI = application;
			this.pageApplication = application;
			application.HandleEvent += updateEvent;
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (application != null) application.HandleEvent -= updateEvent;
			if (pageApplication != null) pageApplication.HandleEvent -= updateEvent;
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
