using System;
using Gtk;
using ShaderCompiler.Core;
using System.IO;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow ()
	: base (Gtk.WindowType.Toplevel)
	{
		Build ();
		
		loadSettings();
		compileButton.Clicked += compileButton_Clicked;
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		saveSettings();
		Application.Quit ();
		a.RetVal = true;
	}
	
	private void loadSettings()
	{
		try
		{
			if (!new FileInfo("Settings.txt").Exists)
			{
				csProjTextBox.Text = "Enter CS project path here...";
				shaderDirTextBox.Text = "Enter .rs/.fx output path here...";
				return;
			}
			
			using (var file = new FileStream("Settings.txt", FileMode.OpenOrCreate, FileAccess.Read))
			using (var reader = new StreamReader(file))
			{
				csProjTextBox.Text = reader.ReadLine();
				shaderDirTextBox.Text = reader.ReadLine();
				compileTypesComboBox.Active = int.Parse(reader.ReadLine());
				compileMetroShadersCheckBox.Active = bool.Parse(reader.ReadLine());
			}
		}
		catch{}
	}
	
	private void saveSettings()
	{
		try
		{
			using (var file = new FileStream("Settings.txt", FileMode.Create, FileAccess.Write))
			using (var writer = new StreamWriter(file))
			{
				writer.WriteLine(csProjTextBox.Text);
				writer.WriteLine(shaderDirTextBox.Text);
				writer.WriteLine(compileTypesComboBox.Active.ToString());
				writer.WriteLine(compileMetroShadersCheckBox.Active.ToString());
			}
		}
		catch{}
	}
	
	private void compileButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			var compiler = new Compiler(csProjTextBox.Text);
			if (compileTypesComboBox.ActiveText == "All")
			{
				compileProjectType(compiler, "D3D11", false);
				compileProjectType(compiler, "D3D9", false);
				compileProjectType(compiler, "XNA", false);
				compileProjectType(compiler, "GL3", false);
				compileProjectType(compiler, "GL2", false);
				compileProjectType(compiler, "GLES2", true);
			}
			else
			{
				compileProjectType(compiler, compileTypesComboBox.ActiveText, true);
			}
		}
		catch (Exception ex)
		{
			var dlg = new Dialog("Error", this, DialogFlags.Modal, ResponseType.Ok);
			dlg.VBox.Add (new Label (ex.Message));
			dlg.ShowAll ();
			dlg.Run();
			dlg.Destroy();
		}
	}
	
	private void compileProjectType(Compiler compiler, string type, bool compileMaterial)
	{
		string tag = null;
		var outputType = CompilerOutputs.GL2;
		switch (type)
		{
			case ("D3D11"):
				tag = "D3D11_";
				outputType = CompilerOutputs.D3D11;
				break;
				
			case ("D3D9"):
				tag = "D3D9_";
				outputType = CompilerOutputs.D3D9;
				break;
				
			case ("XNA"):
				tag = "XNA_";
				outputType = CompilerOutputs.XNA;
				break;
				
			case ("GL3"):
				tag = "GL3_";
				outputType = CompilerOutputs.GL3;
				break;
				
			case ("GL2"):
				tag = "GL2_";
				outputType = CompilerOutputs.GL2;
				break;
				
			case ("GLES2"):
				tag = "GLES2_";
				outputType = CompilerOutputs.GLES2;
				break;
		}
		
		compiler.FileTag = tag;
		compiler.Compile(shaderDirTextBox.Text, outputType, compileMaterial, compileMetroShadersCheckBox.Active);
	}
}
