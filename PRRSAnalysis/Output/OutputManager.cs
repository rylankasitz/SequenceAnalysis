using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PRRSAnalysis.ComponentLayouts;
using PRRSAnalysis.DataStorage;
using System.IO;

namespace PRRSAnalysis.Output
{
    public class OutputManager : AnalysisLoop
    {
        private DataManager _dataManager;

        public OutputManager(DataManager dataManager)
        {
            Priority = 4;
            _dataManager = dataManager;
        }

        public override void Run(string analysisItem)
        {
            if (!Directory.Exists(_dataManager.DataFolder + "JsonFiles\\"))
                Directory.CreateDirectory(_dataManager.DataFolder + "JsonFiles\\");

            string jsonPercentIdentity = JsonConvert.SerializeObject(_dataManager.PercentIdentities[analysisItem]);
            File.WriteAllText(_dataManager.DataFolder + "JsonFiles\\" + analysisItem + "_PI.json", jsonPercentIdentity);

            string jsonRecombination = JsonConvert.SerializeObject(_dataManager.PercentIdentities[analysisItem]);
            File.WriteAllText(_dataManager.DataFolder + "JsonFiles\\" + analysisItem + "_Recomb.json", jsonRecombination);
        }
    }
}
