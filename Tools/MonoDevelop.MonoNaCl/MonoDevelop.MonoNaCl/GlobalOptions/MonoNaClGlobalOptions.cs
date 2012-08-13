using System;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Ide.Projects;
using MonoDevelop.Ide.Gui.Dialogs;

namespace MonoDevelop.MonoNaCl
{
	public class MonoNaClGlobalOptionsBinding : OptionsPanel
	{
		private MonoNaClGlobalOptions panel;
		
		public override Gtk.Widget CreatePanelWidget ()
		{
			panel = new MonoNaClGlobalOptions ();
			panel.Load ();
			return panel;
		}

		public override bool ValidateChanges ()
		{
			return panel.Validate ();
		}
			
		public override void ApplyChanges ()
		{
			panel.Store ();
		}
	}

	[System.ComponentModel.ToolboxItem(true)]
	public partial class MonoNaClGlobalOptions : Gtk.Bin
	{
		MonoNaClSettingsService instance;
	
		public MonoNaClGlobalOptions ()
		{
			this.Build ();
		}
	
		public void Load ()
		{
			instance = MonoNaClSettingsService.Instance;
			NACL_SDK_ROOT.Text = instance.NACL_SDK_ROOT;
			NACL_MONO_ROOT.Text = instance.NACL_MONO_ROOT;
		}

		public bool Validate ()
		{
			return true;
		}
		
		public void Store ()
		{
			instance.NACL_SDK_ROOT = NACL_SDK_ROOT.Text;
			instance.NACL_MONO_ROOT = NACL_MONO_ROOT.Text;
		}
	}
}


