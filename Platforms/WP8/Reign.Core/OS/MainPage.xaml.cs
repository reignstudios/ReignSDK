using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Reign.Core
{
	public partial class MainPage : PhoneApplicationPage
	{
		public MainPage()
		{
			InitializeComponent();

			surface = new DrawingSurface();
			surface.Loaded += drawingSurface_Loaded;
			this.Content = surface;
		}

		private void drawingSurface_Loaded(object sender, RoutedEventArgs e)
		{
			//drawingSurface.SetContentProvider(provider);// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< NEXT STEP: make a way to call these !!!
			//drawingSurface.SetManipulationHandler(handler);

			//application.frameSize = new Size2((int)Surface.ActualWidth, (int)Surface.ActualHeight);
			//application.shown();

			//CompositionTarget.Rendering += rendering;
		}

		/*private void rendering(object sender, object e)
		{
			OS.UpdateAndRender();
		}*/
	}
}