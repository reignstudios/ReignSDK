using System;
using Windows.Graphics.Display;
using Windows.UI.Core;

namespace Reign.Core
{
	class CoreMetroWindow
	{
		private CoreWindow window;
		private ApplicationI application;
		private ApplicationEvent theEvent;
		private bool leftPointerOn, middlePointerOn, rightPointerOn;

		public CoreMetroWindow(ApplicationI application, CoreWindow window, ApplicationEvent theEvent)
		{
			this.window = window;
			this.application = application;
			this.theEvent = theEvent;

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
			
			window.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
			window.PointerMoved -= pointerMoved;
			window.PointerPressed -= pointerPressed;
			window.PointerReleased -= pointerReleased;
			window.PointerWheelChanged -= pointerWheelChanged;
			window.KeyDown -= keyDown;
			window.KeyUp -= keyUp;
		}

		private void pointerMoved(CoreWindow sender, PointerEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.MouseMove;
			var loc = e.CurrentPoint.RawPosition;
			theEvent.CursorLocation = new Point2((int)loc.X, (int)loc.Y);
			application.Metro_HandleEvent(theEvent);
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
			theEvent.CursorLocation = new Point2((int)loc.X, (int)loc.Y);
			application.Metro_HandleEvent(theEvent);
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
			theEvent.CursorLocation = new Point2((int)loc.X, (int)loc.Y);
			application.Metro_HandleEvent(theEvent);
		}

		private void pointerWheelChanged(CoreWindow sender, PointerEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.ScrollWheel;
			var loc = e.CurrentPoint.RawPosition;
			theEvent.CursorLocation = new Point2((int)loc.X, (int)loc.Y);
			application.Metro_HandleEvent(theEvent);
		}

		private void keyDown(CoreWindow sender, KeyEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.KeyDown;
			theEvent.KeyCode = (int)e.VirtualKey;
			application.Metro_HandleEvent(theEvent);
		}

		private void keyUp(CoreWindow sender, KeyEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.KeyUp;
			theEvent.KeyCode = (int)e.VirtualKey;
			application.Metro_HandleEvent(theEvent);
		}

		public int ConvertDipsToPixels(double dips)
		{
			return (int)(dips * DisplayProperties.LogicalDpi / 96f + .5f);
		}

		private void sizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
		{
			application.Metro_FrameSize = new Size2(ConvertDipsToPixels(args.Size.Width), ConvertDipsToPixels(args.Size.Height));
		}
	}
}
