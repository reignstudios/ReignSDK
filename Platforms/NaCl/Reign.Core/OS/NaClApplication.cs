using System;

namespace Reign.Core
{
	public abstract class NaClApplication : ApplicationI
	{
		#region Properties
		public static NaClApplication CurrentApplication {get; private set;}
		public ApplicationOrientations Orientation {get; private set;}
		
		public Size2 FrameSize {get; private set;}
		public bool Closed {get; private set;}
		
		public event ApplicationHandleEventMethod HandleEvent;
		public event ApplicationStateMethod PauseCallback, ResumeCallback;
		
		private ApplicationEvent theEvent;
		#endregion
		
		#region Constructors
		public void Init(ApplicationDesc desc)
		{
			OS.CurrentApplication = this;
			CurrentApplication = this;
			theEvent = new ApplicationEvent();
			
			if (desc.FrameSize.Width == 0 || desc.FrameSize.Height == 0) FrameSize = new Size2(512, 512);
			else FrameSize = desc.FrameSize;
		}
		#endregion
		
		#region Method Events
		internal void show()
		{
			Shown();
		}
		
		internal void dispose()
		{
			Closing();
		}
		
		private void handleEvent(ApplicationEvent applicationEvent)
		{
			if (HandleEvent != null) HandleEvent(applicationEvent);
		}
		
		protected internal virtual void monoThreadUpdate()
		{
			
		}
		
		private void handleMouseMoveEvent(int cursorX, int cursorY)
		{
			theEvent.Type = ApplicationEventTypes.MouseMove;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			handleEvent(theEvent);
		}
		
		private void handleLeftMouseDownEvent(int cursorX, int cursorY)
		{
			theEvent.Type = ApplicationEventTypes.LeftMouseDown;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			handleEvent(theEvent);
		}
		
		private void handleMiddleMouseDownEvent(int cursorX, int cursorY)
		{
			theEvent.Type = ApplicationEventTypes.MiddleMouseDown;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			handleEvent(theEvent);
		}
		
		private void handleRightMouseDownEvent(int cursorX, int cursorY)
		{
			theEvent.Type = ApplicationEventTypes.RightMouseDown;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			handleEvent(theEvent);
		}
		
		private void handleLeftMouseUpEvent(int cursorX, int cursorY)
		{
			theEvent.Type = ApplicationEventTypes.LeftMouseUp;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			handleEvent(theEvent);
		}
		
		private void handleMiddleMouseUpEvent(int cursorX, int cursorY)
		{
			theEvent.Type = ApplicationEventTypes.MiddleMouseUp;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			handleEvent(theEvent);
		}
		
		private void handleRightMouseUpEvent(int cursorX, int cursorY)
		{
			theEvent.Type = ApplicationEventTypes.RightMouseUp;
			theEvent.CursorPosition = new Point2(cursorX, cursorY);
			handleEvent(theEvent);
		}
		
		#region NaCl Callbacks
		private static void naclHandleMouseMoveEvent(int cursorX, int cursorY)
		{
			CurrentApplication.handleMouseMoveEvent(cursorX, cursorY);
		}
		
		private static void naclHandleLeftMouseDownEvent(int cursorX, int cursorY)
		{
			CurrentApplication.handleLeftMouseDownEvent(cursorX, cursorY);
		}
		
		private static void naclHandleMiddleMouseDownEvent(int cursorX, int cursorY)
		{
			CurrentApplication.handleMiddleMouseDownEvent(cursorX, cursorY);
		}
		
		private static void naclHandleRightMouseDownEvent(int cursorX, int cursorY)
		{
			CurrentApplication.handleRightMouseDownEvent(cursorX, cursorY);
		}
		
		private static void naclHandleLeftMouseUpEvent(int cursorX, int cursorY)
		{
			CurrentApplication.handleLeftMouseUpEvent(cursorX, cursorY);
		}
		
		private static void naclHandleMiddleMouseUpEvent(int cursorX, int cursorY)
		{
			CurrentApplication.handleMiddleMouseUpEvent(cursorX, cursorY);
		}
		
		private static void naclHandleRightMouseUpEvent(int cursorX, int cursorY)
		{
			CurrentApplication.handleRightMouseUpEvent(cursorX, cursorY);
		}
		#endregion
		#endregion
		
		#region Methods
		public virtual void Shown()
		{
			
		}
		
		public virtual void Closing()
		{
			
		}
		
		public new virtual void Close()
		{
			
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

