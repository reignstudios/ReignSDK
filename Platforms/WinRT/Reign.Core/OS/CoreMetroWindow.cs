using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace Reign.Core
{
	class CoreMetroWindow
	{
		#region Properties
		private CoreWindow window;
		private ApplicationI application;
		private ApplicationEvent theEvent;
		private bool leftPointerOn, middlePointerOn, rightPointerOn;
		private bool coreApp;
		#endregion

		#region Constructors
		public CoreMetroWindow(ApplicationI application, CoreWindow window, ApplicationEvent theEvent, bool coreApp)
		{
			this.window = window;
			this.application = application;
			this.theEvent = theEvent;
			this.coreApp = coreApp;

			window.SizeChanged += sizeChanged;
			
			window.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
			window.PointerMoved += pointerMoved;
			window.PointerPressed += pointerPressed;
			window.PointerReleased += pointerReleased;
			window.PointerWheelChanged += pointerWheelChanged;
			window.KeyDown += keyDown;
			window.KeyUp += keyUp;
		}

		public void Dispose()
		{
			window.SizeChanged -= sizeChanged;
			
			window.PointerMoved -= pointerMoved;
			window.PointerPressed -= pointerPressed;
			window.PointerReleased -= pointerReleased;
			window.PointerWheelChanged -= pointerWheelChanged;
			window.KeyDown -= keyDown;
			window.KeyUp -= keyUp;
		}
		#endregion

		#region Methods
		private void handleEvent(ApplicationEvent theEvent)
		{
			if (coreApp) ((CoreWindowApplication)application).handleEvent(theEvent);
			else ((XAMLApplication)application).handleEvent(theEvent);
		}

		public void ShowCursor()
		{
			window.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
		}

		public void HideCursor()
		{
			window.PointerCursor = null;
		}

		private void pointerMoved(CoreWindow sender, PointerEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.MouseMove;
			var loc = e.CurrentPoint.RawPosition;
			theEvent.CursorPosition = new Point2((int)loc.X, (int)loc.Y);
			handleEvent(theEvent);
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
			theEvent.CursorPosition = new Point2((int)loc.X, (int)loc.Y);
			handleEvent(theEvent);
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
			theEvent.CursorPosition = new Point2((int)loc.X, (int)loc.Y);
			handleEvent(theEvent);
		}

		private void pointerWheelChanged(CoreWindow sender, PointerEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.ScrollWheel;
			var loc = e.CurrentPoint.RawPosition;
			theEvent.CursorPosition = new Point2((int)loc.X, (int)loc.Y);
			handleEvent(theEvent);
		}

		private void keyDown(CoreWindow sender, KeyEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.KeyDown;
			theEvent.KeyCode = (int)e.VirtualKey;
			handleEvent(theEvent);
		}

		private void keyUp(CoreWindow sender, KeyEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.KeyUp;
			theEvent.KeyCode = (int)e.VirtualKey;
			handleEvent(theEvent);
		}

		public int ConvertDipsToPixels(double dips)
		{
			return (int)(dips * DisplayProperties.LogicalDpi / 96f + .5f);
		}

		private void sizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
		{
			if (coreApp) ((CoreWindowApplication)application).updateFrameSize(new Size2(ConvertDipsToPixels(args.Size.Width), ConvertDipsToPixels(args.Size.Height)), ApplicationView.Value == ApplicationViewState.Snapped);
			else ((XAMLApplication)application).updateFrameSize(new Size2(ConvertDipsToPixels(args.Size.Width), ConvertDipsToPixels(args.Size.Height)), ApplicationView.Value == ApplicationViewState.Snapped);
		}

		public bool IsTrial()
		{
			#if DEBUG
			return CurrentAppSimulator.LicenseInformation.IsTrial;
			#else
			return CurrentApp.LicenseInformation.IsTrial;
			#endif
		}

		public bool InAppPurchased(string appID)
		{
			#if DEBUG
			return CurrentAppSimulator.LicenseInformation.ProductLicenses[appID].IsActive;
			#else
			return CurrentApp.LicenseInformation.ProductLicenses[appID].IsActive;
			#endif
		}

		public async Task<bool> BuyInAppItem(string appID)
		{
			#if DEBUG
			await CurrentAppSimulator.RequestProductPurchaseAsync(appID, false);
			return CurrentAppSimulator.LicenseInformation.ProductLicenses[appID].IsActive;
			#else
			await CurrentApp.RequestProductPurchaseAsync(appID, false);
			return CurrentApp.LicenseInformation.ProductLicenses[appID].IsActive;
			#endif
		}
		#endregion
	}
}
