﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Graphics;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Reign.Core
{
	public class SilverlightUserControl : UserControl
	{
		private SilverlightApplication application;
		private DrawingSurface surface;
		private bool shown;

		public SilverlightUserControl(SilverlightApplication application)
		{
			this.application = application;

			surface = new DrawingSurface();
			surface.SizeChanged += sizeChanged;
			surface.Draw += updateAndRender;
			this.Content = surface;
		}

		private void sizeChanged(object sender, SizeChangedEventArgs e)
		{
			application.FrameSize = new Size2((int)e.NewSize.Width, (int)e.NewSize.Height);
		}

		private void updateAndRender(object sender, DrawEventArgs e)
		{
			if (!shown)
			{
				shown = true;
				OS.time = new Time(0);
				application.GraphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;
				application.Shown();
			}

			OS.time.ManualUpdate(e.DeltaTime.Milliseconds / 1000f);
			application.Update(OS.time);
			application.Render(OS.time);
			e.InvalidateSurface();
		}
	}
	
	public abstract class SilverlightApplication : Application, ApplicationI
	{
		#region Properties
		public SilverlightUserControl MainUserControl {get; private set;}
		public GraphicsDevice GraphicsDevice {get; internal set;}
		private bool failedToStart;

		public ApplicationOrientations Orientation {get; private set;}
		public Size2 FrameSize {get; internal set;}
		public new bool Closed {get; private set;}

		public event ApplicationHandleEventMethod HandleEvent;
		public event ApplicationStateMethod PauseCallback, ResumeCallback;

		private ApplicationEvent theEvent;
		#endregion

		#region Constructors
		public void Init(ApplicationDesc desc)
		{
			OS.CurrentApplication = this;

			if (GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware)
			{
				string message;
				switch (GraphicsDeviceManager.Current.RenderModeReason)
				{
					case RenderModeReason.Not3DCapable:
						message = "You graphics hardware is not capable of displaying this page ";
						break;

					case RenderModeReason.GPUAccelerationDisabled:
						message = "Hardware graphics acceleration has not been enabled on this web page.\n\n" +
						"Please notify the web site owner.";
						break;

					case RenderModeReason.TemporarilyUnavailable:
						message = "Your graphics hardware is temporarily unavailable.\n\n"+
						"Try reloading the web page or restarting your browser.";
						break;

					case RenderModeReason.SecurityBlocked:
						message =
						"You need to configure your system to allow this web site to display 3D graphics:\n\n" +
						"  1. Right-Click the page\n" +
						"  2. Select 'Silverlight'\n" +
						"     (The 'Microsoft Silverlight Configuration' dialog will be displayed)\n" +
						"  3. Select the 'Permissions' tab\n" +
						"  4. Find this site in the list and change its 3D Graphics permission from 'Deny' to 'Allow'\n" +
						"  5. Click 'OK'\n" +
						"  6. Reload the page";
						break;

					default:
						message = "Unknown error";
						break;
				}

				MessageBox.Show(message, "3D Content Blocked", MessageBoxButton.OK);
				failedToStart = true;
				return;
			}

			this.Startup += this.Application_Startup;
			this.Exit += this.Application_Exit;
			this.UnhandledException += this.Application_UnhandledException;
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			if (failedToStart) return;

			MainUserControl = new SilverlightUserControl(this);
			this.RootVisual = MainUserControl;
		}

		private void Application_Exit(object sender, EventArgs e)
		{
			if (failedToStart) return;
			Closing();
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				Closing();
				Message.Show("UnhandledException", e.ExceptionObject.Message);

				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate {ReportErrorToDOM(e);});
			}
		}

		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
		#endregion

		#region Methods
		public virtual void Shown()
		{
			
		}

		public virtual void Closing()
		{
			
		}

		public void Close()
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
