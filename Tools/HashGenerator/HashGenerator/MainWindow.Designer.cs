namespace HashGenerator
{
	partial class MainWindow
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
			this.fileNameTextBox = new System.Windows.Forms.TextBox();
			this.generateButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.hashTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.hexTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.guidTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// fileNameTextBox
			// 
			this.fileNameTextBox.Location = new System.Drawing.Point(75, 12);
			this.fileNameTextBox.Name = "fileNameTextBox";
			this.fileNameTextBox.Size = new System.Drawing.Size(367, 20);
			this.fileNameTextBox.TabIndex = 0;
			// 
			// generateButton
			// 
			this.generateButton.Location = new System.Drawing.Point(367, 38);
			this.generateButton.Name = "generateButton";
			this.generateButton.Size = new System.Drawing.Size(75, 23);
			this.generateButton.TabIndex = 1;
			this.generateButton.Text = "Generate";
			this.generateButton.UseVisualStyleBackColor = true;
			this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "File Name:";
			// 
			// hashTextBox
			// 
			this.hashTextBox.Location = new System.Drawing.Point(75, 67);
			this.hashTextBox.Name = "hashTextBox";
			this.hashTextBox.Size = new System.Drawing.Size(367, 20);
			this.hashTextBox.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Hash";
			// 
			// hexTextBox
			// 
			this.hexTextBox.Location = new System.Drawing.Point(75, 93);
			this.hexTextBox.Name = "hexTextBox";
			this.hexTextBox.Size = new System.Drawing.Size(367, 20);
			this.hexTextBox.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 93);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Hex";
			// 
			// guidTextBox
			// 
			this.guidTextBox.Location = new System.Drawing.Point(90, 156);
			this.guidTextBox.Name = "guidTextBox";
			this.guidTextBox.Size = new System.Drawing.Size(352, 20);
			this.guidTextBox.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 156);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Random Guid";
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(454, 188);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.guidTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.hexTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.hashTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.generateButton);
			this.Controls.Add(this.fileNameTextBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Hash Generator";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox fileNameTextBox;
		private System.Windows.Forms.Button generateButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox hashTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox hexTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox guidTextBox;
		private System.Windows.Forms.Label label4;
	}
}

