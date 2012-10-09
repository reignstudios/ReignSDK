using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Reign.Video;

namespace ModelConverter
{
	public partial class ToolWindow : Form
	{
		class SoftwareMaterialWrapper
		{
			public SoftwareMaterial Material;
			public Type ShaderMaterialType;
			public KeyValuePair<string,string> Texture;
			public string ShaderConstant;

			public SoftwareMaterialWrapper(SoftwareMaterial material)
			{
				Material = material;
			}

			public override string ToString()
			{
				return Material.Name;
			}
		}

		class MaterialTextureBinderWrapper
		{
			public MaterialFieldBinder Binder;

			public MaterialTextureBinderWrapper(MaterialFieldBinder binder)
			{
				Binder = binder;
			}

			public override string ToString()
			{
				return string.Format("{0}, {1}, {2}", Binder.MaterialName, Binder.ID, Binder.ShaderMaterialFieldName);
			}
		}

		MainWindow mainWindow;

		public ToolWindow(MainWindow mainWindow)
		{
			InitializeComponent();
			this.mainWindow = mainWindow;

			try
			{
				if (new FileInfo("Settings.txt").Exists == false) return;
				using (var file = new FileStream("Settings.txt", FileMode.Open, FileAccess.Read))
				using (var reader = new StreamReader(file))
				{
					modelFileTextBox.Text = reader.ReadLine();
					contentTextBox.Text = reader.ReadLine();
				}
			}
			catch {}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			try
			{
				using (var file = new FileStream("Settings.txt", FileMode.Create, FileAccess.Write))
				using (var writer = new StreamWriter(file))
				{
					writer.WriteLine(modelFileTextBox.Text);
					writer.WriteLine(contentTextBox.Text);
				}
			}
			catch {}
			base.OnClosing(e);
		}

		private void openButton_Click(object sender, EventArgs e)
		{
			var dlg = new OpenFileDialog();
			if (dlg.ShowDialog() != DialogResult.OK) return;

			var contentDlg = new FolderBrowserDialog();
			if (contentDlg.ShowDialog() != DialogResult.OK) return;
			
			string contentPath = contentDlg.SelectedPath;
			if (!string.IsNullOrEmpty(contentPath))
			{
				char c = contentPath[contentPath.Length-1];
				contentPath += (c == '/' || c == '\\') ? "" : "\\";
			}

			modelFileTextBox.Text = dlg.FileName;
			contentTextBox.Text = contentPath;
		}

		private void loadButton_Click(object sender, EventArgs e)
		{
			mainWindow.Load(modelFileTextBox.Text);
		}

		private void convertButton_Click(object sender, EventArgs e)
		{
			var materialTypes = new Dictionary<string,Type>();
			foreach (var material in materialListBox.Items)
			{
				var mat = ((SoftwareMaterialWrapper)material);
				materialTypes.Add(mat.Material.Name, mat.ShaderMaterialType);
			}

			var materialFieldTypes = new List<MaterialFieldBinder>();
			foreach (var binder in materialTextureBinderListBox.Items)
			{
				var b = (MaterialTextureBinderWrapper)binder;
				materialFieldTypes.Add(b.Binder);
			}

			mainWindow.Convert(contentTextBox.Text, materialTypes, materialFieldTypes);
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			var dlg = new SaveFileDialog();
			if (dlg.ShowDialog() != DialogResult.OK) return;

			mainWindow.Save(dlg.FileName, saveColorsCheckBox.Checked, saveUVsCheckBox.Checked, saveNormalsCheckBox.Checked);
		}

		private void saveTriangleMesh_Click(object sender, EventArgs e)
		{
			var dlg = new SaveFileDialog();
			if (dlg.ShowDialog() != DialogResult.OK) return;

			mainWindow.SaveTriangleMesh(dlg.FileName);
		}

		public void LoadSoftwareModelData(SoftwareModel softwareModel, List<Type> materials)
		{
			materialListBox.Items.Clear();
			foreach (var material in softwareModel.Materials)
			{
				materialListBox.Items.Add(new SoftwareMaterialWrapper(material));
			}

			shaderMaterialComboBox.Items.Clear();
			foreach (var material in materials)
			{
				shaderMaterialComboBox.Items.Add(material);
			}
		}

		private void materialListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (materialListBox.SelectedIndex == -1) return;
			var material = (SoftwareMaterialWrapper)materialListBox.SelectedItem;

			// set current states to UI
			shaderMaterialComboBox.SelectedItem = material.ShaderMaterialType;

			textureIDComboBox.SelectedIndex = -1;
			textureIDComboBox.Items.Clear();
			foreach (var texture in material.Material.Textures)
			{
			    textureIDComboBox.Items.Add(texture);
			}

			fillShaderConstants();
		}

		private void shaderMaterialComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (shaderMaterialComboBox.SelectedIndex == -1) return;
			if (materialListBox.SelectedIndex == -1) return;
			var material = (SoftwareMaterialWrapper)materialListBox.SelectedItem;

			// update current states
			material.ShaderMaterialType = (Type)shaderMaterialComboBox.SelectedItem;
			fillShaderConstants();
		}

		private void fillShaderConstants()
		{
			if (shaderMaterialComboBox.SelectedIndex == -1) return;

			shaderConstantComboBox.SelectedIndex = -1;
			shaderConstantComboBox.Items.Clear();
			var item = (Type)shaderMaterialComboBox.SelectedItem;
			foreach (var field in item.GetFields())
			{
				if (field.IsStatic || !(field.FieldType == typeof(Texture2DI) || field.FieldType == typeof(Texture3DI))) continue;
				shaderConstantComboBox.Items.Add(field.Name);
			}
		}

		private void textureIDComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (textureIDComboBox.SelectedIndex == -1) return;
			if (materialListBox.SelectedIndex == -1) return;
			var material = (SoftwareMaterialWrapper)materialListBox.SelectedItem;

			material.Texture = (KeyValuePair<string,string>)textureIDComboBox.SelectedItem;
		}

		private void shaderConstantComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (shaderConstantComboBox.SelectedIndex == -1) return;
			if (materialListBox.SelectedIndex == -1) return;
			var material = (SoftwareMaterialWrapper)materialListBox.SelectedItem;

			material.ShaderConstant = (string)shaderConstantComboBox.SelectedItem;
		}

		private void addBinderButton_Click(object sender, EventArgs e)
		{
			if (materialListBox.SelectedIndex == -1) return;
			if (shaderMaterialComboBox.SelectedIndex == -1) return;
			if (textureIDComboBox.SelectedIndex == -1) return;
			if (shaderConstantComboBox.SelectedIndex == -1) return;

			var material = (SoftwareMaterialWrapper)materialListBox.SelectedItem;
			materialTextureBinderListBox.Items.Add(new MaterialTextureBinderWrapper(new MaterialFieldBinder(material.Material.Name, material.Texture.Key, material.ShaderConstant)));
		}

		private void removeBinderButton_Click(object sender, EventArgs e)
		{
			if (materialTextureBinderListBox.SelectedIndex == -1) return;
			materialTextureBinderListBox.Items.Remove(materialTextureBinderListBox.SelectedItem);
		}
	}
}
