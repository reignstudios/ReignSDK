using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Phone.Graphics.Interop;
using Windows.Phone.Input.Interop;

namespace Reign.Core
{
	class MainPageUriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
			return new Uri("/Reign.Core;component/OS/MainPage.xaml", UriKind.Relative);
        }
    }

	public class XAMLApplication : System.Windows.Application
	{
		#region Properties
		private Application application;
		protected ApplicationEvent theEvent;

		public static PhoneApplicationFrame RootFrame { get; private set; }
		public MainPage MainPage {get; internal set;}
		private bool phoneApplicationInitialized;
		internal ApplicationOrientations orientation;
		#endregion

		#region Constructors
		public XAMLApplication(ApplicationOrientations orientation)
		{
			this.orientation = orientation;
			UnhandledException += XAMLApplication_UnhandledException;

			this.Resources.Add("LocalizedStrings", "clr-namespace:Demo_Windows");

			var phoneApplicationService = new PhoneApplicationService();
			phoneApplicationService.Launching += Application_Launching;
			phoneApplicationService.Closing += Application_Closing;
			phoneApplicationService.Activated += Application_Activated;
			phoneApplicationService.Deactivated += Application_Deactivated;
			this.ApplicationLifetimeObjects.Add(phoneApplicationService);

			if (!phoneApplicationInitialized)
			{
				RootFrame = new PhoneApplicationFrame();
				RootFrame.UriMapper = new MainPageUriMapper();// Override the main page loader
				RootFrame.Navigated += CompleteInitializePhoneApplication;

				RootFrame.NavigationFailed += RootFrame_NavigationFailed;
				RootFrame.Navigated += CheckForResetNavigation;
				phoneApplicationInitialized = true;
			}

			if (Debugger.IsAttached)
			{
				Application.Current.Host.Settings.EnableFrameRateCounter = true;
				PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
			}
		}

		private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
		{
			if (RootVisual != RootFrame) RootVisual = RootFrame;
			RootFrame.Navigated -= CompleteInitializePhoneApplication;
		}

		private void XAMLApplication_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (Debugger.IsAttached) Debugger.Break();
		}

		private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			if (Debugger.IsAttached) Debugger.Break();
		}

		private void CheckForResetNavigation(object sender, NavigationEventArgs e)
		{
			if (e.NavigationMode == NavigationMode.Reset) RootFrame.Navigated += ClearBackStackAfterReset;
		}

		private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
		{
			RootFrame.Navigated -= ClearBackStackAfterReset;
			if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh) return;
			while (RootFrame.RemoveBackEntry() != null)
			{
				; // do nothing
			}
		}

		protected void setApplication(Application application)
		{
			this.application = application;
		}
		#endregion

		#region Methods
		private void Application_Launching(object sender, LaunchingEventArgs e)
		{
		}

		private void Application_Activated(object sender, ActivatedEventArgs e)
		{
		}

		private void Application_Deactivated(object sender, DeactivatedEventArgs e)
		{
		}

		private void Application_Closing(object sender, ClosingEventArgs e)
		{
		}
		#endregion
	}
}
