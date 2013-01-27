using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI.Core;

namespace Reign.Core
{
	class CoreWindowApplicationSource : IFrameworkViewSource
	{
		private CoreWindowApplication application;

		public CoreWindowApplicationSource(CoreWindowApplication application)
		{
			this.application = application;
		}

		public IFrameworkView CreateView()
		{
			return application;
		}
	}

	public abstract class CoreWindowApplication : IFrameworkView, ApplicationI
    {
		#region Properties
		public delegate void BuyAppCallbackMethod(bool succeeded);
		public BuyAppCallbackMethod BuyAppCallback;

		private CoreMetroWindow coreMetroWindow;
		internal CoreWindowApplicationSource source;
		private bool running, visible;
		private SuspendingDeferral deferral;

		public bool IsSnapped {get; private set;}
		public Windows.UI.Core.CoreWindow CoreWindow {get; private set;}
		public ApplicationOrientations Orientation {get; private set;}
		public Size2 FrameSize {get; private set;}
		public new bool Closed {get; private set;}

		public event ApplicationHandleEventMethod HandleEvent ;
		public event ApplicationStateMethod PauseCallback, ResumeCallback;

		private ApplicationEvent theEvent;
		#endregion

		#region Constructors
		public void Init(ApplicationDesc desc)
		{
			theEvent = new ApplicationEvent();
			source = new CoreWindowApplicationSource(this);
		}

		public void Initialize(CoreApplicationView applicationView)
		{
			applicationView.Activated += activated;
			CoreApplication.Suspending += suspending;
			CoreApplication.Resuming += resuming;
		}

		public void Uninitialize()
		{
			
		}
		#endregion

		#region Methods Events
		public void SetWindow(CoreWindow window)
		{
			if (CoreWindow != window) window.VisibilityChanged += visibilityChanged;

			CoreWindow = window;
			if (coreMetroWindow != null) coreMetroWindow.Dispose();
			coreMetroWindow = new CoreMetroWindow(this, window, theEvent, true);
		}

		public void Load(string entryPoint)
		{
			
		}

		public void Run()
		{
			var coreWindow = CoreWindow.GetForCurrentThread();
			FrameSize = new Size2(coreMetroWindow.ConvertDipsToPixels(coreWindow.Bounds.Width), coreMetroWindow.ConvertDipsToPixels(coreWindow.Bounds.Height));
			Shown();

			running = true;
			while (running)
			{
				if (visible)
				{
					CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
					OS.UpdateAndRender();
				}
				else
				{
					CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessOneAndAllPending);
				}
			}
		}

		private void visibilityChanged(CoreWindow sender, VisibilityChangedEventArgs args)
		{
			visible = args.Visible;

			if (running)
			{
				if (visible) Resume();
				else Pause();
			}
		}

		private void activated(CoreApplicationView sender, Windows.ApplicationModel.Activation.IActivatedEventArgs args)
		{
			CoreWindow.GetForCurrentThread().Activate();
		}

		private void suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
		{
			running = false;
			deferral = e.SuspendingOperation.GetDeferral();
			Closing();
		}

		private void resuming(object sender, object e)
		{
			Shown();
			running = true;
		}

		public bool IsTrial()
		{
			return coreMetroWindow.IsTrial();
		}

		public bool InAppPurchased(string appID)
		{
			return coreMetroWindow.InAppPurchased(appID);
		}

		public async void BuyInAppItem(string appID)
		{
			if (BuyAppCallback != null) BuyAppCallback(await coreMetroWindow.BuyInAppItem(appID));
			else Debug.ThrowError("MetroApplication", "BuyAppCallback method cannot be null");
		}

		internal void updateFrameSize(Size2 size, bool isSnapped)
		{
			FrameSize = size;
			IsSnapped = isSnapped;
		}

		internal void handleEvent(ApplicationEvent theEvent)
		{
			if (HandleEvent != null) HandleEvent(theEvent);
		}
		#endregion

		#region Methods
		public virtual void Shown()
		{
			
		}

		public virtual void Closing()
		{
			
		}

		public void Close()
		{
			
		}

		public virtual void Update(Time time)
		{
			
		}

		public virtual void Render(Time time)
		{
			
		}

		public virtual void Pause()
		{
			if (PauseCallback != null) PauseCallback();
		}

		public virtual void Resume()
		{
			if (ResumeCallback != null) ResumeCallback();
		}

		public void ShowCursor()
		{
			coreMetroWindow.ShowCursor();
		}

		public void HideCursor()
		{
			coreMetroWindow.HideCursor();
		}
		#endregion
    }
}
