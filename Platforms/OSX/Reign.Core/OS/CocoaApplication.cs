using System;
using System.Drawing;
using MonoMac.AppKit;

namespace Reign.Core
{
	public abstract class CocoaApplication : NSWindow, ApplicationI
	{
		#region Properties
		public NSView View {get; private set;}
		public ApplicationOrientations Orientation {get; private set;}
		
		private Size2 lastFrameSize;
		public Size2 FrameSize
		{
			get
			{
				var frame = View.Frame;
				var newFrameSize = new Size2((int)frame.Width, (int)frame.Height);
				// TODO: View Content for GL is not resizing correctly or something.
				return newFrameSize;
			}
		}
		
		public bool Closed {get; private set;}
		
		public event ApplicationHandleEventMethod HandleEvent;
		public event ApplicationStateMethod PauseCallback, ResumeCallback;
		
		private ApplicationEvent theEvent;
		#endregion
	
		#region Constructors
		public CocoaApplication()
		: base(new RectangleF(0, 0, 512, 512), NSWindowStyle.Borderless, NSBackingStore.Buffered, true)
		{
			
		}
		
		public void Init(ApplicationDesc desc)
		{
			theEvent = new ApplicationEvent();
			
			var frame = desc.FrameSize;
			if (frame.Width == 0 || frame.Height == 0) frame = (OS.ScreenSize.ToVector2() / 1.5f).ToSize();
			SetContentSize(new SizeF(frame.Width, frame.Height));
			lastFrameSize = frame;
			
			AcceptsMouseMovedEvents = true;
			View = new NSView();
			switch (desc.StartPosition)
			{
				case (ApplicationStartPositions.CenterCurrentScreen): Center(); break;
			}
			
			switch (desc.Type)
			{
				case (ApplicationTypes.Box): StyleMask = NSWindowStyle.Borderless; break;
				case (ApplicationTypes.Frame): StyleMask = NSWindowStyle.Titled | NSWindowStyle.Miniaturizable | NSWindowStyle.Closable; break;
				case (ApplicationTypes.FrameSizable): StyleMask = NSWindowStyle.Titled | NSWindowStyle.Resizable | NSWindowStyle.Miniaturizable | NSWindowStyle.Closable; break;
				default: Debug.ThrowError("NSWindow", "Unsuported window type"); break;
			}
			
			Title = desc.Name;
			ContentView = View;
			WillClose += closingEvent;
		}
		#endregion
		
		#region Method Events
		internal void show()
		{
			MakeKeyAndOrderFront(NSApplication.SharedApplication);
			Shown();
		}
		
		private void closingEvent(object sender, EventArgs e)
		{
			Closing();
		}
		
		private void handleEvent(ApplicationEvent applicationEvent)
		{
			if (HandleEvent != null) HandleEvent(applicationEvent);
		}
		
		public override void SendEvent(NSEvent e)
		{
			switch (e.Type)
			{
				case (NSEventType.KeyDown):
					theEvent.Type = ApplicationEventTypes.KeyDown;
					theEvent.KeyCode = e.KeyCode;
					break;
					
				case (NSEventType.KeyUp):
					theEvent.Type = ApplicationEventTypes.KeyUp;
					theEvent.KeyCode = e.KeyCode;
					break;
					
				case (NSEventType.FlagsChanged):
					theEvent.Type = ApplicationEventTypes.Unkown;
					theEvent.KeyCode = e.KeyCode;
					break;
					
				default:
					base.SendEvent(e);
					return;
			}
			
			handleEvent(theEvent);
		}
		
		public override void MouseMoved (NSEvent e)
		{
			mouseMoved(e);
		}
		
		public override void MouseDragged (NSEvent e)
		{
			mouseMoved(e);
		}
		
		public override void OtherMouseDragged (NSEvent e)
		{
			mouseMoved(e);
		}
		
		public override void RightMouseDragged (NSEvent e)
		{
			mouseMoved(e);
		}
		
		private void mouseMoved(NSEvent e)
		{
			theEvent.Type = ApplicationEventTypes.MouseMove;
			var pos = e.LocationInWindow;
			theEvent.CursorPosition.X = (int)pos.X;
			theEvent.CursorPosition.Y = (int)pos.Y;
			
			handleEvent(theEvent);
		}
		
		public override void MouseDown(NSEvent e)
		{
			switch (e.Type)
			{
				case (NSEventType.LeftMouseDown): theEvent.Type = ApplicationEventTypes.LeftMouseDown; break;
				case (NSEventType.OtherMouseDown): theEvent.Type = ApplicationEventTypes.MiddleMouseDown; break;
				case (NSEventType.RightMouseDown): theEvent.Type = ApplicationEventTypes.RightMouseDown; break;
			}
			
			var pos = e.LocationInWindow;
			theEvent.CursorPosition.X = (int)pos.X;
			theEvent.CursorPosition.Y = (int)pos.Y;
			
			handleEvent(theEvent);
		}
		
		public override void MouseUp(NSEvent e)
		{
			switch (e.Type)
			{
				case (NSEventType.LeftMouseUp): theEvent.Type = ApplicationEventTypes.LeftMouseUp; break;
				case (NSEventType.OtherMouseUp): theEvent.Type = ApplicationEventTypes.MiddleMouseUp; break;
				case (NSEventType.RightMouseUp): theEvent.Type = ApplicationEventTypes.RightMouseUp; break;
			}
			
			var pos = e.LocationInWindow;
			theEvent.CursorPosition.X = (int)pos.X;
			theEvent.CursorPosition.Y = (int)pos.Y;

			handleEvent(theEvent);
		}
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
			if (Closed) return;
			Closed = true;
			
			Closing();
			base.Close();
			Dispose();
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

