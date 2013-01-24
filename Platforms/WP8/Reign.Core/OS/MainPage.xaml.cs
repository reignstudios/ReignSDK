using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Graphics.Interop;
using Windows.Phone.Input.Interop;
using System.Windows.Media;

namespace Reign.Core
{
	public partial class MainPage : PhoneApplicationPage
	{
		public DrawingSurface Surface {get{return surface;}}

		public MainPage()
		{
			InitializeComponent();

			if (OS.CurrentApplication.orientation == ApplicationOrientations.Landscape)
			{
				SupportedOrientations = SupportedPageOrientation.Landscape;
				Orientation = PageOrientation.Landscape;
			}
			else
			{
				SupportedOrientations = SupportedPageOrientation.Portrait;
				Orientation = PageOrientation.Portrait;
			}
		}

		private void drawingSurface_Loaded(object sender, RoutedEventArgs e)
		{
			OS.CurrentApplication.MainPage = this;
			OS.CurrentApplication.frameSize = new Size2((int)surface.ActualWidth, (int)surface.ActualHeight);
			OS.CurrentApplication.shown();
		}
	}
}