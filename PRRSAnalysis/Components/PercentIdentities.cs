using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.DataStorage;
using PRRSAnalysis.ComponentLayouts;
using PRRSAnalysis.AnalysisHelpers;
using System.Windows.Forms;

namespace PRRSAnalysis.Components
{
    public class PercentIdentities : AnalysisLoop
    {
        private DataManager _dataManager;

        public PercentIdentities(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public override void Run(string analysisName, UpdateProgressBar updateProgressBar)
        {
            _dataManager.PercentIdentities[analysisName] = new PercentIdentityData();
            foreach (KeyValuePair<string, string> sequence1 in _dataManager.Alignments[analysisName].Contents)
            {
                _dataManager.PercentIdentities[analysisName].Dic[sequence1.Key] = new Dictionary<string, float>();
                _dataManager.PercentIdentities[analysisName].DicInverse[sequence1.Key] = new Dictionary<string, float>();
                _dataManager.PercentIdentities[analysisName].Sequences.Add(sequence1.Key);
                List<float> dataList = new List<float>();
                float total = 0;
                int count = 0;
                Dictionary<int, Dictionary<string, string>> siteChanges = new Dictionary<int, Dictionary<string, string>>();
                foreach (KeyValuePair<string, string> sequence2 in _dataManager.Alignments[analysisName].Contents)
                {              
                    float percent = GlobalCalculations.CalculatePercentIdentity(sequence1.Value, sequence2.Value, sequence2.Key, siteChanges);
                    if (sequence1.Key != sequence2.Key) total += percent; count++;
                    if (!_dataManager.PercentIdentities[analysisName].Dic[sequence1.Key].ContainsKey(sequence2.Key))
                    {
                        _dataManager.PercentIdentities[analysisName].Dic[sequence1.Key][sequence2.Key] = percent;
                        _dataManager.PercentIdentities[analysisName].DicInverse[sequence1.Key][sequence2.Key] = 100 - percent;
                    }
                    dataList.Add(percent);
                    
                }
                _dataManager.PercentIdentities[analysisName].SiteChanges[sequence1.Key] = siteChanges;
                _dataManager.PercentIdentities[analysisName].Data.Add(dataList);
            }

            updateProgressBar((int) (20 / (float) _dataManager.AnalysisCount));
        }
    }
}
