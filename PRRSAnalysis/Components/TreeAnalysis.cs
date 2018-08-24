using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.ComponentLayouts;
using PRRSAnalysis.DataStorage;
using PRRSAnalysis.AnalysisHelpers;

namespace PRRSAnalysis.Components
{
    public class TreeAnalysis : AnalysisLoop
    {
        private DataManager _dataManager;
        private CommandlineRun _commandlineRun;

        public TreeAnalysis(DataManager dataManager)
        {
            _dataManager = dataManager;
            _commandlineRun = new CommandlineRun();
        }

        public override void Run(string analysisItem)
        {
            _commandlineRun.ProgramName = "PhyML-3.1_win32";
            string infile = _dataManager.FastaToPhyiFile(_dataManager.Alignments[analysisItem].FileLocation);
            _commandlineRun.Arguments = "-i " + infile;
            _commandlineRun.Run();

            if (!_dataManager.TreeData.ContainsKey(analysisItem))
            {
                _dataManager.TreeData.Add(analysisItem, new TreeData());
            }
            _dataManager.TreeData[analysisItem].NewickFile = _dataManager.Alignments[analysisItem].FileLocation + "_phyml_tree.txt";
        }
    }
}
