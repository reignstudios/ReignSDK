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
using System.IO;

using ShaderCompiler.Core;
using Microsoft.Win32;

namespace ShaderCompiler
{
	public partial class MainWindow : Window
	{
		const string testShaderCode =
@"
using ShaderCompiler.Core;

namespace ShadersCS
{
	public sealed class MyShader : ShaderI
	{
		[VSInput(VSInputTypes.Position, 0)] public Vector3 Position_VS;
		[VSInput(VSInputTypes.UV, 0)] public Vector2 UV_VS;

		[VSOutputPSInput(VSOutputPSInputTypes.Position, 0)] public Vector4 Position_VSPS;
		[VSOutputPSInput(VSOutputPSInputTypes.InOut, 0)] public Vector2 UV_VSPS;

		[PSOutput(PSOutputTypes.Color, 0)] public Vector4 Color_PS;

		[FieldUsage(FieldUsageTypes.PS)] public Texture2D DiffuseTexture;
		[FieldUsage(FieldUsageTypes.VS)] public Matrix4 Camera;
		[FieldUsage(FieldUsageTypes.VS_PS)] public Vector3 Location;
		[FieldUsage(FieldUsageTypes.PS)] public Vector4 Color;

		[ShaderMethod(ShaderMethodTypes.VS)]
		public void MainVS()
		{
			Position_VSPS = Camera.Multiply(new Vector4(Position_VS + Location, 0));
			UV_VSPS = UV_VS;
		}

		[ShaderMethod(ShaderMethodTypes.PS)]
		public void MainPS()
		{
			double dis = SL.Distance(Location.xy, UV_VSPS);
			SL.Clip(.1 - dis);

			Vector4 diffuse = DiffuseTexture.Sample(UV_VSPS);
			Color_PS = Color * diffuse;
		}
	}
}
";

		public MainWindow()
		{
			InitializeComponent();

			rsl.Text = testShaderCode;

			try
			{
				if (!new FileInfo("Settings.txt").Exists)
				{
					csProjectTextbox.Text = "Enter CS project path here...";
					rsOutputTextbox.Text = "Enter .rs/.fx output path here...";
					return;
				}

				using (var file = new FileStream("Settings.txt", FileMode.OpenOrCreate, FileAccess.Read))
				using (var reader = new StreamReader(file))
				{
					csProjectTextbox.Text = reader.ReadLine();
					rsOutputTextbox.Text = reader.ReadLine();
					compileFromCombo.SelectedIndex = int.Parse(reader.ReadLine());
					compileTypeCombo.SelectedIndex = int.Parse(reader.ReadLine());
					compileMaterialsCheckBox.IsChecked = bool.Parse(reader.ReadLine());
					compileMetroShadersCheckBox.IsChecked = bool.Parse(reader.ReadLine());
				}
			}
			catch{}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			using (var file = new FileStream("Settings.txt", FileMode.Create, FileAccess.Write))
			using (var writer = new StreamWriter(file))
			{
				writer.WriteLine(csProjectTextbox.Text);
				writer.WriteLine(rsOutputTextbox.Text);
				writer.WriteLine(compileFromCombo.SelectedIndex.ToString());
				writer.WriteLine(compileTypeCombo.SelectedIndex.ToString());
				writer.WriteLine(compileMaterialsCheckBox.IsChecked.ToString());
				writer.WriteLine(compileMetroShadersCheckBox.IsChecked.ToString());
			}

			base.OnClosing(e);
		}

		private void compileFromMemory()
		{
			var c = new Compiler(rsl.Text, CompilerCodeSources.Memory);
		
			var outputType = CompilerOutputs.GL2;
			var comboBoxValue = ((ComboBoxItem)compileTypeCombo.SelectedValue).Content as string;
			switch (comboBoxValue)
			{
				case ("D3D11"):
					outputType = CompilerOutputs.D3D11;
					break;

				case ("D3D9"):
					outputType = CompilerOutputs.D3D9;
					break;

					case ("XNA"):
					outputType = CompilerOutputs.XNA;
					break;

				case ("GL3"):
					outputType = CompilerOutputs.GL3;
					break;

				case ("GL2"):
					outputType = CompilerOutputs.GL2;
					break;

				case ("GLES2"):
					outputType = CompilerOutputs.GLES2;
					break;

				default: throw new Exception("Unsuported memory compile type.");
			}
		
			hlsl.Text = c.CompileFromMemory(rsl.Text, outputType);
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
			compiler.Compile(rsOutputTextbox.Text, outputType, compileMaterial, (bool)compileMetroShadersCheckBox.IsChecked);
		}

		private void compileProject()
		{
			var compiler = new Compiler(csProjectTextbox.Text, CompilerCodeSources.Project);
		
			var comboBoxValue = ((ComboBoxItem)compileTypeCombo.SelectedValue).Content as string;
			if (comboBoxValue == "All")
			{
				compileProjectType(compiler, "D3D11", false);
				compileProjectType(compiler, "D3D9", false);
				compileProjectType(compiler, "XNA", false);
				compileProjectType(compiler, "GL3", false);
				compileProjectType(compiler, "GL2", false);
				compileProjectType(compiler, "GLES2", (bool)compileMaterialsCheckBox.IsChecked);
			}
			else
			{
				compileProjectType(compiler, comboBoxValue, (bool)compileMaterialsCheckBox.IsChecked);
			}
		}

		private void compileButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (compileFromCombo.Text == "Memory") compileFromMemory();
				if (compileFromCombo.Text == "Project") compileProject();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				hlsl.Text = ex.Message;
				MessageBox.Show(ex.Message);
			}
		}

		private bool getFilePath(out string path)
		{
			var dlg = new System.Windows.Forms.OpenFileDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				path = dlg.FileName;
				return true;
			}

			path = null;
			return false;
		}

		private bool getFolderPath(out string path)
		{
			var dlg = new System.Windows.Forms.FolderBrowserDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				char endChar = dlg.SelectedPath[dlg.SelectedPath.Length-1];
				path = dlg.SelectedPath + ((endChar != '/' || endChar != '\\') ? "\\" : "");
				return true;
			}

			path = null;
			return false;
		}

		private void browseCSButton_Click(object sender, RoutedEventArgs e)
		{
			string path;
			if (!getFilePath(out path)) return;

			csProjectTextbox.Text = path;
		}

		private void browseRSButton_Click(object sender, RoutedEventArgs e)
		{
			string path;
			if (!getFolderPath(out path)) return;

			rsOutputTextbox.Text = path;
		}
	}
}
