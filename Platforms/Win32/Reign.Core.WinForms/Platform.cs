using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace Reign.Core.WinForms
{
	public class Platform : IPlatform
	{
		public override Size2 ScreenSize
		{
			get
			{
				var screen = Screen.PrimaryScreen;
				return new Size2(screen.Bounds.Width, screen.Bounds.Height);
			}
		}

		public override void Run(IApplication application, int fps)
		{
			base.Run(application, fps);

			var form = (Form)application;
			Time.OptimizedMode();
			form.Show();
			Application.Idle += mainLoop;
			Application.Run(form);
			Time.EndOptimizedMode();
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct Message
		{
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public Point2 p;
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

		private void mainLoop(object sender, EventArgs e)
		{
			var msg = new Message();
			while (!PeekMessage(out msg, IntPtr.Zero, 0, 0, 0))
			{
				UpdateAndRender();
			}
		}
	}
}