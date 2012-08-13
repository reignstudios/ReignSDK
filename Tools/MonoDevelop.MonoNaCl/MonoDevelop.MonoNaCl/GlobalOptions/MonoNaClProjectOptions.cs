using System;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Ide.Projects;
using MonoDevelop.Ide.Gui.Dialogs;

namespace MonoDevelop.MonoNaCl
{
	public class ProjectOptionsBinding : ItemOptionsPanel
	{
		private MonoNaClProjectOptions panel;

		public override Gtk.Widget CreatePanelWidget ()
		{
			panel = new MonoNaClProjectOptions ();
			panel.Load(ConfiguredProject as MonoNaClProject);
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
	public partial class MonoNaClProjectOptions : Gtk.Bin
	{
		private MonoNaClProject project;
	
		public MonoNaClProjectOptions ()
		{
			this.Build ();
		}
		
		public void Load(MonoNaClProject project)
		{
			this.project = project;
			if (project == null) return;
			
			requiresGLES.Active = project.RequiresGLES;
			copyAllJsonObjects.Active = project.CopyAllJsonObjects;
			copyAllHtmlObjects.Active = project.CopyAllHtmlObjects;
			generateManifest.Active = project.GenerateManifest;
			
			appName.Text = project.AppName;
			appVersion.Text = project.AppVersion;
			appLaunchHtml.Text = project.AppLaunchHTML;
			appDescription.Text = project.AppDescription;
		}
		
		public bool Validate()
		{
			return true;
		}
		
		public void Store()
		{
			if (project == null) return;
			
			project.RequiresGLES = requiresGLES.Active;
			project.CopyAllJsonObjects = copyAllJsonObjects.Active;
			project.CopyAllHtmlObjects = copyAllHtmlObjects.Active;
			project.GenerateManifest = generateManifest.Active;
			
			project.AppName = appName.Text;
			project.AppVersion = appVersion.Text;
			project.AppLaunchHTML = appLaunchHtml.Text;
			project.AppDescription = appDescription.Text;
		}
	}
}

