using System;

namespace Reign.Core
{
	public enum WindowEventTypes
	{
		Unkown,
		Move,
		Scaled,
		Closed,
		KeyDown,
		KeyUp,
		MouseMove,
		LeftMouseDown,
		LeftMouseUp,
		MiddleMouseDown,
		MiddleMouseUp,
		RightMouseDown,
		RightMouseUp,
		ScrollWheel
	}

	public class WindowEvent
	{
		public WindowEventTypes Type;
		public int KeyCode;
		public float ScrollWheelVelocity;
		public Point CursorLocation;
	}
	
	#if !NaCl
	public enum WindowTypes
	{
		Box,
		Frame,
		FrameSizable
	}

	public enum WindowStartPositions
	{
		Default,
		CenterCurrentScreen
	}
	#endif

	public class Window
	#if WINDOWS
	: WinForm
	#elif OSX
	: NSWindowEx
	#elif LINUX
	: XWindow
	#elif NaCl
	: NaClWindow
	#endif
	{
		#region Properties
		public delegate void HandleEventMethod(WindowEvent theEvent);
		public HandleEventMethod HandleEvent;
		
		#if LINUX || OSX || NaCl
		internal bool closed;
		#endif
		public new bool Closed
		{
			get
			{
				#if WINDOWS
				return IsDisposed;
				#endif
				
				#if LINUX || OSX || NaCl
				return closed;
				#endif
			}
		}

		public new Point Location
		{
			get
			{
				#if WINDOWS
				var loc = base.Location;
				return new Point(loc.X, loc.Y);
				#endif
				
				#if LINUX
				throw new NotImplementedException();
				return new Point();
				#endif
				
				#if OSX
				var frame = Frame;
				return new Point((int)frame.X, (int)frame.Y);
				#endif
				
				#if NaCl
				return new Point();
				#endif
			}
		}
		
		
		public Point ViewLocation
		{
			get
			{
				#if WINDOWS
				var point = PointToScreen(new System.Drawing.Point(0, 0));
				return new Point(point.X, point.Y);
				#endif
				
				#if LINUX
				throw new NotImplementedException();
				return new Point();
				#endif
			
				#if OSX || NaCl
				return Location;
				#endif
			}
		}

		#if NaCl
		internal Size2 frameSize;
		#endif
		public Size2 FrameSize
		{
			get
			{
				#if WINDOWS
				var frame = ClientSize;
				return new Size2(frame.Width, frame.Height);
				#endif

				#if LINUX
				var attrb = new X11.XWindowAttributes();
				X11.XGetWindowAttributes(DC, Handle, out attrb);
				return new Size2(attrb.width, attrb.height);
				#endif

				#if OSX
				var frame = View.Frame;
				return new Size2((int)frame.Width, (int)frame.Height);
				#endif
				
				#if NaCl
				return frameSize;
				#endif
			}
			set
			{
				#if WINDOWS
				ClientSize = new System.Drawing.Size(value.Width, value.Height);
				#endif
			}
		}

		public Size2 ScreenSize
		{
			get
			{
				#if WINDOWS
				var screen = System.Windows.Forms.Screen.FromControl(this);
				return new Size2(screen.Bounds.Width, screen.Bounds.Height);
				#endif

				#if LINUX
				throw new NotImplementedException();
				return new Size2();
				#endif

				#if OSX
				throw new NotImplementedException();
				return new Size2();
				#endif
				
				#if NaCl
				throw new NotImplementedException();
				return new Size2();
				#endif
			}
		}
		#endregion

		#region Constructors
		#if NaCl
		public Window(string name, int width, int height)
		#else
		public Window(string name, int width, int height, WindowStartPositions startPosition, WindowTypes type)
		#endif
		#if OSX
		: base(width, height, type)
		#endif
		{
			try
			{
				theEvent = new WindowEvent();
				#if NaCl
				init(this, name, width, height);
				#else
				init(this, name, width, height, startPosition, type);
				#endif
			}
			catch (Exception e)
			{
				Close();
				throw e;
			}
		}
		#endregion

		#region Methods
		protected internal virtual void update(Time time)
		{
			
		}

		protected internal virtual void render(Time time)
		{
			
		}
		
		protected internal virtual void handleEvent(WindowEvent theEvent)
		{
			if (HandleEvent != null) HandleEvent(theEvent);
		}
		
		protected internal virtual void shown()
		{
			
		}
		
		public override void Show()
		{
			base.Show();
		}
		
		protected internal virtual void closing()
		{
			#if !WINDOWS
			closed = true;
			#endif
		}

		public override void Close()
		{
			if (Closed) return;
			#if !WINDOWS
			closed = true;
			#endif
			
			base.Close ();
		}
		#endregion
	}
}