using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Reign.Core
{
	public class ApplicationPage : Page
	{
		private SwapChainBackgroundPanel swapChainPanel;
		private AdControl adControl;

		public ApplicationPage(string applicationID, string unitID, ApplicationAdSize adSize, ApplicationAdGravity adGravity, bool supportAds)
		{
			swapChainPanel = new SwapChainBackgroundPanel();

			if (supportAds)
			{
				adControl = new AdControl();
				adControl.ApplicationId = applicationID;
				adControl.AdUnitId = unitID;

				adControl.IsEnabled = false;
				adControl.Visibility = Visibility.Collapsed;
				
				switch (adSize)
				{
					case (ApplicationAdSize.Sqaure_250x250):
						adControl.Width = 250;
						adControl.Height = 250;
						break;

					default:
						Debug.ThrowError("ApplicationPage", "Unsuported Ad size");
						break;
				}

				switch (adGravity)
				{
					case (ApplicationAdGravity.BottomLeft):
						adControl.HorizontalAlignment = HorizontalAlignment.Left;
						adControl.VerticalAlignment = VerticalAlignment.Bottom;
						break;

					case (ApplicationAdGravity.BottomRight):
						adControl.HorizontalAlignment = HorizontalAlignment.Right;
						adControl.VerticalAlignment = VerticalAlignment.Bottom;
						break;

					case (ApplicationAdGravity.BottomCenter):
						adControl.HorizontalAlignment = HorizontalAlignment.Center;
						adControl.VerticalAlignment = VerticalAlignment.Top;
						break;

					case (ApplicationAdGravity.TopLeft):
						adControl.HorizontalAlignment = HorizontalAlignment.Left;
						adControl.VerticalAlignment = VerticalAlignment.Top;
						break;

					case (ApplicationAdGravity.TopRight):
						adControl.HorizontalAlignment = HorizontalAlignment.Right;
						adControl.VerticalAlignment = VerticalAlignment.Top;
						break;

					case (ApplicationAdGravity.TopCenter):
						adControl.HorizontalAlignment = HorizontalAlignment.Center;
						adControl.VerticalAlignment = VerticalAlignment.Top;
						break;

					default:
						Debug.ThrowError("ApplicationPage", "Unsuported Ad gravity");
						break;
				}
				
				swapChainPanel.Children.Add(adControl);
			}

			base.Content = swapChainPanel;
			OS.CurrentXAMLApplication.SwapChainPanel = swapChainPanel;
		}

		public void EnableAds()
		{
			if (adControl != null)
			{
				adControl.IsEnabled = true;
				adControl.Visibility = Visibility.Visible;
			}
		}

		public void DisableAds()
		{
			if (adControl != null)
			{
				adControl.Visibility = Visibility.Collapsed;
				adControl.IsEnabled = false;
			}
		}
	}

	public class XAMLApplication : Windows.UI.Xaml.Application, ApplicationI
	{
		#region Properties
		public SwapChainBackgroundPanel SwapChainPanel {get; internal set;}
		private ApplicationEvent theEvent;
		internal ApplicationOrientations orientation;
		
		internal Size2 frameSize;
		public Size2 FrameSize
		{
			get {return frameSize;}
		}

		public Size2 Metro_FrameSize
		{
			get {return frameSize;}
			set {frameSize = value;}
		}

		public delegate void ApplicationEventMethod();
		public ApplicationEventMethod Closing;

		public delegate void HandleEventMethod(ApplicationEvent theEvent);
		public HandleEventMethod HandleEvent;

		public delegate void StateMethod();
		public static StateMethod PauseCallback, ResumeCallback;
		#endregion

		#region Constructors
		public XAMLApplication(ApplicationOrientations orientation, string applicationID, string unitID, ApplicationAdSize adSize, ApplicationAdGravity adGravity, bool supportAds)
		: this(applicationID, unitID, adSize, adGravity, supportAds)
		{
			this.orientation = orientation;
			theEvent = new ApplicationEvent();
			OS.Init(this);
		}
		#endregion

		#region Methods
		protected internal virtual void shown()
		{
			
		}
		
		protected internal virtual void closing()
		{
			deferral.Complete();
		}
		
		protected internal virtual void handleEvent(ApplicationEvent theEvent)
		{
			if (HandleEvent != null) HandleEvent(theEvent);
		}

		public void Metro_HandleEvent(ApplicationEvent theEvent)
		{
			handleEvent(theEvent);
		}
		
		protected internal virtual void update(Time time)
		{
			
		}

		protected internal virtual void render(Time time)
		{
			
		}
		
		protected internal virtual void pause()
		{
			if (PauseCallback != null) PauseCallback();
		}
		
		protected internal virtual void resume()
		{
			if (ResumeCallback != null) ResumeCallback();
		}
		#endregion


		#region Base
		public delegate void BuyAppCallbackMethod(bool succeeded);
		public BuyAppCallbackMethod BuyAppCallback;

		private CoreMetroWindow coreMetroWindow;
		private bool running, visible;
		private SuspendingDeferral deferral;
		private string applicationID, unitID;
		private ApplicationAdSize adSize;
		private ApplicationAdGravity adGravity;
		private bool supportAds;

		private XAMLApplication(string applicationID, string unitID, ApplicationAdSize adSize, ApplicationAdGravity adGravity, bool supportAds)
        {
            this.Suspending += onSuspending;
			this.Resuming += onResuming;

			this.applicationID = applicationID;
			this.unitID = unitID;
			this.adSize = adSize;
			this.adGravity = adGravity;
			this.supportAds = supportAds;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
			var window = Window.Current.CoreWindow;
			if (OS.CoreWindow != window) window.VisibilityChanged += visibilityChanged;
			OS.CoreWindow = window;

			if (coreMetroWindow != null) coreMetroWindow.Dispose();
			coreMetroWindow = new CoreMetroWindow(this, Window.Current.CoreWindow, theEvent);
			frameSize = new Size2(coreMetroWindow.ConvertDipsToPixels(window.Bounds.Width), coreMetroWindow.ConvertDipsToPixels(window.Bounds.Height));

			if (Window.Current.Content as ApplicationPage == null)
            {
                /*if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }*/
				
				Window.Current.Content = new ApplicationPage(applicationID, unitID, adSize, adGravity, supportAds);
				shown();
            }
			
            Window.Current.Activate();
			running = true;
        }

		private void rendering(object sender, object e)
		{
			OS.UpdateAndRender();
		}

		private void visibilityChanged(CoreWindow sender, VisibilityChangedEventArgs args)
		{
			visible = args.Visible;

			if (running)
			{
				if (visible)
				{
					resume();
					CompositionTarget.Rendering += rendering;
				}
				else
				{
					CompositionTarget.Rendering -= rendering;
					pause();
				}
			}
		}

        private void onSuspending(object sender, SuspendingEventArgs e)
        {
			running = false;
            deferral = e.SuspendingOperation.GetDeferral();
			closing();
        }

		private void onResuming(object sender, object e)
		{
			shown();
			running = true;
		}

		public void ShowCursor()
		{
			coreMetroWindow.ShowCursor();
		}

		public void HideCursor()
		{
			coreMetroWindow.HideCursor();
		}

		public void EnableAds()
		{
			var page = Window.Current.Content as ApplicationPage;
			if (page != null) page.EnableAds();
		}

		public void DisableAds()
		{
			var page = Window.Current.Content as ApplicationPage;
			if (page != null) page.DisableAds();
		}

		public bool IsTrial()
		{
			return coreMetroWindow.IsTrial();
		}

		public bool InAppPurchased(string appID)
		{
			return coreMetroWindow.InAppPurchased(appID);
		}

		public async void BuyInAppItem(string appID)
		{
			if (BuyAppCallback != null) BuyAppCallback(await coreMetroWindow.BuyInAppItem(appID));
			else Debug.ThrowError("XAMLApplication", "BuyAppCallback method cannot be null");
		}
		#endregion
	}
}
