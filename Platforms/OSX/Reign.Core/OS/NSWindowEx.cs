using System;
using System.Drawing;
using MonoMac.AppKit;

namespace Reign.Core
{
	public class NSWindowEx : NSWindow
	{
		#region Properties
		private Window window;
		public NSView View {get; private set;}
		protected WindowEvent theEvent;
		#endregion
	
		#region Constructors
		public NSWindowEx(int width, int height, WindowTypes type)
		: base(new RectangleF(0, 0, width, height), getStyle(type), NSBackingStore.Buffered, true)
		{
			
		}
		
		private static NSWindowStyle getStyle(WindowTypes type)
		{
			var style = NSWindowStyle.Borderless;
			switch (type)
			{
				case (WindowTypes.Box): style = NSWindowStyle.Borderless; break;
				case (WindowTypes.Frame): style = NSWindowStyle.Titled | NSWindowStyle.Miniaturizable | NSWindowStyle.Closable; break;
				case (WindowTypes.FrameSizable): style = NSWindowStyle.Resizable | NSWindowStyle.Miniaturizable | NSWindowStyle.Closable; break;
				default: Debug.ThrowError("NSWindow", "Unsuported window type"); break;
			}
			
			return style;
		}
		
		protected void init(Window window, string name, int width, int height, WindowStartPositions startPosition, WindowTypes type)
		{
			this.window = window;
			
			View = new NSView(new RectangleF(0, 0, width, height));
			
			switch (startPosition)
			{
				case (WindowStartPositions.CenterCurrentScreen): Center(); break;
			}
			
			ContentView = View;
			WillClose += closingEvent;
		}
		#endregion
		
		#region Methods
		public virtual void Show()
		{
			window.MakeKeyAndOrderFront(NSApplication.SharedApplication);
			window.shown();
		}
		
		public new virtual void Close()
		{
			window.closing();
			base.Close();
			Dispose();
		}
		
		private void closingEvent(object sender, EventArgs e)
		{
			window.closing();
		}
		
		/*public override void SendEvent(NSEvent theEvent)
		{
			if (theEvent == null ||
				!(theEvent.Type == NSEventType.KeyDown || theEvent.Type == NSEventType.KeyUp ||
				theEvent.Type == NSEventType.LeftMouseDown || theEvent.Type == NSEventType.LeftMouseUp ||
				theEvent.Type == NSEventType.OtherMouseDown || theEvent.Type == NSEventType.OtherMouseUp ||
				theEvent.Type == NSEventType.RightMouseDown || theEvent.Type == NSEventType.RightMouseUp ||
				theEvent.Type == NSEventType.ScrollWheel))
			{
				base.SendEvent(theEvent);
				return;
			}
			
			if (HandleEvent != null) HandleEvent(theEvent);
		}*/
		
		public override void MouseDown(NSEvent e)
		{
			switch (e.Type)
			{
				case (NSEventType.LeftMouseDown): theEvent.Type = WindowEventTypes.LeftMouseDown; break;
				case (NSEventType.OtherMouseDown): theEvent.Type = WindowEventTypes.MiddleMouseDown; break;
				case (NSEventType.RightMouseDown): theEvent.Type = WindowEventTypes.RightMouseDown; break;
			}
			
			window.HandleEvent(theEvent);
		}
		
		public override void MouseUp(NSEvent e)
		{
			switch (e.Type)
			{
				case (NSEventType.LeftMouseUp): theEvent.Type = WindowEventTypes.LeftMouseUp; break;
				case (NSEventType.OtherMouseUp): theEvent.Type = WindowEventTypes.MiddleMouseUp; break;
				case (NSEventType.RightMouseUp): theEvent.Type = WindowEventTypes.RightMouseUp; break;
			}

			window.HandleEvent(theEvent);
		}
		
		public override void KeyDown(NSEvent e)
		{
			theEvent.Type = WindowEventTypes.KeyDown;
			theEvent.KeyCode = e.KeyCode;
			window.HandleEvent(theEvent);
		}
		
		public override void KeyUp(NSEvent e)
		{
			theEvent.Type = WindowEventTypes.KeyUp;
			theEvent.KeyCode = e.KeyCode;
			window.HandleEvent(theEvent);
		}
		#endregion
	}
}

