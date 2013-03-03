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
	class ApplicationPage : Page
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
					case ApplicationAdSize.Sqaure_250x250:
						adControl.Width = 250;
						adControl.Height = 250;
						break;

					default:
						Debug.ThrowError("ApplicationPage", "Unsuported Ad size");
						break;
				}

				switch (adGravity)
				{
					case ApplicationAdGravity.BottomLeft:
						adControl.HorizontalAlignment = HorizontalAlignment.Left;
						adControl.VerticalAlignment = VerticalAlignment.Bottom;
						break;

					case ApplicationAdGravity.BottomRight:
						adControl.HorizontalAlignment = HorizontalAlignment.Right;
						adControl.VerticalAlignment = VerticalAlignment.Bottom;
						break;

					case ApplicationAdGravity.BottomCenter:
						adControl.HorizontalAlignment = HorizontalAlignment.Center;
						adControl.VerticalAlignment = VerticalAlignment.Top;
						break;

					case ApplicationAdGravity.TopLeft:
						adControl.HorizontalAlignment = HorizontalAlignment.Left;
						adControl.VerticalAlignment = VerticalAlignment.Top;
						break;

					case ApplicationAdGravity.TopRight:
						adControl.HorizontalAlignment = HorizontalAlignment.Right;
						adControl.VerticalAlignment = VerticalAlignment.Top;
						break;

					case ApplicationAdGravity.TopCenter:
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
			((XAMLApplication)OS.CurrentApplication).SwapChainPanel = swapChainPanel;
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

	public abstract class XAMLApplication : Application, ApplicationI
	{
		#region Properties
		public SwapChainBackgroundPanel SwapChainPanel {get; internal set;}
		public delegate void BuyAppCallbackMethod(bool succeeded);
		public BuyAppCallbackMethod BuyAppCallback;

		private CoreMetroWindow coreMetroWindow;
		private bool running, visible;
		private SuspendingDeferral deferral;
		private ApplicationDesc desc;
		
		public bool IsSnapped {get; private set;}
		public Windows.UI.Core.CoreWindow CoreWindow {get; private set;}
		public ApplicationOrientations Orientation {get; private set;}
		public Size2 FrameSize {get; private set;}
		public new bool Closed {get; private set;}

		public event ApplicationHandleEventMethod HandleEvent;
		public event ApplicationStateMethod PauseCallback, ResumeCallback;

		private ApplicationEvent theEvent;
		#endregion

		#region Constructors
		public void Init(ApplicationDesc desc)
		{
			OS.CurrentApplication = this;
			OS.time = new Time(0);
			OS.time.Start();

			this.desc = desc;
			theEvent = new ApplicationEvent();
			this.Suspending += onSuspending;
			this.Resuming += onResuming;
		}
		#endregion

		#region Method Events
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
			var window = Window.Current.CoreWindow;
			if (CoreWindow != window) window.VisibilityChanged += visibilityChanged;
			CoreWindow = window;

			if (coreMetroWindow != null) coreMetroWindow.Dispose();
			coreMetroWindow = new CoreMetroWindow(this, Window.Current.CoreWindow, theEvent, false);
			FrameSize = new Size2(coreMetroWindow.ConvertDipsToPixels(window.Bounds.Width), coreMetroWindow.ConvertDipsToPixels(window.Bounds.Height));

			if (Window.Current.Content as ApplicationPage == null)
            {
                /*if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }*/
				
				Window.Current.Content = new ApplicationPage(desc.WinRTAdApplicationID, desc.WinRTAdUnitID, desc.AdSize, desc.AdGravity, desc.UseAds);
				Shown();
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
					Resume();
					CompositionTarget.Rendering += rendering;
				}
				else
				{
					CompositionTarget.Rendering -= rendering;
					Pause();
				}
			}
		}

        private void onSuspending(object sender, SuspendingEventArgs e)
        {
			running = false;
            deferral = e.SuspendingOperation.GetDeferral();
			this.Closing();
        }

		private void onResuming(object sender, object e)
		{
			Shown();
			running = true;
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

		internal void updateFrameSize(Size2 size, bool isSnapped)
		{
			FrameSize = size;
			IsSnapped = isSnapped;
		}

		internal void handleEvent(ApplicationEvent theEvent)
		{
			if (HandleEvent != null) HandleEvent(theEvent);
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
			coreMetroWindow.ShowCursor();
		}

		public void HideCursor()
		{
			coreMetroWindow.HideCursor();
		}
		#endregion
	}
}
