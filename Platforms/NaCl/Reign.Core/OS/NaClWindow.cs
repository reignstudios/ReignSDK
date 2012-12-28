using System;

namespace Reign.Core
{
	public class NaClWindow
	{
		#region Properties
		private Window window;
		protected WindowEvent theEvent;
		#endregion
		
		#region Constructors
		protected void init(Window window, string name, int width, int height)
		{
			this.window = window;
			window.frameSize = new Size2(width, height);
		}
		#endregion
		
		#region Methods
		public virtual void Show()
		{
			window.shown();
		}
		
		public virtual void Close()
		{
			window.closing();
		}
		
		protected internal virtual void monoThreadUpdate()
		{
			
		}
		
		private void handleMouseMoveEvent(int cursorX, int cursorY)
		{
			theEvent.Type = WindowEventTypes.MouseMove;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			window.handleEvent(theEvent);
		}
		
		private void handleLeftMouseDownEvent(int cursorX, int cursorY)
		{
			theEvent.Type = WindowEventTypes.LeftMouseDown;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			window.handleEvent(theEvent);
		}
		
		private void handleMiddleMouseDownEvent(int cursorX, int cursorY)
		{
			theEvent.Type = WindowEventTypes.MiddleMouseDown;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			window.handleEvent(theEvent);
		}
		
		private void handleRightMouseDownEvent(int cursorX, int cursorY)
		{
			theEvent.Type = WindowEventTypes.RightMouseDown;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			window.handleEvent(theEvent);
		}
		
		private void handleLeftMouseUpEvent(int cursorX, int cursorY)
		{
			theEvent.Type = WindowEventTypes.LeftMouseUp;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			window.handleEvent(theEvent);
		}
		
		private void handleMiddleMouseUpEvent(int cursorX, int cursorY)
		{
			theEvent.Type = WindowEventTypes.MiddleMouseUp;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			window.handleEvent(theEvent);
		}
		
		private void handleRightMouseUpEvent(int cursorX, int cursorY)
		{
			theEvent.Type = WindowEventTypes.RightMouseUp;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			window.handleEvent(theEvent);
		}
		
		#region NaCl Callbacks
		private static void naclHandleMouseMoveEvent(int cursorX, int cursorY)
		{
			OS.CurrentWindow.handleMouseMoveEvent(cursorX, cursorY);
		}
		
		private static void naclHandleLeftMouseDownEvent(int cursorX, int cursorY)
		{
			OS.CurrentWindow.handleLeftMouseDownEvent(cursorX, cursorY);
		}
		
		private static void naclHandleMiddleMouseDownEvent(int cursorX, int cursorY)
		{
			OS.CurrentWindow.handleMiddleMouseDownEvent(cursorX, cursorY);
		}
		
		private static void naclHandleRightMouseDownEvent(int cursorX, int cursorY)
		{
			OS.CurrentWindow.handleRightMouseDownEvent(cursorX, cursorY);
		}
		
		private static void naclHandleLeftMouseUpEvent(int cursorX, int cursorY)
		{
			OS.CurrentWindow.handleLeftMouseUpEvent(cursorX, cursorY);
		}
		
		private static void naclHandleMiddleMouseUpEvent(int cursorX, int cursorY)
		{
			OS.CurrentWindow.handleMiddleMouseUpEvent(cursorX, cursorY);
		}
		
		private static void naclHandleRightMouseUpEvent(int cursorX, int cursorY)
		{
			OS.CurrentWindow.handleRightMouseUpEvent(cursorX, cursorY);
		}
		#endregion
		#endregion
	}
}

