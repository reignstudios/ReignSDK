using System;
using Gtk;
using System.IO;
using Reign.Video;
using Reign.Core;

public partial class MainWindow: Gtk.Window
{
	SoftwareModel softwareModel;

	public MainWindow ()
	: base (Gtk.WindowType.Toplevel)
	{
		Build ();

		loadSettings ();
		convertButton.Clicked += compileButton_Clicked;
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		saveSettings ();
		Application.Quit ();
		a.RetVal = true;
	}

	private void loadSettings()
	{
		try
		{
			if (!new FileInfo("Settings.txt").Exists)
			{
				openTextBox.Text = "Enter .rmx path here...";
				saveTextBox.Text = "Enter .rm output path here...";
				return;
			}
			
			using (var file = new FileStream("Settings.txt", FileMode.OpenOrCreate, FileAccess.Read))
				using (var reader = new StreamReader(file))
			{
				openTextBox.Text = reader.ReadLine();
				saveTextBox.Text = reader.ReadLine();
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
				writer.WriteLine(openTextBox.Text);
				writer.WriteLine(saveTextBox.Text);
			}
		}
		catch{}
	}

	private void compileButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			softwareModel = new SoftwareModel (openTextBox.Text, softwareModelLoaded);
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

	private void softwareModelLoaded(object sender, bool succeeded)
	{
		if (succeeded)
		{
			var sModel = (SoftwareModel)sender;
			sModel.Rotate(MathUtilities.DegToRad(float.Parse(rotX.Text)), MathUtilities.DegToRad(float.Parse(rotY.Text)), MathUtilities.DegToRad(float.Parse(rotZ.Text)));
			Model.Save(saveTextBox.Text, false, sModel, useColorsCheckBox.Active, useUVCheckBox.Active, useNormalCheckBox.Active);
		}
		else
		{
			var dlg = new Dialog("Error", this, DialogFlags.Modal, ResponseType.Ok);
			dlg.VBox.Add (new Label ("Failed to open SoftwareModel!"));
			dlg.ShowAll ();
			dlg.Run();
			dlg.Destroy();
		}
	}
}
