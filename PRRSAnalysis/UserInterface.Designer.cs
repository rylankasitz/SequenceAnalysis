﻿namespace PRRSAnalysis
{
    partial class UserInterface
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserInterface));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.uxFileToolbar = new System.Windows.Forms.ToolStripDropDownButton();
            this.openDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uxOpenFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.uxOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.addDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.uxAddFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.uxRunToolbar = new System.Windows.Forms.ToolStripDropDownButton();
            this.uxRunFullAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.uxOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.uxFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.uxSequenceList = new System.Windows.Forms.CheckedListBox();
            this.uxSettingsLabel = new System.Windows.Forms.Label();
            this.uxMafftSettingsLabel = new System.Windows.Forms.Label();
            this.uxAlignmentType = new System.Windows.Forms.ComboBox();
            this.uxMinOrfLenghtLabel = new System.Windows.Forms.Label();
            this.uxMinOrfLengthTextbox = new System.Windows.Forms.TextBox();
            this.uxVaccineLocationLabel = new System.Windows.Forms.Label();
            this.uxVaccineLocationTextBox = new System.Windows.Forms.TextBox();
            this.uxVaccineLocationButton = new System.Windows.Forms.Button();
            this.uxProgressBar = new System.Windows.Forms.ProgressBar();
            this.uxOutputLocationTextBox = new System.Windows.Forms.TextBox();
            this.uxOutputLocationButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxFileToolbar,
            this.uxRunToolbar});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(722, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // uxFileToolbar
            // 
            this.uxFileToolbar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.uxFileToolbar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.uxFileToolbar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDataToolStripMenuItem,
            this.addDataToolStripMenuItem});
            this.uxFileToolbar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxFileToolbar.Name = "uxFileToolbar";
            this.uxFileToolbar.Size = new System.Drawing.Size(38, 22);
            this.uxFileToolbar.Text = "File";
            this.uxFileToolbar.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            // 
            // openDataToolStripMenuItem
            // 
            this.openDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxOpenFiles,
            this.uxOpenFolder});
            this.openDataToolStripMenuItem.Name = "openDataToolStripMenuItem";
            this.openDataToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.openDataToolStripMenuItem.Text = "Open Data";
            // 
            // uxOpenFiles
            // 
            this.uxOpenFiles.Name = "uxOpenFiles";
            this.uxOpenFiles.Size = new System.Drawing.Size(107, 22);
            this.uxOpenFiles.Text = "File(s)";
            this.uxOpenFiles.Click += new System.EventHandler(this.uxOpenFiles_Click);
            // 
            // uxOpenFolder
            // 
            this.uxOpenFolder.Name = "uxOpenFolder";
            this.uxOpenFolder.Size = new System.Drawing.Size(107, 22);
            this.uxOpenFolder.Text = "Folder";
            this.uxOpenFolder.Click += new System.EventHandler(this.uxOpenFolder_Click);
            // 
            // addDataToolStripMenuItem
            // 
            this.addDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxAddFiles,
            this.uxAddFolder});
            this.addDataToolStripMenuItem.Name = "addDataToolStripMenuItem";
            this.addDataToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.addDataToolStripMenuItem.Text = "Add Data";
            // 
            // uxAddFiles
            // 
            this.uxAddFiles.Name = "uxAddFiles";
            this.uxAddFiles.Size = new System.Drawing.Size(107, 22);
            this.uxAddFiles.Text = "File(s)";
            this.uxAddFiles.Click += new System.EventHandler(this.uxAddFiles_Click);
            // 
            // uxAddFolder
            // 
            this.uxAddFolder.Name = "uxAddFolder";
            this.uxAddFolder.Size = new System.Drawing.Size(107, 22);
            this.uxAddFolder.Text = "Folder";
            this.uxAddFolder.Click += new System.EventHandler(this.uxAddFolder_Click);
            // 
            // uxRunToolbar
            // 
            this.uxRunToolbar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.uxRunToolbar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxRunFullAnalysis});
            this.uxRunToolbar.Image = ((System.Drawing.Image)(resources.GetObject("uxRunToolbar.Image")));
            this.uxRunToolbar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxRunToolbar.Name = "uxRunToolbar";
            this.uxRunToolbar.Size = new System.Drawing.Size(41, 22);
            this.uxRunToolbar.Text = "Run";
            // 
            // uxRunFullAnalysis
            // 
            this.uxRunFullAnalysis.Name = "uxRunFullAnalysis";
            this.uxRunFullAnalysis.Size = new System.Drawing.Size(180, 22);
            this.uxRunFullAnalysis.Text = "Full Analysis";
            this.uxRunFullAnalysis.Click += new System.EventHandler(this.uxRunFullAnalysis_Click);
            // 
            // uxOpenFileDialog
            // 
            this.uxOpenFileDialog.FileName = "uxOpenFileDialog";
            // 
            // uxSequenceList
            // 
            this.uxSequenceList.CheckOnClick = true;
            this.uxSequenceList.FormattingEnabled = true;
            this.uxSequenceList.Location = new System.Drawing.Point(0, 28);
            this.uxSequenceList.Name = "uxSequenceList";
            this.uxSequenceList.Size = new System.Drawing.Size(242, 424);
            this.uxSequenceList.TabIndex = 1;
            // 
            // uxSettingsLabel
            // 
            this.uxSettingsLabel.AutoSize = true;
            this.uxSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uxSettingsLabel.Location = new System.Drawing.Point(248, 28);
            this.uxSettingsLabel.Name = "uxSettingsLabel";
            this.uxSettingsLabel.Size = new System.Drawing.Size(68, 20);
            this.uxSettingsLabel.TabIndex = 2;
            this.uxSettingsLabel.Text = "Settings";
            // 
            // uxMafftSettingsLabel
            // 
            this.uxMafftSettingsLabel.AutoSize = true;
            this.uxMafftSettingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uxMafftSettingsLabel.Location = new System.Drawing.Point(250, 116);
            this.uxMafftSettingsLabel.Name = "uxMafftSettingsLabel";
            this.uxMafftSettingsLabel.Size = new System.Drawing.Size(97, 13);
            this.uxMafftSettingsLabel.TabIndex = 3;
            this.uxMafftSettingsLabel.Text = "Alignment (MAFFT)";
            // 
            // uxAlignmentType
            // 
            this.uxAlignmentType.FormattingEnabled = true;
            this.uxAlignmentType.Items.AddRange(new object[] {
            "Accurate",
            "Fast",
            "Very Fast"});
            this.uxAlignmentType.Location = new System.Drawing.Point(353, 113);
            this.uxAlignmentType.Name = "uxAlignmentType";
            this.uxAlignmentType.Size = new System.Drawing.Size(96, 21);
            this.uxAlignmentType.TabIndex = 4;
            this.uxAlignmentType.SelectedIndexChanged += new System.EventHandler(this.uxAlignmentType_SelectedIndexChanged);
            // 
            // uxMinOrfLenghtLabel
            // 
            this.uxMinOrfLenghtLabel.AutoSize = true;
            this.uxMinOrfLenghtLabel.Location = new System.Drawing.Point(248, 59);
            this.uxMinOrfLenghtLabel.Name = "uxMinOrfLenghtLabel";
            this.uxMinOrfLenghtLabel.Size = new System.Drawing.Size(99, 13);
            this.uxMinOrfLenghtLabel.TabIndex = 9;
            this.uxMinOrfLenghtLabel.Text = "Minimun Orf Length";
            // 
            // uxMinOrfLengthTextbox
            // 
            this.uxMinOrfLengthTextbox.Location = new System.Drawing.Point(353, 56);
            this.uxMinOrfLengthTextbox.Name = "uxMinOrfLengthTextbox";
            this.uxMinOrfLengthTextbox.Size = new System.Drawing.Size(77, 20);
            this.uxMinOrfLengthTextbox.TabIndex = 10;
            this.uxMinOrfLengthTextbox.TextChanged += new System.EventHandler(this.uxMinOrfLengthTextbox_TextChanged);
            // 
            // uxVaccineLocationLabel
            // 
            this.uxVaccineLocationLabel.AutoSize = true;
            this.uxVaccineLocationLabel.Location = new System.Drawing.Point(250, 150);
            this.uxVaccineLocationLabel.Name = "uxVaccineLocationLabel";
            this.uxVaccineLocationLabel.Size = new System.Drawing.Size(101, 13);
            this.uxVaccineLocationLabel.TabIndex = 12;
            this.uxVaccineLocationLabel.Text = "Vaccine(s) Location";
            // 
            // uxVaccineLocationTextBox
            // 
            this.uxVaccineLocationTextBox.Location = new System.Drawing.Point(353, 145);
            this.uxVaccineLocationTextBox.Name = "uxVaccineLocationTextBox";
            this.uxVaccineLocationTextBox.Size = new System.Drawing.Size(325, 20);
            this.uxVaccineLocationTextBox.TabIndex = 13;
            this.uxVaccineLocationTextBox.TextChanged += new System.EventHandler(this.uxVaccineLocationTextBox_TextChanged);
            // 
            // uxVaccineLocationButton
            // 
            this.uxVaccineLocationButton.Location = new System.Drawing.Point(684, 143);
            this.uxVaccineLocationButton.Name = "uxVaccineLocationButton";
            this.uxVaccineLocationButton.Size = new System.Drawing.Size(26, 23);
            this.uxVaccineLocationButton.TabIndex = 14;
            this.uxVaccineLocationButton.Text = "...";
            this.uxVaccineLocationButton.UseVisualStyleBackColor = true;
            this.uxVaccineLocationButton.Click += new System.EventHandler(this.uxVaccineLocationButton_Click);
            // 
            // uxProgressBar
            // 
            this.uxProgressBar.Location = new System.Drawing.Point(252, 415);
            this.uxProgressBar.Maximum = 1000;
            this.uxProgressBar.Name = "uxProgressBar";
            this.uxProgressBar.Size = new System.Drawing.Size(458, 23);
            this.uxProgressBar.TabIndex = 15;
            // 
            // uxOutputLocationTextBox
            // 
            this.uxOutputLocationTextBox.Location = new System.Drawing.Point(353, 181);
            this.uxOutputLocationTextBox.Name = "uxOutputLocationTextBox";
            this.uxOutputLocationTextBox.Size = new System.Drawing.Size(327, 20);
            this.uxOutputLocationTextBox.TabIndex = 16;
            this.uxOutputLocationTextBox.TextChanged += new System.EventHandler(this.uxOutputLocationTextBox_TextChanged);
            // 
            // uxOutputLocationButton
            // 
            this.uxOutputLocationButton.Location = new System.Drawing.Point(686, 179);
            this.uxOutputLocationButton.Name = "uxOutputLocationButton";
            this.uxOutputLocationButton.Size = new System.Drawing.Size(26, 23);
            this.uxOutputLocationButton.TabIndex = 17;
            this.uxOutputLocationButton.Text = "...";
            this.uxOutputLocationButton.UseVisualStyleBackColor = true;
            this.uxOutputLocationButton.Click += new System.EventHandler(this.uxOutputLocationButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(250, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Output Location";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox1.Location = new System.Drawing.Point(248, 86);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(121, 17);
            this.checkBox1.TabIndex = 19;
            this.checkBox1.Text = "Partial Orf File          ";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // UserInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 450);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uxOutputLocationButton);
            this.Controls.Add(this.uxOutputLocationTextBox);
            this.Controls.Add(this.uxProgressBar);
            this.Controls.Add(this.uxVaccineLocationButton);
            this.Controls.Add(this.uxVaccineLocationTextBox);
            this.Controls.Add(this.uxVaccineLocationLabel);
            this.Controls.Add(this.uxMinOrfLengthTextbox);
            this.Controls.Add(this.uxMinOrfLenghtLabel);
            this.Controls.Add(this.uxAlignmentType);
            this.Controls.Add(this.uxMafftSettingsLabel);
            this.Controls.Add(this.uxSettingsLabel);
            this.Controls.Add(this.uxSequenceList);
            this.Controls.Add(this.toolStrip1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(738, 489);
            this.MinimumSize = new System.Drawing.Size(738, 489);
            this.Name = "UserInterface";
            this.Text = "Sequence Analysis Tool";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton uxFileToolbar;
        private System.Windows.Forms.ToolStripMenuItem openDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uxOpenFiles;
        private System.Windows.Forms.ToolStripMenuItem uxOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem addDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uxAddFiles;
        private System.Windows.Forms.ToolStripMenuItem uxAddFolder;
        private System.Windows.Forms.ToolStripDropDownButton uxRunToolbar;
        private System.Windows.Forms.ToolStripMenuItem uxRunFullAnalysis;
        private System.Windows.Forms.OpenFileDialog uxOpenFileDialog;
        private System.Windows.Forms.FolderBrowserDialog uxFolderDialog;
        private System.Windows.Forms.CheckedListBox uxSequenceList;
        private System.Windows.Forms.Label uxSettingsLabel;
        private System.Windows.Forms.Label uxMafftSettingsLabel;
        private System.Windows.Forms.ComboBox uxAlignmentType;
        private System.Windows.Forms.Label uxMinOrfLenghtLabel;
        private System.Windows.Forms.TextBox uxMinOrfLengthTextbox;
        private System.Windows.Forms.Label uxVaccineLocationLabel;
        private System.Windows.Forms.TextBox uxVaccineLocationTextBox;
        private System.Windows.Forms.Button uxVaccineLocationButton;
        public System.Windows.Forms.ProgressBar uxProgressBar;
        private System.Windows.Forms.TextBox uxOutputLocationTextBox;
        private System.Windows.Forms.Button uxOutputLocationButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

