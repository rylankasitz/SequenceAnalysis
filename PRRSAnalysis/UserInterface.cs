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

            uxSequenceList.ItemCheck += uxSequenceListItem_Click;
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
                updateSequenceList();
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
                updateSequenceList();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        #endregion

        private void uxRunFullAnalysis_Click(object sender, EventArgs e)
        {
            _dataManager.AddSequencesFromFile(_dataManager.VaccineLocation, vaccine: true);
            _runAnalysis();
        }

        private void uxSequenceListItem_Click(object sender, EventArgs e)
        {
            CheckedListBox checkBox = (CheckedListBox) sender;
            if (!checkBox.CheckedItems.Contains(checkBox.SelectedItem))
            {
                if (!_dataManager.SequencesLoaded.ContainsKey(checkBox.SelectedItem.ToString()))
                    _dataManager.SequencesLoaded.Add(checkBox.SelectedItem.ToString(), new SequenceData());
                if (!_dataManager.SequencesUsed.ContainsKey(checkBox.SelectedItem.ToString()))
                    _dataManager.SequencesUsed.Add(checkBox.SelectedItem.ToString(), _dataManager.SequencesLoaded[checkBox.SelectedItem.ToString()]);
            }
            else
            {
                if (_dataManager.SequencesUsed.ContainsKey(checkBox.SelectedItem.ToString()))
                    _dataManager.SequencesUsed.Remove(checkBox.SelectedItem.ToString());
            }
        }

        private void updateSequenceList()
        {
            foreach(SequenceData sequenceData in _dataManager.SequencesLoaded.Values)
            {
                if(!sequenceData.Vaccine)
                    uxSequenceList.Items.Add(sequenceData.Name); 
            }
        }
    }
}
