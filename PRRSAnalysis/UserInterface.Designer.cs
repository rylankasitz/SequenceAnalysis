namespace PRRSAnalysis
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
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // uxFileToolbar
            // 
            this.uxFileToolbar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.uxFileToolbar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uxFileToolbar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDataToolStripMenuItem,
            this.addDataToolStripMenuItem});
            this.uxFileToolbar.Image = ((System.Drawing.Image)(resources.GetObject("uxFileToolbar.Image")));
            this.uxFileToolbar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxFileToolbar.Name = "uxFileToolbar";
            this.uxFileToolbar.Size = new System.Drawing.Size(29, 22);
            this.uxFileToolbar.Text = "File";
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
            this.uxRunToolbar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uxRunToolbar.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxRunFullAnalysis});
            this.uxRunToolbar.Image = ((System.Drawing.Image)(resources.GetObject("uxRunToolbar.Image")));
            this.uxRunToolbar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uxRunToolbar.Name = "uxRunToolbar";
            this.uxRunToolbar.Size = new System.Drawing.Size(29, 22);
            this.uxRunToolbar.Text = "toolStripDropDownButton1";
            // 
            // uxRunFullAnalysis
            // 
            this.uxRunFullAnalysis.Name = "uxRunFullAnalysis";
            this.uxRunFullAnalysis.Size = new System.Drawing.Size(139, 22);
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
            // UserInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.uxSequenceList);
            this.Controls.Add(this.toolStrip1);
            this.Name = "UserInterface";
            this.Text = "Form1";
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
    }
}

