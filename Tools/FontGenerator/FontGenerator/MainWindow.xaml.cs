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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using System.Xml.Serialization;
using System.IO;

namespace FontGenerator
{
	class FontColor
	{
		public Color Color;
		public double Offset;

		public FontColor(Color color, double offset)
		{
			Color = color;
			Offset = offset;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", Color.ToString(), Math.Round(Offset, 4));
		}
	}

	public partial class MainWindow : Window
	{
		bool loaded;
		RenderTargetBitmap finalRenderTarget, testRenderTarget;
		Color fontColor, shadowColor, glowColor, bgColor;
		double fontSize;
		Reign.Video.FontMetrics.Character[] xmlCharacters;

		public MainWindow()
		{
			InitializeComponent();
			bgColor = Colors.DarkGray;

			// load fonts
			fontSize = 32;
			double fontSizeValue = 4;
			for (int i = 0; i != 30; ++i)
			{
				fontSizeComboBox.Items.Add(fontSizeValue);
				fontSizeValue += 4;
			}
			fontSizeComboBox.SelectedIndex = 13;
			foreach (var font in Fonts.SystemFontFamilies)
			{
				currentFont.Items.Add(font);
			}
			foreach (var item in currentFont.Items)
			{
				if (((FontFamily)item).Source == "Arial") currentFont.SelectedItem = item;
			}

			// texture size
			double textureSizeValue = 32;
			for (int i = 0; i != 8; ++i)
			{
				textureSizeComboBox.Items.Add(textureSizeValue);
				textureSizeValue *= 2;
			}
			textureSizeComboBox.SelectedIndex = 4;

			// create colors
			fontColor = Colors.White;
			shadowColor = Colors.Black;
			glowColor = Colors.OrangeRed;
			colorRect.Fill = new SolidColorBrush(fontColor);
			shadowColorRect.Fill = new SolidColorBrush(shadowColor);
			glowColorRect.Fill = new SolidColorBrush(glowColor);
			var colors = new FontColor[3]
			{
				new FontColor(Colors.White, 0),
				new FontColor(Colors.Yellow, .5),
				new FontColor(Colors.Red, 1)
			};
			foreach (var color in colors)
			{
				colorListBox.Items.Add(color);
			}

			Loaded += MainWindow_Loaded;
			loaded = true;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			render();
		}

		private void render()
		{
			if (!loaded) return;

			renderImage(testImage, ref testRenderTarget, testImageBorder.ActualWidth, testImageBorder.ActualHeight, testText.Text, true);

			if (generateFinalImage.IsChecked == true)
			{
				double textureSize = (double)textureSizeComboBox.SelectedValue;
				xmlCharacters = renderImage(finalImage, ref finalRenderTarget, textureSize, textureSize, generateCharacters(), false);
			}
		}

		private string generateCharacters()
		{
			string text = "";
			for (int i = 0; i != 95; ++i)
			{
				text += (char)(i + 32);
			}

			return text;
		}

		private Reign.Video.FontMetrics.Character[] renderImage(Image sourceImage, ref RenderTargetBitmap renderTarget, double width, double height, string text, bool isStackPanel)
		{
			var characters = new Reign.Video.FontMetrics.Character[text.Length];

			var grid = new Grid()
			{
				Width = width,
				Height = height
			};

			// create panel type
			Panel panel;
			if (isStackPanel)
			{
				var stackPanel = new StackPanel()
				{
					Orientation = Orientation.Horizontal,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				panel = stackPanel;
			}
			else
			{
				var wrapPanel = new WrapPanel()
				{
					Orientation = Orientation.Horizontal,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
				panel = wrapPanel;
			}
			grid.Children.Add(panel);

			// fill panel with text
			foreach (char c in text)
			{
				var cGrid = new Grid();
				if (enableDropShadow.IsChecked == true)
				{
					var textBlock = createCharacter(c, Colors.Black);
					var effect = new DropShadowEffect()
					{
						RenderingBias = RenderingBias.Quality,
						BlurRadius = (shadowBlur.Value / 10) * 5,
						Direction = (shadowDirectionSlider.Value / 10) * 360,
						Color = shadowColor,
						Opacity = 1,
						//ShadowDepth = 5
					};
					textBlock.Effect = effect;
					cGrid.Children.Add(textBlock);
				}

				if (enableOuterGlow.IsChecked == true)
				{
					var textBlock = createCharacter(c, glowColor);
					var effect = new BlurEffect
					{
						RenderingBias = RenderingBias.Quality,
						Radius = (glowBlurSlider.Value / 10) * 8
					};
					textBlock.Effect = effect;
					cGrid.Children.Add(textBlock);
				}

				if (enableColors.IsChecked == true)
				{
					var textBlock = createCharacter(c, Colors.Transparent);
					double angle = (angleSlider.Value / 10) * Math.PI * 2;
					var gradientBrush = new LinearGradientBrush()
					{
						StartPoint = new Point(0, 0),//Math.Cos(-angle) * .5 + 1, Math.Sin(-angle) * .5 + 1),
						EndPoint = new Point(1, 1)//Math.Cos(angle) * .5 + 1, Math.Sin(angle) * .5 + 1)
					};
					foreach (var fontColor in colorListBox.Items)
					{
						var color = (FontColor)fontColor;
						gradientBrush.GradientStops.Add(new GradientStop(color.Color, color.Offset));
					}
					textBlock.Foreground = gradientBrush;
					cGrid.Children.Add(textBlock);
				}
				else
				{
					var textBlock = createCharacter(c, fontColor);
					cGrid.Children.Add(textBlock);
				}

				panel.Children.Add(cGrid);
			}

			// update grid layout
			grid.Measure(new Size(grid.Width, grid.Height));
			grid.Arrange(new Rect(new Size(grid.Width, grid.Height)));
			grid.UpdateLayout();

			// adjust for buffer bleed
			foreach (var child in panel.Children)
			{
				var childGrid = (Grid)child;
				var sliderValue = (bufferBleedSlider.Value / 10) * 20;
				childGrid.Width = childGrid.ActualWidth + sliderValue;
				childGrid.Height = childGrid.ActualHeight + sliderValue;
				
			}
			grid.UpdateLayout();

			// create characters
			int i = 0;
			foreach (var child in panel.Children)
			{
				var childGrid = (Grid)child;
				var gridPos = childGrid.TranslatePoint(new Point(0, 0), grid);
				characters[i] = new Reign.Video.FontMetrics.Character()
				{
					Key = text[i],
					X = (int)gridPos.X,
					Y = (int)gridPos.Y,
					Width = (int)childGrid.Width,
					Height = (int)childGrid.Height,
				};
				++i;
			}

			// render image
			if (renderTarget == null) renderTarget = new RenderTargetBitmap((int)grid.Width, (int)grid.Height, 96, 96, PixelFormats.Pbgra32);
			else renderTarget.Clear();
			renderTarget.Render(grid);
			sourceImage.Source = renderTarget;

			return characters;
		}

		private TextBlock createCharacter(char c, Color color)
		{
			return new TextBlock()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Text = new string(new char[1]{c}),
				FontFamily = (FontFamily)currentFont.SelectedValue,
				FontSize = fontSize,
				Foreground = new SolidColorBrush(color),
			};
		}

		private void colorOffsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (colorListBox.SelectedIndex == -1) return;
			var color = (FontColor)colorListBox.SelectedValue;
			color.Offset = colorOffsetSlider.Value / 10;

			var fontColors = new List<FontColor>();
			foreach (var item in colorListBox.Items) fontColors.Add((FontColor)item);
			fontColors.Sort((x, y) => x.Offset.CompareTo(y.Offset));
			colorListBox.Items.Clear();
			foreach (var fontColor in fontColors) colorListBox.Items.Add(fontColor);
			colorListBox.SelectedItem = color;

			colorListBox.Items.Refresh();
			render();
		}

		private void colorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (colorListBox.SelectedIndex == -1) return;
			var color = (FontColor)colorListBox.SelectedValue;
			colorOffsetSlider.Value = color.Offset * 10;
			if (enableColors.IsChecked == true) colorRect.Fill = new SolidColorBrush(color.Color);
		}

		private void testText_TextChanged(object sender, TextChangedEventArgs e)
		{
			render();
		}

		private void render_Checked(object sender, RoutedEventArgs e)
		{
			if (enableColors.IsChecked == true)
			{
				if (colorListBox.SelectedIndex != -1)
				{
					var color = (FontColor)colorListBox.SelectedValue;
					colorRect.Fill = new SolidColorBrush(color.Color);
				}
				else
				{
					colorRect.Fill = new SolidColorBrush(Colors.Transparent);
				}
			}
			else
			{
				colorRect.Fill = new SolidColorBrush(fontColor);
			}

			render();
		}

		private void currentFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			render();
		}

		private void subColorButton_Click(object sender, RoutedEventArgs e)
		{
			if (colorListBox.SelectedIndex == -1) return;
			colorListBox.Items.Remove(colorListBox.SelectedItem);
			render();
		}

		private void addColorButton_Click(object sender, RoutedEventArgs e)
		{
			colorListBox.Items.Add(new FontColor(Colors.Black, .5));
			render();
		}

		private bool getColor(out Color color)
		{
			color = Colors.Transparent;
			var dlg = new System.Windows.Forms.ColorDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				color = Color.FromArgb(dlg.Color.A, dlg.Color.R, dlg.Color.G, dlg.Color.B);
				return true;
			}

			return false;
		}

		private void setColorButton_Click(object sender, RoutedEventArgs e)
		{
			Color color;
			if (getColor(out color))
			{
				colorRect.Fill = new SolidColorBrush(color);
				if (enableColors.IsChecked == true)
				{
					var item = (FontColor)colorListBox.SelectedItem;
					item.Color = color;
					colorListBox.Items.Refresh();
				}
				else
				{
					fontColor = color;
				}
				render();
			}
		}

		private void shadowColorButton_Click(object sender, RoutedEventArgs e)
		{
			Color color;
			if (getColor(out color))
			{
				shadowColorRect.Fill = new SolidColorBrush(color);
				shadowColor = color;
				render();
			}
		}

		private void glowColorButton_Click(object sender, RoutedEventArgs e)
		{
			Color color;
			if (getColor(out color))
			{
				glowColorRect.Fill = new SolidColorBrush(color);
				glowColor = color;
				render();
			}
		}

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			render();
		}

		private void textureSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			render();
		}

		private void fontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (fontSizeComboBox.SelectedIndex == -1) return;
			fontSize = (double)fontSizeComboBox.SelectedValue;
			render();
		}

		private void bgColorButton_Click(object sender, RoutedEventArgs e)
		{
			Color color;
			if (getColor(out color))
			{
				bgColor = color;
				finalImageBorder.Background = new SolidColorBrush(color);
				testImageBorder.Background = new SolidColorBrush(color);
				render();
			}
		}

		private void viewImageButton_Click(object sender, RoutedEventArgs e)
		{
			var window = new FinalImageWindow(finalImage.Source, bgColor);
			window.ShowDialog();
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			if (generateFinalImage.IsChecked != true || xmlCharacters == null) return;

			var dlg = new Microsoft.Win32.SaveFileDialog();
			if (dlg.ShowDialog() == true)
			{
				try
				{
					// save xml
					var fontMetrics = new Reign.Video.FontMetrics();
					fontMetrics.Characters = xmlCharacters;

					var xml = new XmlSerializer(typeof(Reign.Video.FontMetrics));
					using (var file = new FileStream(dlg.FileName + ".xml", FileMode.Create, FileAccess.Write))
					{
						xml.Serialize(file, fontMetrics);
					}

					// save image
					using (var file = new FileStream(dlg.FileName + ".png", FileMode.Create, FileAccess.Write))
					{
						var encoder = new PngBitmapEncoder();
						encoder.Frames.Add(BitmapFrame.Create((BitmapSource)finalImage.Source));
						encoder.Save(file);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}
	}
}
