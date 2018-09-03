using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PRRSAnalysis.ComponentLayouts;
using PRRSAnalysis.DataStorage;
using System.IO;
using PRRSAnalysis.AnalysisHelpers;
using System.Diagnostics;

namespace PRRSAnalysis.Output
{
    public class OutputManager : SingleLoop
    {
        private DataManager _dataManager;
        private CommandlineRun _commandlineRun;
       
        public OutputManager(DataManager dataManager)
        {
            Priority = 4;

            _dataManager = dataManager;
            _commandlineRun = new CommandlineRun();
        }

        public override void Run()
        {
            _dataManager.WriteJsonFile(_dataManager.PercentIdentities, "PercentIdentities");
            _dataManager.WriteJsonFile(_dataManager.RecombinationData, "Recombination");
            _dataManager.WriteJsonFile(_dataManager.AnalysisNames, "AnalysisNames");
            _dataManager.WriteJsonFile(_dataManager.TreeData, "Trees");
            _commandlineRun.ProgramName = "BuildGraphs.exe";
            _commandlineRun.Arguments = "\"" + Path.GetFullPath(_dataManager.DataFolder) + "\"";
            _commandlineRun.Run();
        }
    }
}
