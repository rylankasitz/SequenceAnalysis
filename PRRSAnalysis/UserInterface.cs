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

public delegate void UpdateProgressBar(int val);

namespace PRRSAnalysis
{
    public partial class UserInterface : Form
    {
        private DataManager _dataManager;
        private RunDelegate _runAnalysis;
        private UpdateProgressBar _updateProgressBar;

        public UserInterface(DataManager dataManager, RunDelegate run)
        {
            InitializeComponent();

            _dataManager = dataManager;
            _runAnalysis = run;
            _updateProgressBar = new UpdateProgressBar(updateProgressBar);

            uxSequenceList.ItemCheck += uxSequenceListItem_Click;
            uxOutputLocationTextBox.Text = _dataManager.MainOutputFolder;
            uxVaccineLocationTextBox.Text = _dataManager.VaccineLocation;
            uxMinOrfLengthTextbox.Text = _dataManager.MinimumOrfLength.ToString();
            uxAlignmentType.SelectedItem = _dataManager.MafftSettings;
            uxRunReverseReadsCB.Checked = _dataManager.RunReverseFrames;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dataManager.SaveData();
        }

        private void uxRunFullAnalysis_Click(object sender, EventArgs e)
        {
            string runname = Prompt.ShowDialog("Enter Run Name", "Run Name");
            _dataManager.OutputFolder = _dataManager.MainOutputFolder + runname + "\\";
            _dataManager.CreateDirectory(_dataManager.OutputFolder);
            
            uxProgressBar.Value = 0;
            _dataManager.ResetVariablesForRun();
            _dataManager.AddSequencesFromFile(_dataManager.VaccineLocation, vaccine: true);
            _runAnalysis(_updateProgressBar);
        }

        private void updateProgressBar(int val)
        {
            if (uxProgressBar.InvokeRequired)
            {
                UpdateProgressBar d = new UpdateProgressBar(updateProgressBar);
                Invoke(d, new object[] { val });
            }
            else
            {
                if (uxProgressBar.Value + val > uxProgressBar.Maximum) uxProgressBar.Value = uxProgressBar.Maximum;
                else uxProgressBar.Value += val;
            }
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
       
        #region Sequence List Events

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
            uxSequenceList.Items.Clear();
            foreach(SequenceData sequenceData in _dataManager.SequencesLoaded.Values)
            {
                if (!sequenceData.Vaccine)
                {
                    uxSequenceList.Items.Add(sequenceData.Name);
                }
            }
            for(int i = 0; i < uxSequenceList.Items.Count; i++)
            {
                uxSequenceList.SelectedItem = uxSequenceList.Items[i];
                uxSequenceList.SetItemChecked(i, true);
            }
        }

        #endregion

        #region Settings Change Events

        private void uxAlignmentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _dataManager.MafftSettings = uxAlignmentType.Text;
        }

        private void uxMinOrfLengthTextbox_TextChanged(object sender, EventArgs e)
        {
            _dataManager.MinimumOrfLength = Convert.ToInt32(uxMinOrfLengthTextbox.Text);
        }

        private void uxRunReverseReadsCB_CheckedChanged(object sender, EventArgs e)
        {
            _dataManager.RunReverseFrames = uxRunReverseReadsCB.Checked;
        }

        private void uxVaccineLocationButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (uxOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    uxVaccineLocationTextBox.Text = uxOpenFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void uxVaccineLocationTextBox_TextChanged(object sender, EventArgs e)
        {
            _dataManager.VaccineLocation = uxVaccineLocationTextBox.Text;
        }

        private void uxOutputLocationButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (uxOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    uxOutputLocationTextBox.Text = uxOpenFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void uxOutputLocationTextBox_TextChanged(object sender, EventArgs e)
        {
            _dataManager.OutputFolder = uxOutputLocationTextBox.Text;
        }

        #endregion
    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
