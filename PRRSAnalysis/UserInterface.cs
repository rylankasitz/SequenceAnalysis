using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PRRSAnalysis.DataStorage;

namespace PRRSAnalysis
{
    public partial class UserInterface : Form
    {
        private DataManager _dataManager;
        private RunDelegate _runAnalysis;

        public UserInterface(DataManager dataManager, RunDelegate run)
        {
            InitializeComponent();

            _dataManager = dataManager;
            _runAnalysis = run;
        }

        #region File Menu Methods

        private void uxOpenFiles_Click(object sender, EventArgs e)
        {
            _dataManager.ClearAllSequenceData();
            loadFileViewer();
        }
        private void uxOpenFolder_Click(object sender, EventArgs e)
        {
            _dataManager.ClearAllSequenceData();
            loadFolderViewer();
        }
        private void uxAddFiles_Click(object sender, EventArgs e)
        {
            loadFileViewer();
        }   
        private void uxAddFolder_Click(object sender, EventArgs e)
        {
            loadFolderViewer();
        }
        private void loadFileViewer()
        {
            try
            {
                if (uxOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in uxOpenFileDialog.FileNames)
                    {
                        _dataManager.AddSequencesFromFile(file);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }
        private void loadFolderViewer()
        {
            try
            {
                if (uxFolderDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(uxFolderDialog.SelectedPath))
                {
                    foreach (string file in Directory.GetFiles(uxFolderDialog.SelectedPath))
                    {
                        _dataManager.AddSequencesFromFile(file);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        #endregion

        private void uxRunFullAnalysis_Click(object sender, EventArgs e)
        {
            _runAnalysis();
        }
    }
}
