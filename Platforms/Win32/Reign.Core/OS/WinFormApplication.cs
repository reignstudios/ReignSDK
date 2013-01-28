using System;
using System.Windows.Forms;

namespace Reign.Core
{
	public abstract class WinFormApplication : Form, ApplicationI
	{
		#region Properties
		public new IntPtr Handle {get{return base.Handle;}}
		public ApplicationOrientations Orientation {get; private set;}

		public Size2 FrameSize
		{
			get
			{
				var frame = ClientSize;
				return new Size2(frame.Width, frame.Height);
			}
		}

		public new bool Closed {get{return IsDisposed;}}

		public event ApplicationHandleEventMethod HandleEvent;
		public event ApplicationStateMethod PauseCallback, ResumeCallback;

		private ApplicationEvent theEvent;
		#endregion

		#region Constructors
		public void Init(ApplicationDesc desc)
		{
			theEvent = new ApplicationEvent();

			base.Name = desc.Name;
			base.Text = desc.Name;
			var frame = desc.FrameSize;
			if (frame.Width == 0 || frame.Height == 0) frame = (OS.ScreenSize.ToVector2() / 1.5f).ToSize();
			base.ClientSize = new System.Drawing.Size(frame.Width, frame.Height);
	
			switch (desc.StartPosition)
			{
				case (ApplicationStartPositions.Default): StartPosition = FormStartPosition.WindowsDefaultLocation; break;
				case (ApplicationStartPositions.CenterCurrentScreen): StartPosition = FormStartPosition.CenterScreen; break;
			}
	
			switch (desc.Type)
			{
				case (ApplicationTypes.Box):
					FormBorderStyle = FormBorderStyle.None; break;

				case (ApplicationTypes.Frame):
					FormBorderStyle = FormBorderStyle.FixedDialog;
					MaximizeBox = false;
					break;

				case (ApplicationTypes.FrameSizable):
					FormBorderStyle = FormBorderStyle.Sizable; break;
			}
	
			base.FormClosing += winFormClosing;
			base.Shown += winFormShown;

			MouseMove += mouseMoveEvent;
			MouseDown += mouseDownEvent;
			MouseUp += mouseUpEvent;
			KeyDown += keyDownEvent;
			KeyUp += keyUpEvent;
			MouseWheel += scrollEvent;
		}
		#endregion

		#region Method Events
		private void winFormShown(object sender, EventArgs e)
		{
			Shown();
		}

		private void winFormClosing(object sender, FormClosingEventArgs e)
		{
			Closing();
		}

		private void handleEvent(ApplicationEvent applicationEvent)
		{
			if (HandleEvent != null) HandleEvent(applicationEvent);
		}

		private void mouseMoveEvent(object sender, MouseEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.MouseMove;
			theEvent.CursorPosition.X = e.X;
			theEvent.CursorPosition.Y = e.Y;
			handleEvent(theEvent);
		}

		private void mouseDownEvent(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case (MouseButtons.Left): theEvent.Type = ApplicationEventTypes.LeftMouseDown; break;
				case (MouseButtons.Middle): theEvent.Type = ApplicationEventTypes.MiddleMouseDown; break;
				case (MouseButtons.Right): theEvent.Type = ApplicationEventTypes.RightMouseDown; break;
			}

			theEvent.CursorPosition.X = e.X;
			theEvent.CursorPosition.Y = e.Y;
			handleEvent(theEvent);
		}

		private void mouseUpEvent(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case (MouseButtons.Left): theEvent.Type = ApplicationEventTypes.LeftMouseUp; break;
				case (MouseButtons.Middle): theEvent.Type = ApplicationEventTypes.MiddleMouseUp; break;
				case (MouseButtons.Right): theEvent.Type = ApplicationEventTypes.RightMouseUp; break;
			}

			theEvent.CursorPosition.X = e.X;
			theEvent.CursorPosition.Y = e.Y;
			handleEvent(theEvent);
		}

		private void keyDownEvent(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			theEvent.Type = ApplicationEventTypes.KeyDown;
			theEvent.KeyCode = (int)e.KeyCode;
			handleEvent(theEvent);
		}

		private void keyUpEvent(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			theEvent.Type = ApplicationEventTypes.KeyUp;
			theEvent.KeyCode = (int)e.KeyCode;
			handleEvent(theEvent);
		}

		private void scrollEvent(object sender, MouseEventArgs e)
		{
			theEvent.Type = ApplicationEventTypes.ScrollWheel;
			theEvent.ScrollWheelVelocity = e.Delta;
			theEvent.CursorPosition.X = e.X;
			theEvent.CursorPosition.Y = e.Y;
			handleEvent(theEvent);
		}
		#endregion

		#region Methods
		public new virtual void Shown()
		{
			
		}

		public new virtual void Closing()
		{
			
		}

		public new void Close()
		{
			if (Closed) return;
			base.Close();
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
			Cursor.Show();
		}

		public void HideCursor()
		{
			Cursor.Hide();
		}
		#endregion
	}
}
