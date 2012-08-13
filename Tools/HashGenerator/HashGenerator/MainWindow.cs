using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace HashGenerator
{
	public partial class MainWindow : Form
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void generateButton_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(fileNameTextBox.Text))
			{
				using (var file = new FileStream(fileNameTextBox.Text, FileMode.Open, FileAccess.Read))
				{
					var hash = new SHA1CryptoServiceProvider().ComputeHash(file);
					hashTextBox.Text = BitConverter.ToString(hash);

					string hexValue = "";
					foreach (byte x in hash)
					{
						hexValue += String.Format("{0:x2}", x);
					}
					hexTextBox.Text = "0x" + hexValue.ToUpper();
				}
			}

			guidTextBox.Text = Guid.NewGuid().ToString().ToUpper();
		}
	}
}
