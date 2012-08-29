using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
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
		internal MetroApplicationSource source;
		private Application application;
		private bool running, visible;
		public CoreWindow CoreWindow {get; private set;}
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
			window.SizeChanged += sizeChanged;
			window.VisibilityChanged += visibilityChanged;
			window.Closed += closed;
			//window.PointerPressed += window_PointerPressed;
			//window.PointerReleased += window_PointerReleased;
			window.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
		}

		public void Load(string entryPoint)
		{
			
		}

		public void Run()
		{
			var coreWindow = CoreWindow.GetForCurrentThread();
			CoreWindow = coreWindow;
			application.frameSize = new Size2((int)coreWindow.Bounds.Width, (int)coreWindow.Bounds.Height);
			application.shown();

			running = true;
			while (running)
			{
				if (visible)
				{
					CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
					application.UpdateAndRender();
				}
				else
				{
					CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessOneAndAllPending);
				}
			}
		}

		private void sizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
		{
			application.frameSize = new Size2((int)args.Size.Width, (int)args.Size.Height);
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

		private void closed(CoreWindow sender, CoreWindowEventArgs args)
		{
			
		}

		private void activated(CoreApplicationView sender, Windows.ApplicationModel.Activation.IActivatedEventArgs args)
		{
			CoreWindow.GetForCurrentThread().Activate();
		}

		private void suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			var task = new Task(new Action(delegate
			{
				// Insert suspend code here !!!
				deferral.Complete();
			}));
		}

		private void resuming(object sender, object e)
		{
			
		}
		#endregion
    }
}
