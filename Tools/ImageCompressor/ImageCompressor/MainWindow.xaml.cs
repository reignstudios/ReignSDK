using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.IO.Compression;
using Reign.Video;

namespace ImageCompressor
{
	public partial class MainWindow : Window
	{
		byte[] srcData, dstData;
		int width, height;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void openButton_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog();
			if (dlg.ShowDialog(this).Value)
			{
				try
				{
					if (new FileInfo(dlg.FileName).Extension.ToLower() == ".bmpc")
					{
						var image = new ImageBMPC(dlg.FileName, false);
						var writable = new WriteableBitmap(image.Size.Width, image.Size.Height, 96, 96, PixelFormats.Bgra32, null);
						int stride = (image.Size.Width * 32 + 7) / 8;

						// Flip RB Color bits
						var data = new byte[image.Mipmaps[0].Data.LongLength];
						image.Mipmaps[0].Data.CopyTo(data, 0);
						for (int i2 = 0; i2 != data.Length; i2 += 4)
						{
							byte c = data[i2];
							data[i2] = data[i2+2];
							data[i2+2] = c;
						}

						writable.WritePixels(new Int32Rect(0, 0, image.Size.Width, image.Size.Height), data, stride, 0);
						dstImage.Source = writable;
					}
					else
					{
						// Load binary data
						var bitmap = new BitmapImage(new Uri(dlg.FileName, UriKind.Absolute));
						int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;
						srcData = new byte[bitmap.PixelWidth * stride];
						dstData = new byte[bitmap.PixelWidth * stride];
						bitmap.CopyPixels(srcData, stride, 0);
						bitmap.CopyPixels(dstData, stride, 0);

						// Get image atributes
						var ext = new FileInfo(dlg.FileName).Extension.ToLower();
						width = bitmap.PixelWidth;
						height = bitmap.PixelHeight;

						// Display images
						var w = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, 96, 96, PixelFormats.Bgra32, null);
						w.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), srcData, stride, 0);
						dstImage.Source = w;
						srcImage.Source = bitmap;

						processesImage();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void processesImage()
		{
			//if (srcData == null) return;
		}

		private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			processesImage();
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			if (dstData == null)
			{
				MessageBox.Show("Must load an image from a file first.");
				return;
			}

			var dlg = new SaveFileDialog();
			if (dlg.ShowDialog(this).Value)
			{
				try
				{
					string fileName = dlg.FileName;
					var fileInfo = new FileInfo(fileName);
					if (string.IsNullOrEmpty(fileInfo.Extension)) fileName += ((ComboBoxItem)formatComboBox.SelectedItem).Content;

					// Flip RB Color bits
					var data = new byte[dstData.LongLength];
					dstData.CopyTo(data, 0);
					for (int i2 = 0; i2 != data.Length; i2 += 4)
					{
						byte c = data[i2];
						data[i2] = data[i2+2];
						data[i2+2] = c;
					}

					using (var stream = ImageBMPC.Save(data, width, height))
					using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
					{
						stream.CopyTo(file);
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
