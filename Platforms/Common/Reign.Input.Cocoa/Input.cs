using System;
using Reign.Core;

namespace Reign.Input.Cocoa
{
	public class Input : Disposable, InputI
	{
		#region Properties
		internal delegate void UpdateCallbackMethod();
		internal UpdateCallbackMethod UpdateCallback;
		
		#if OSX
		internal delegate void UpdateEventCallbackMethod(WindowEvent theEvent);
		#else
		internal delegate void UpdateEventCallbackMethod(ApplicationEvent theEvent);
		#endif
		internal UpdateEventCallbackMethod UpdateEventCallback;
		
		#if OSX
		internal Window window;
		#else
		internal Application application;
		#endif
		#endregion
	
		#region Constructors
		#if OSX
		public Input(RootDisposable rootDisposable, Window window)
		#else
		public Input(RootDisposable rootDisposable, Application application)
		#endif
		: base(rootDisposable)
		{
			#if OSX
			this.window = window;
			window.HandleEvent += updateEvent;
			#else
			this.application = application;
			application.HandleEvent += updateEvent;
			#endif
		}
		
		public override void Dispose()
		{
			disposeChilderen();
			#if OSX
			window.HandleEvent -= updateEvent;
			#else
			application.HandleEvent -= updateEvent;
			#endif
			base.Dispose();
		}
		#endregion
		
		#region Methods
		#if OSX
		private void updateEvent(WindowEvent theEvent)
		#else
		private void updateEvent(ApplicationEvent theEvent)
		#endif
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

