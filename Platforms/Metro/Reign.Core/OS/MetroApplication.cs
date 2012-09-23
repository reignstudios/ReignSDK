using System;
using System.Threading.Tasks;
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
		internal MetroApplicationSource source;
		private Application application;
		protected ApplicationEvent theEvent;
		private bool running, visible, leftPointerOn, middlePointerOn, rightPointerOn;
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
			
			window.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
			window.PointerMoved += pointerMoved;
			window.PointerPressed += pointerPressed;
			window.PointerReleased += pointerReleased;
			window.PointerWheelChanged += pointerWheelChanged;
			window.KeyDown += keyDown;
			window.KeyUp += keyUp;
		}

		private void pointerMoved(CoreWindow sender, PointerEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.MouseMove;
			var loc = e.CurrentPoint.RawPosition;
			theEvent.CursorLocation = new Point((int)loc.X, (int)loc.Y);
			application.handleEvent(theEvent);
		}

		private void pointerPressed(CoreWindow sender, PointerEventArgs e)
		{
			if (e.CurrentPoint.Properties.IsLeftButtonPressed)
			{
				theEvent.Type = ApplicationEventTypes.LeftMouseDown;
				leftPointerOn = true;
			}
			else if (e.CurrentPoint.Properties.IsMiddleButtonPressed)
			{
				theEvent.Type = ApplicationEventTypes.MiddleMouseDown;
				middlePointerOn = true;
			}
			else if (e.CurrentPoint.Properties.IsRightButtonPressed)
			{
				theEvent.Type = ApplicationEventTypes.RightMouseDown;
				rightPointerOn = true;
			}
			
			var loc = e.CurrentPoint.RawPosition;
			theEvent.CursorLocation = new Point((int)loc.X, (int)loc.Y);
			application.handleEvent(theEvent);
		}

		private void pointerReleased(CoreWindow sender, PointerEventArgs e)
		{
			if (leftPointerOn) theEvent.Type = ApplicationEventTypes.LeftMouseUp;
			else if (middlePointerOn) theEvent.Type = ApplicationEventTypes.MiddleMouseUp;
			else if (rightPointerOn) theEvent.Type = ApplicationEventTypes.RightMouseUp;
			leftPointerOn = false;
			middlePointerOn = false;
			rightPointerOn = false;

			var loc = e.CurrentPoint.RawPosition;
			theEvent.CursorLocation = new Point((int)loc.X, (int)loc.Y);
			application.handleEvent(theEvent);
		}

		private void pointerWheelChanged(CoreWindow sender, PointerEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.ScrollWheel;
			var loc = e.CurrentPoint.RawPosition;
			theEvent.CursorLocation = new Point((int)loc.X, (int)loc.Y);
			application.handleEvent(theEvent);
		}

		private void keyDown(CoreWindow sender, KeyEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.KeyDown;
			theEvent.KeyCode = (int)e.KeyStatus.ScanCode;
			application.handleEvent(theEvent);
		}

		private void keyUp(CoreWindow sender, KeyEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.KeyUp;
			theEvent.KeyCode = (int)e.KeyStatus.ScanCode;
			application.handleEvent(theEvent);
		}

		public void Load(string entryPoint)
		{
			
		}

		public void Run()
		{
			var coreWindow = CoreWindow.GetForCurrentThread();
			CoreWindow = coreWindow;
			application.frameSize = new Size2(convertDipsToPixels(coreWindow.Bounds.Width), convertDipsToPixels(coreWindow.Bounds.Height));
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

		private int convertDipsToPixels(double dips)
		{
			return (int)(dips * DisplayProperties.LogicalDpi / 96f + .5f);
		}

		private void sizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
		{
			application.frameSize = new Size2(convertDipsToPixels(args.Size.Width), convertDipsToPixels(args.Size.Height));
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
