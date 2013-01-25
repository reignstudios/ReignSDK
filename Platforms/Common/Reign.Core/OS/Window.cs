using System;

namespace Reign.Core
{
	public enum WindowEventTypes
	{
		Unkown,
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
		public Point2 CursorPosition;
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
	#if WIN32
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
				#if WIN32
				return IsDisposed;
				#endif
				
				#if LINUX || OSX || NaCl
				return closed;
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
				#if WIN32
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
				#if WIN32
				ClientSize = new System.Drawing.Size(value.Width, value.Height);
				#endif
			}
		}

		public Size2 MinFrameSize
		{
			get
			{
				#if WIN32
				var size = MinimumSize;
				return new Size2(size.Width, size.Height);
				#endif
				
				#if OSX || LINUX || NaCl
				return new Size2();
				#endif
			}
			set
			{
				#if WIN32
				MinimumSize = new System.Drawing.Size(value.Width, value.Height);
				#endif
			}
		}

		public Size2 MaxFrameSize
		{
			get
			{
				#if WIN32
				var size = MaximumSize;
				return new Size2(size.Width, size.Height);
				#endif
				
				#if OSX || LINUX || NaCl
				return new Size2();
				#endif
			}
			set
			{
				#if WIN32
				MaximumSize = new System.Drawing.Size(value.Width, value.Height);
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
			#if !WIN32
			closed = true;
			#endif
		}

		public override void Close()
		{
			if (Closed) return;
			#if !WIN32
			closed = true;
			#endif
			
			base.Close ();
		}

		public void HideCursor()
		{
			#if WIN32
			System.Windows.Forms.Cursor.Hide();
			#endif
		}

		public void ShowCursor()
		{
			#if WIN32
			System.Windows.Forms.Cursor.Show();
			#endif
		}
		#endregion
	}
}