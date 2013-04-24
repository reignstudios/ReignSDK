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
	/// <summary>
	/// Interaction logic for MetricsWindow.xaml
	/// </summary>
	public partial class MetricsWindow : Window
	{
		public MetricsWindow(Reign.Video.FontMetrics.Character[] tempCharacters, double textureSize)
		{
			InitializeComponent();

			foreach (var c in tempCharacters)
			{
				listBox.Items.Add
				(
					string.Format("\"{0}\" - Index={1} X={2} Y={3} Width={4} Height={5} - Xf={6} Yf={7} Widthf={8} Heightf={9}",
					c.Key, (int)c.Key,
					c.X, c.Y, c.Width, c.Height,
					c.X/textureSize, c.Y/textureSize, c.Width/textureSize, c.Height/textureSize));
			}
		}
	}
}
