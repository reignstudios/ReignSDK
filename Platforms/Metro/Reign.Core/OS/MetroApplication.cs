using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Reign.Core
{
	public class MetroApplication : Windows.UI.Xaml.Application
	{
		#region Properties
		private Reign.Core.Application application;
		#endregion

		#region Constructors
		public MetroApplication()
		{
			this.Suspending += OnSuspending;
		}

		protected void setApplication(Application application)
		{
			this.application = application;
		}

		protected override void OnLaunched(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs args)
		{
			base.OnLaunched(args);

			if (args.PreviousExecutionState == ApplicationExecutionState.Running)
			{
				Window.Current.Activate();
				return;
			}

			if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
			{
				application.resume();
				return;
			}

			application.shown();
			CompositionTarget.Rendering += updateAndRender;
		}

		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			application.pause();
			deferral.Complete();
		}
		#endregion

		#region Methods
		private void updateAndRender(object sender, object e)
		{
			application.UpdateAndRender();
		}
		#endregion
	}
}
