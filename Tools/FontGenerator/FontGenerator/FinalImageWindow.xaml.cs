using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FontGenerator
{
	public partial class FinalImageWindow : Window
	{
		public FinalImageWindow(ImageSource imageSource, Color bgColor)
		{
			InitializeComponent();

			grid.Background = new SolidColorBrush(bgColor);
			image.Source = imageSource;
		}
	}
}
