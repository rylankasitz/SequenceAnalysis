using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.ComponentLayouts;
using PRRSAnalysis.DataStorage;
using PRRSAnalysis.AnalysisHelpers;
using System.IO;
using System.Windows.Forms;

namespace PRRSAnalysis.Components
{
    public class Recombination : SingleLoop
    {
        private DataManager _dataManager;
        private CommandlineRun _commandlineRun;

        public Recombination(DataManager dataManager)
        {
            _dataManager = dataManager;
            _commandlineRun = new CommandlineRun();
        }
        public override void Run(UpdateProgressBar updateProgressBar)
        {
            _commandlineRun.AplicationPath = _dataManager.RDPLocation + "\\";
            _commandlineRun.ProgramName = "rdp4.exe";
            _commandlineRun.Arguments = "-f" + new FileInfo(_dataManager.Alignments["Wholegenome"].FileLocation).FullName;
            _commandlineRun.Run();
            AddData();
            sortData();

            updateProgressBar(30);
        }
        private void AddData()
        {
            StreamReader reader = new StreamReader(_dataManager.DataFolder + "Wholegenome_aligned.fasta.csv");
            string line;
            int counter = 0;
            while((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                if (counter > 2 && parts.Length >= 20)
                {    
                    string sequenceName = _dataManager.CutName(removeExtra(parts[8]));
                    if (!_dataManager.RecombinationData.ContainsKey(sequenceName)) _dataManager.RecombinationData.Add(sequenceName, new List<RecombinationData>());
                    RecombinationData recombinationData = new RecombinationData
                    {
                        StartSite = Convert.ToInt32(removeExtra(parts[4])),
                        EndSite = Convert.ToInt32(removeExtra(parts[5])),
                        SequenceLength = _dataManager.SequencesUsed[sequenceName].Contents.Length
                    };
                    _dataManager.RecombinationData[sequenceName].Add(recombinationData);
                }
                counter++;
            }
        }
        private void sortData()
        {
            foreach(string key in _dataManager.RecombinationData.Keys)
            {
                List<RecombinationData> list = new List<RecombinationData>();
                _dataManager.RecombinationData[key].ForEach((item) => { list.Add(item);});
                list.Sort((pair1, pair2) => pair1.StartSite.CompareTo(pair2.StartSite));
                int i = 0;
                foreach(RecombinationData item in list)
                {
                    _dataManager.RecombinationData[key][i] = item;
                    i++;
                }
            }
        }
        private string removeExtra(string data)
        {
            return data.Trim(new Char[] { '^', '*', '~' });
        }
    }
}
