using System;

namespace Reign.Core
{
	public abstract class X11Application : ApplicationI
	{
		#region Properties
		public IntPtr Handle {get; private set;}
		private IntPtr dc;
		public IntPtr DC {get{return dc;}}
		private int sc;
		private X11.XEvent xEvent;
		private bool windowShown;
		
		public ApplicationOrientations Orientation {get; private set;}
		
		public Size2 FrameSize
		{
			get
			{
				var attrb = new X11.XWindowAttributes();
				X11.XGetWindowAttributes(DC, Handle, out attrb);
				return new Size2(attrb.width, attrb.height);
			}
		}
		
		public bool Closed {get; private set;}
		
		public event ApplicationHandleEventMethod HandleEvent;
		public event ApplicationStateMethod PauseCallback, ResumeCallback;
		
		private ApplicationEvent theEvent;
		#endregion
	
		#region Constructors
		public void Init(ApplicationDesc desc)
		{
			theEvent = new ApplicationEvent();
		
			var frame = desc.FrameSize;
			if (frame.Width == 0 || frame.Height == 0) frame = (OS.ScreenSize.ToVector2() / 1.5f).ToSize();
			
			X11.XInitThreads();
			dc = X11.XOpenDisplay(IntPtr.Zero);
			if (dc == IntPtr.Zero)
			{
				Debug.ThrowError("Window", "Cannot open Display");
			}
			
			sc = X11.XDefaultScreen(dc);
			Handle = X11.XCreateSimpleWindow(dc, X11.XRootWindow(dc, sc), 0, 0, (uint)frame.Width, (uint)frame.Height, 0, X11.XBlackPixel(dc, sc), X11.XWhitePixel(dc, sc));
			
			X11.XSelectInput(dc, Handle, X11.ExposureMask | X11.KeyPressMask | X11.KeyReleaseMask | X11.ButtonPressMask | X11.ButtonReleaseMask);
			
			// Enable Capture of close box
			var normalHint = X11.XInternAtom(dc, "WM_NORMAL_HINTS", false);
			var deleteHint = X11.XInternAtom(dc, "WM_DELETE_WINDOW", false);
			X11.XSetWMProtocols(dc, Handle, new IntPtr[]{normalHint, deleteHint}, 2);
			X11.XStoreName(dc, Handle, desc.Name);
			
			// Size
			if (desc.Type == ApplicationTypes.Frame || desc.Type == ApplicationTypes.FrameSizable || desc.Type == ApplicationTypes.Box)
			{
				unsafe
				{
					var sizeHints = new X11.XSizeHints();
					
					var flags = X11.XSizeHintsFlags.PPosition;
					if (desc.Type != ApplicationTypes.FrameSizable)
					{
						flags |= X11.XSizeHintsFlags.PMinSize | X11.XSizeHintsFlags.PMaxSize;
						sizeHints.min_width  = sizeHints.max_width  = frame.Width;
						sizeHints.min_height = sizeHints.max_height = frame.Height;
					}
					sizeHints.flags = (IntPtr)flags;
					
					X11.XSetNormalHints(dc, Handle, &sizeHints);
				}
			}
			
			// Position
			if (desc.StartPosition == ApplicationStartPositions.CenterCurrentScreen)
			{
				var screenSize = OS.ScreenSize;
				X11.XMoveWindow(dc, Handle, (screenSize.Width-frame.Width) / 2, (screenSize.Height-frame.Height) / 2);
			}
			
		}
		#endregion
		
		#region Method Events
		private void handleEvent(ApplicationEvent applicationEvent)
		{
			if (HandleEvent != null) HandleEvent(applicationEvent);
		}
		
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
							//theEvent.Type = ApplicationEventTypes.Scaled;
						}
						else
						{
							theEvent.Type = ApplicationEventTypes.Unkown;
							windowShown = true;
						}
						break;
						
					case (X11.ClientMessage):
					theEvent.Type = ApplicationEventTypes.Closed;
						Close();
						return;
						
					case (X11.KeyPress):
						theEvent.Type = ApplicationEventTypes.KeyDown;
						theEvent.KeyCode = keyCode;
						break;
						
					case (X11.KeyRelease):
						theEvent.Type = ApplicationEventTypes.KeyUp;
						theEvent.KeyCode = keyCode;
						break;
						
					case (X11.ButtonPress):
						if (keyCode == 1) theEvent.Type = ApplicationEventTypes.LeftMouseDown;
						if (keyCode == 2) theEvent.Type = ApplicationEventTypes.MiddleMouseDown;
						if (keyCode == 3) theEvent.Type = ApplicationEventTypes.RightMouseDown;
						if (keyCode == 4)
						{
							theEvent.Type = ApplicationEventTypes.ScrollWheel;
							theEvent.ScrollWheelVelocity = 1;
						}
						if (keyCode == 5)
						{
							theEvent.Type = ApplicationEventTypes.ScrollWheel;
							theEvent.ScrollWheelVelocity = -1;
						}
						theEvent.KeyCode = keyCode;
						break;
						
					case (X11.ButtonRelease):
						if (keyCode == 1) theEvent.Type = ApplicationEventTypes.LeftMouseUp;
						if (keyCode == 2) theEvent.Type = ApplicationEventTypes.MiddleMouseUp;
						if (keyCode == 3) theEvent.Type = ApplicationEventTypes.RightMouseUp;
						if (keyCode == 4)
						{
							theEvent.Type = ApplicationEventTypes.ScrollWheel;
							theEvent.ScrollWheelVelocity = 1;
						}
						if (keyCode == 5)
						{
							theEvent.Type = ApplicationEventTypes.ScrollWheel;
							theEvent.ScrollWheelVelocity = -1;
						}
						theEvent.KeyCode = keyCode;
						break;
				}
				
				X11.XNextEvent(dc, ref xEvent);
				
				handleEvent(theEvent);
			}
		}
		
		internal void show()
		{
			X11.XMapWindow(dc, Handle);
			X11.XFlush(dc);
			Shown();
		}
		#endregion
		
		#region Methods
		public virtual void Shown()
		{
			
		}
		
		public virtual void Closing()
		{
			
		}
		
		public virtual void Close()
		{
			if (Closed) return;
			Closed = true;
			
			Closing();
			X11.XDestroyWindow(dc, Handle);
			X11.XCloseDisplay(dc);
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
			
		}
		
		public void HideCursor()
		{
			
		}
		#endregion
	}
}

