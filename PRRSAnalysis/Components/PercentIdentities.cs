using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.DataStorage;
using PRRSAnalysis.ComponentLayouts;
using PRRSAnalysis.AnalysisHelpers;

namespace PRRSAnalysis.Components
{
    public class PercentIdentities : AnalysisLoop
    {
        private DataManager _dataManager;

        public PercentIdentities(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public override void Run(string analysisName)
        {
            _dataManager.PercentIdentities[analysisName] = new Dictionary<string, PercentIdentityData>();
            foreach (KeyValuePair<string, string> sequence1 in _dataManager.Alignments[analysisName].Contents)
            {
                _dataManager.PercentIdentities[analysisName][sequence1.Key] = new PercentIdentityData();
                foreach (KeyValuePair<string, string> sequence2 in _dataManager.Alignments[analysisName].Contents)
                {
                    float percent = GlobalCalculations.CalculatePercentIdentity(sequence1.Value, sequence2.Value);
                    if (!_dataManager.PercentIdentities[analysisName][sequence1.Key].PercentIdentityComparisons.ContainsKey(sequence2.Key))
                    {
                        _dataManager.PercentIdentities[analysisName][sequence1.Key].PercentIdentityComparisons[sequence2.Key] = percent;
                    }                  
                }
            }
        }

        
    }
}
