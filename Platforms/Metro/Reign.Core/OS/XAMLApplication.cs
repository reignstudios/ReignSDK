using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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

		public ApplicationPage()
		{
			swapChainPanel = new SwapChainBackgroundPanel();

			adControl = new AdControl();
			adControl.Width = 250;
			adControl.Height = 250;
			adControl.ApplicationId = "d25517cb-12d4-4699-8bdc-52040c712cab";
			adControl.AdUnitId = "10043105";
			adControl.HorizontalAlignment = HorizontalAlignment.Left;
			adControl.VerticalAlignment = VerticalAlignment.Top;
			swapChainPanel.Children.Add(adControl);

			base.Content = swapChainPanel;
			OS.CurrentPageApplication.SwapChainPanel = swapChainPanel;
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
		public XAMLApplication(ApplicationOrientations orientation)
		: this()
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
		private CoreMetroWindow coreMetroWindow;

		private XAMLApplication()
        {
            this.Suspending += onSuspending;
			this.Resuming += onResuming;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
			var coreWindow = Window.Current.CoreWindow;
			OS.CoreWindow = coreWindow;
			if (coreMetroWindow == null) coreMetroWindow = new CoreMetroWindow(this, Window.Current.CoreWindow, theEvent);
			frameSize = new Size2(coreMetroWindow.ConvertDipsToPixels(coreWindow.Bounds.Width), coreMetroWindow.ConvertDipsToPixels(coreWindow.Bounds.Height));

			if (Window.Current.Content as ApplicationPage == null)
            {
                /*if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }*/
				
				Window.Current.Content = new ApplicationPage();
				shown();
				CompositionTarget.Rendering += rendering;
            }
			
            Window.Current.Activate();
        }

		private void rendering(object sender, object e)
		{
			OS.UpdateAndRender();
		}

        private void onSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // Insert suspend code here !!!
            deferral.Complete();
        }

		private void onResuming(object sender, object e)
		{
			
		}
		#endregion
	}
}
