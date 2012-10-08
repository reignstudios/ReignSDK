namespace ModelConverter
{
	partial class ToolWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.openButton = new System.Windows.Forms.Button();
			this.convertButton = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.modelFileTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.contentTextBox = new System.Windows.Forms.TextBox();
			this.loadButton = new System.Windows.Forms.Button();
			this.materialListBox = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.removeBinderButton = new System.Windows.Forms.Button();
			this.addBinderButton = new System.Windows.Forms.Button();
			this.materialTextureBinderListBox = new System.Windows.Forms.ListBox();
			this.shaderConstantComboBox = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textureIDComboBox = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.shaderMaterialComboBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.saveTriangleMesh = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// openButton
			// 
			this.openButton.Location = new System.Drawing.Point(12, 12);
			this.openButton.Name = "openButton";
			this.openButton.Size = new System.Drawing.Size(75, 23);
			this.openButton.TabIndex = 0;
			this.openButton.Text = "Open";
			this.openButton.UseVisualStyleBackColor = true;
			this.openButton.Click += new System.EventHandler(this.openButton_Click);
			// 
			// convertButton
			// 
			this.convertButton.Location = new System.Drawing.Point(174, 12);
			this.convertButton.Name = "convertButton";
			this.convertButton.Size = new System.Drawing.Size(75, 23);
			this.convertButton.TabIndex = 1;
			this.convertButton.Text = "Convert";
			this.convertButton.UseVisualStyleBackColor = true;
			this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(255, 12);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(75, 23);
			this.saveButton.TabIndex = 2;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// modelFileTextBox
			// 
			this.modelFileTextBox.Location = new System.Drawing.Point(62, 41);
			this.modelFileTextBox.Name = "modelFileTextBox";
			this.modelFileTextBox.Size = new System.Drawing.Size(469, 20);
			this.modelFileTextBox.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(39, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Model:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 74);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Content:";
			// 
			// contentTextBox
			// 
			this.contentTextBox.Location = new System.Drawing.Point(62, 67);
			this.contentTextBox.Name = "contentTextBox";
			this.contentTextBox.Size = new System.Drawing.Size(469, 20);
			this.contentTextBox.TabIndex = 6;
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(93, 12);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(75, 23);
			this.loadButton.TabIndex = 7;
			this.loadButton.Text = "Load";
			this.loadButton.UseVisualStyleBackColor = true;
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// materialListBox
			// 
			this.materialListBox.FormattingEnabled = true;
			this.materialListBox.Location = new System.Drawing.Point(6, 19);
			this.materialListBox.Name = "materialListBox";
			this.materialListBox.Size = new System.Drawing.Size(200, 186);
			this.materialListBox.TabIndex = 8;
			this.materialListBox.SelectedIndexChanged += new System.EventHandler(this.materialListBox_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.removeBinderButton);
			this.groupBox1.Controls.Add(this.addBinderButton);
			this.groupBox1.Controls.Add(this.materialTextureBinderListBox);
			this.groupBox1.Controls.Add(this.shaderConstantComboBox);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textureIDComboBox);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.shaderMaterialComboBox);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.materialListBox);
			this.groupBox1.Location = new System.Drawing.Point(12, 93);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(519, 220);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Materials";
			// 
			// removeBinderButton
			// 
			this.removeBinderButton.Location = new System.Drawing.Point(212, 158);
			this.removeBinderButton.Name = "removeBinderButton";
			this.removeBinderButton.Size = new System.Drawing.Size(75, 23);
			this.removeBinderButton.TabIndex = 18;
			this.removeBinderButton.Text = "Remove";
			this.removeBinderButton.UseVisualStyleBackColor = true;
			this.removeBinderButton.Click += new System.EventHandler(this.removeBinderButton_Click);
			// 
			// addBinderButton
			// 
			this.addBinderButton.Location = new System.Drawing.Point(212, 129);
			this.addBinderButton.Name = "addBinderButton";
			this.addBinderButton.Size = new System.Drawing.Size(75, 23);
			this.addBinderButton.TabIndex = 17;
			this.addBinderButton.Text = "Add";
			this.addBinderButton.UseVisualStyleBackColor = true;
			this.addBinderButton.Click += new System.EventHandler(this.addBinderButton_Click);
			// 
			// materialTextureBinderListBox
			// 
			this.materialTextureBinderListBox.FormattingEnabled = true;
			this.materialTextureBinderListBox.Location = new System.Drawing.Point(307, 129);
			this.materialTextureBinderListBox.Name = "materialTextureBinderListBox";
			this.materialTextureBinderListBox.Size = new System.Drawing.Size(206, 82);
			this.materialTextureBinderListBox.TabIndex = 16;
			// 
			// shaderConstantComboBox
			// 
			this.shaderConstantComboBox.FormattingEnabled = true;
			this.shaderConstantComboBox.Location = new System.Drawing.Point(307, 102);
			this.shaderConstantComboBox.Name = "shaderConstantComboBox";
			this.shaderConstantComboBox.Size = new System.Drawing.Size(206, 21);
			this.shaderConstantComboBox.TabIndex = 15;
			this.shaderConstantComboBox.SelectedIndexChanged += new System.EventHandler(this.shaderConstantComboBox_SelectedIndexChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(212, 110);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(89, 13);
			this.label6.TabIndex = 14;
			this.label6.Text = "Shader Constant:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(212, 83);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(57, 13);
			this.label5.TabIndex = 13;
			this.label5.Text = "TextureID:";
			// 
			// textureIDComboBox
			// 
			this.textureIDComboBox.FormattingEnabled = true;
			this.textureIDComboBox.Location = new System.Drawing.Point(307, 75);
			this.textureIDComboBox.Name = "textureIDComboBox";
			this.textureIDComboBox.Size = new System.Drawing.Size(206, 21);
			this.textureIDComboBox.TabIndex = 12;
			this.textureIDComboBox.SelectedIndexChanged += new System.EventHandler(this.textureIDComboBox_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(212, 59);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(170, 13);
			this.label4.TabIndex = 11;
			this.label4.Text = "Bind TextureID to Shader Contant:";
			// 
			// shaderMaterialComboBox
			// 
			this.shaderMaterialComboBox.FormattingEnabled = true;
			this.shaderMaterialComboBox.Location = new System.Drawing.Point(215, 35);
			this.shaderMaterialComboBox.Name = "shaderMaterialComboBox";
			this.shaderMaterialComboBox.Size = new System.Drawing.Size(298, 21);
			this.shaderMaterialComboBox.TabIndex = 10;
			this.shaderMaterialComboBox.SelectedIndexChanged += new System.EventHandler(this.shaderMaterialComboBox_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(212, 19);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(84, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Shader Material:";
			// 
			// saveTriangleMesh
			// 
			this.saveTriangleMesh.Location = new System.Drawing.Point(402, 12);
			this.saveTriangleMesh.Name = "saveTriangleMesh";
			this.saveTriangleMesh.Size = new System.Drawing.Size(123, 23);
			this.saveTriangleMesh.TabIndex = 10;
			this.saveTriangleMesh.Text = "Save TriangleMesh";
			this.saveTriangleMesh.UseVisualStyleBackColor = true;
			this.saveTriangleMesh.Click += new System.EventHandler(this.saveTriangleMesh_Click);
			// 
			// ToolWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(543, 322);
			this.Controls.Add(this.saveTriangleMesh);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.loadButton);
			this.Controls.Add(this.contentTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.modelFileTextBox);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.convertButton);
			this.Controls.Add(this.openButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ToolWindow";
			this.Text = "Tool Window";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button openButton;
		private System.Windows.Forms.Button convertButton;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.TextBox modelFileTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox contentTextBox;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.ListBox materialListBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox shaderMaterialComboBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox textureIDComboBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListBox materialTextureBinderListBox;
		private System.Windows.Forms.ComboBox shaderConstantComboBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button removeBinderButton;
		private System.Windows.Forms.Button addBinderButton;
		private System.Windows.Forms.Button saveTriangleMesh;
	}
}