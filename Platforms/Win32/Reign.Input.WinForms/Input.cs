using Reign.Core;

namespace Reign.Input.WinForms
{
	public class Input : DisposableResource, IInput
	{
		#region Properties
		internal delegate void UpdateCallbackMethod();
		internal UpdateCallbackMethod UpdateCallback;
		
		internal delegate void UpdateEventCallbackMethod(ApplicationEvent theEvent);
		internal UpdateEventCallbackMethod UpdateEventCallback;

		internal IApplication application;
		#endregion

		#region Constructors
		public Input(IDisposableResource parent, IApplication application)
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
