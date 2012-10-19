using System;
using System.Windows.Forms;

namespace Reign.Core
{
	public class WinForm : Form
	{
		#region Properties
		private Window window;
		protected WindowEvent theEvent;
		#endregion

		#region Constructors
		protected void init(Window window, string name, int width, int height, WindowStartPositions startPosition, WindowTypes type)
		{
			this.window = window;

			try
			{
				this.Name = name;
				Text = name;
				ClientSize = new System.Drawing.Size(width, height);
	
				switch (startPosition)
				{
					case (WindowStartPositions.Default): StartPosition = FormStartPosition.WindowsDefaultLocation; break;
					case (WindowStartPositions.CenterCurrentScreen): StartPosition = FormStartPosition.CenterScreen; break;
				}
	
				switch (type)
				{
					case (WindowTypes.Box):
						FormBorderStyle = FormBorderStyle.None; break;

					case (WindowTypes.Frame):
						FormBorderStyle = FormBorderStyle.FixedDialog;
						MaximizeBox = false;
						break;

					case (WindowTypes.FrameSizable):
						FormBorderStyle = FormBorderStyle.Sizable; break;
				}
	
				FormClosing += winFormClosing;
				Shown += winFormShown;
				Move += winFormMove;

				MouseDown += mouseDownEvent;
				MouseUp += mouseUpEvent;
				KeyDown += keyDownEvent;
				KeyUp += keyUpEvent;
				MouseWheel += scrollEvent;
			}
			catch (Exception e)
			{
				window.Close();
				throw e;
			}
		}
		#endregion

		#region Methods
		public new virtual void Close()
		{
			base.Close();
		}
		
		public new virtual void Show()
		{
			base.Show();
		}
		
		private void winFormClosing(object sender, FormClosingEventArgs e)
		{
			if (window != null) window.closing();
		}

		private void winFormShown(object sender, EventArgs e)
		{
			if (window != null) window.shown();
		}

		private void winFormMove(object sender, EventArgs e)
		{
			theEvent.Type = WindowEventTypes.Move;
			if (window != null) window.handleEvent(theEvent);
		}

		private void mouseDownEvent(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case (MouseButtons.Left): theEvent.Type = WindowEventTypes.LeftMouseDown; break;
				case (MouseButtons.Middle): theEvent.Type = WindowEventTypes.MiddleMouseDown; break;
				case (MouseButtons.Right): theEvent.Type = WindowEventTypes.RightMouseDown; break;
			}

			window.handleEvent(theEvent);
		}

		private void mouseUpEvent(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case (MouseButtons.Left): theEvent.Type = WindowEventTypes.LeftMouseUp; break;
				case (MouseButtons.Middle): theEvent.Type = WindowEventTypes.MiddleMouseUp; break;
				case (MouseButtons.Right): theEvent.Type = WindowEventTypes.RightMouseUp; break;
			}

			window.handleEvent(theEvent);
		}

		private void keyDownEvent(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			theEvent.Type = WindowEventTypes.KeyDown;
			theEvent.KeyCode = (int)e.KeyCode;
			window.handleEvent(theEvent);
		}

		private void keyUpEvent(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			theEvent.Type = WindowEventTypes.KeyUp;
			theEvent.KeyCode = (int)e.KeyCode;
			window.handleEvent(theEvent);
		}

		private void scrollEvent(object sender, MouseEventArgs e)
		{
			theEvent.Type = WindowEventTypes.ScrollWheel;
			theEvent.ScrollWheelVelocity = e.Delta;
			window.handleEvent(theEvent);
		}
		#endregion
	}
}
