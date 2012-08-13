using System;

namespace Reign.Core
{
	public class XWindow
	{
		#region Properties
		public IntPtr Handle {get; private set;}
		private IntPtr dc;
		public IntPtr DC {get{return dc;}}
		private int sc;
		private X11.XEvent xEvent;
		private int width, height;
		private bool windowShown;
		
		private Window window;
		protected WindowEvent theEvent;
		#endregion
	
		#region Constructors
		protected void init(Window window, string name, int width, int height, WindowStartPositions startPosition, WindowTypes type)
		{
			this.window = window;
			this.width = width;
			this.height = height;
			
			X11.XInitThreads();
			dc = X11.XOpenDisplay(IntPtr.Zero);
			if (dc == IntPtr.Zero)
			{
				Debug.ThrowError("Window", "Cannot open Display");
			}
			
			sc = X11.XDefaultScreen(dc);
			Handle = X11.XCreateSimpleWindow(dc, X11.XRootWindow(dc, sc), 0, 0, (uint)width, (uint)height, 0, X11.XBlackPixel(dc, sc), X11.XWhitePixel(dc, sc));
			
			X11.XSelectInput(dc, Handle, X11.ExposureMask | X11.KeyPressMask | X11.KeyReleaseMask | X11.ButtonPressMask | X11.ButtonReleaseMask);
			
			// Enable Capture of close box
			var normalHint = X11.XInternAtom(dc, "WM_NORMAL_HINTS", false);
			var deleteHint = X11.XInternAtom(dc, "WM_DELETE_WINDOW", false);
			X11.XSetWMProtocols(dc, Handle, new IntPtr[]{normalHint, deleteHint}, 2);
			X11.XStoreName(dc, Handle, name);
			
			// Size
			if (type == WindowTypes.Frame || type == WindowTypes.Box)
			{
				unsafe
				{
					var sizeHints = new X11.XSizeHints();
					
					sizeHints.flags = (IntPtr)(X11.XSizeHintsFlags.PPosition | X11.XSizeHintsFlags.PMinSize | X11.XSizeHintsFlags.PMaxSize);
					sizeHints.min_width  = sizeHints.max_width  = width;
					sizeHints.min_height = sizeHints.max_height = height;
					
					X11.XSetNormalHints(dc, Handle, &sizeHints);
				}
			}
			
			// Position
			if (startPosition == WindowStartPositions.CenterCurrentScreen)
			{
				var screenSize = OS.ScreenSize;
				X11.XMoveWindow(dc, Handle, (screenSize.Width-width) / 2, (screenSize.Height-height) / 2);
			}
			
		}
		#endregion
		
		#region Methods
		internal void updateWindowEvents()
		{
			while (X11.XPending(dc) != 0)
			{
				X11.XPeekEvent(dc, ref xEvent);
				
				int keyCode = (int)xEvent.xkey.keycode;
				switch (xEvent.type)
				{
					case (X11.Expose):
						if (windowShown)
						{
							theEvent.Type = WindowEventTypes.Scaled;
						}
						else
						{
							theEvent.Type = WindowEventTypes.Unkown;
							windowShown = true;
						}
						break;
						
					case (X11.ClientMessage):
						theEvent.Type = WindowEventTypes.Closed;
						Close();
						return;
						
					case (X11.KeyPress):
						theEvent.Type = WindowEventTypes.KeyDown;
						theEvent.KeyCode = keyCode;
						break;
						
					case (X11.KeyRelease):
						theEvent.Type = WindowEventTypes.KeyUp;
						theEvent.KeyCode = keyCode;
						break;
						
					case (X11.ButtonPress):
						if (keyCode == 1) theEvent.Type = WindowEventTypes.LeftMouseDown;
						if (keyCode == 2) theEvent.Type = WindowEventTypes.MiddleMouseDown;
						if (keyCode == 3) theEvent.Type = WindowEventTypes.RightMouseDown;
						if (keyCode == 4)
						{
							theEvent.Type = WindowEventTypes.ScrollWheel;
							theEvent.ScrollWheelVelocity = 1;
						}
						if (keyCode == 5)
						{
							theEvent.Type = WindowEventTypes.ScrollWheel;
							theEvent.ScrollWheelVelocity = -1;
						}
						theEvent.KeyCode = keyCode;
						break;
						
					case (X11.ButtonRelease):
						if (keyCode == 1) theEvent.Type = WindowEventTypes.LeftMouseUp;
						if (keyCode == 2) theEvent.Type = WindowEventTypes.MiddleMouseUp;
						if (keyCode == 3) theEvent.Type = WindowEventTypes.RightMouseUp;
						if (keyCode == 4)
						{
							theEvent.Type = WindowEventTypes.ScrollWheel;
							theEvent.ScrollWheelVelocity = 1;
						}
						if (keyCode == 5)
						{
							theEvent.Type = WindowEventTypes.ScrollWheel;
							theEvent.ScrollWheelVelocity = -1;
						}
						theEvent.KeyCode = keyCode;
						break;
				}
				
				X11.XNextEvent(dc, ref xEvent);
				
				window.handleEvent(theEvent);
			}
		}
		
		public virtual void Show()
		{
			X11.XMapWindow(dc, Handle);
			X11.XFlush(dc);
			window.shown();
		}
		
		public virtual void Close()
		{
			window.closed = true;
			window.closing();
			X11.XDestroyWindow(dc, Handle);
			X11.XCloseDisplay(dc);
		}
		#endregion
	}
}

