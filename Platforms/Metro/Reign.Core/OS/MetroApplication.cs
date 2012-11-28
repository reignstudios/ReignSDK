using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI.Core;

namespace Reign.Core
{
	class MetroApplicationSource : IFrameworkViewSource
	{
		private MetroApplication application;

		public MetroApplicationSource(MetroApplication application)
		{
			this.application = application;
		}

		public IFrameworkView CreateView()
		{
			return application;
		}
	}

	public abstract class MetroApplication : IFrameworkView
    {
		#region Properties
		public delegate void BuyAppCallbackMethod(bool succeeded);
		public BuyAppCallbackMethod BuyAppCallback;

		private CoreMetroWindow coreMetroWindow;
		internal MetroApplicationSource source;
		private Application application;
		protected ApplicationEvent theEvent;
		private bool running, visible;
		protected SuspendingDeferral deferral;
		#endregion

		#region Constructors
		public MetroApplication()
		{
			source = new MetroApplicationSource(this);
		}

		protected void setApplication(Application application)
		{
			this.application = application;
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

		#region Methods
		public void SetWindow(CoreWindow window)
		{
			if (OS.CoreWindow != window) window.VisibilityChanged += visibilityChanged;

			OS.CoreWindow = window;
			if (coreMetroWindow != null) coreMetroWindow.Dispose();
			coreMetroWindow = new CoreMetroWindow((ApplicationI)this, window, theEvent);
		}

		public void Load(string entryPoint)
		{
			
		}

		public void Run()
		{
			var coreWindow = CoreWindow.GetForCurrentThread();
			application.frameSize = new Size2(coreMetroWindow.ConvertDipsToPixels(coreWindow.Bounds.Width), coreMetroWindow.ConvertDipsToPixels(coreWindow.Bounds.Height));
			application.shown();

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
				if (visible) application.resume();
				else application.pause();
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
			application.closing();
		}

		private void resuming(object sender, object e)
		{
			application.shown();
			running = true;
		}

		public void ShowCursor()
		{
			coreMetroWindow.ShowCursor();
		}

		public void HideCursor()
		{
			coreMetroWindow.HideCursor();
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
		#endregion
    }
}
