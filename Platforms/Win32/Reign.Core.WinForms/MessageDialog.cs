using System.Windows.Forms;

namespace Reign.Core.WinForms
{
	public class MessageDialog : IMessageDialog
	{
		public void Show(string title, string message, MessageDialogCallbackMethod callback)
		{
			MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
	}
}
