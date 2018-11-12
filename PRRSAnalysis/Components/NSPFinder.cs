using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.ComponentLayouts;
using PRRSAnalysis.DataStorage;

namespace PRRSAnalysis.Components
{
    public class NSPFinder : SingleLoop
    {
        private DataManager _dataManager;

        public NSPFinder(DataManager dataManager)
        {
            _dataManager = dataManager;
            Priority = 3;
        }

        public override void Run(UpdateProgressBar updateProgressBar)
        {
            string currentOrf = "";
            int prevEnd = 0;
            int tempEnd = 0;
            int preLength = 0;        
            foreach (KeyValuePair<string, NSPTemplate> nspTemplatePair in _dataManager.NSPTemplate)
            {
                currentOrf = nspTemplatePair.Value.Orf;
                if (_dataManager.Alignments.ContainsKey(nspTemplatePair.Value.Orf + "_aa"))
                {
                    string content = _dataManager.Alignments[nspTemplatePair.Value.Orf + "_aa"].Contents.First().Value;
                    string currentEndSite = nspTemplatePair.Value.EndSite;
                    int thresholdScore = 1000;
                    bool found = true;
                    for (int i = prevEnd; i < content.Length - 1; i++)
                    {
                        if (currentEndSite == "X")
                        {
                            _dataManager.NSPLocations[nspTemplatePair.Key] = new int[2] { (i + 1) * 3 + preLength, (i + nspTemplatePair.Value.Length) * 3 + preLength };
                            i += nspTemplatePair.Value.Length;
                            prevEnd = i;
                            found = false;
                            break;
                        }
                        else if (currentEndSite == "End")
                        {
                            _dataManager.NSPLocations[nspTemplatePair.Key] = new int[2] { (i + 1) * 3 + preLength, (content.Length) * 3 + preLength };
                            prevEnd = 0;
                            preLength += content.Length*3;
                            found = false;
                            break;
                        }
                        else if (currentEndSite == (content[i].ToString() + content[i + 1].ToString()))
                        {
                            if (thresholdScore > Math.Abs(i - (nspTemplatePair.Value.Length + prevEnd)))
                            {                           
                                thresholdScore = Math.Abs(i - (nspTemplatePair.Value.Length + prevEnd));
                                _dataManager.NSPLocations[nspTemplatePair.Key] = new int[2] { (prevEnd) * 3 + preLength, (i + 1) * 3 + preLength };
                                tempEnd = i + 1;
                            }
                            else
                            {
                                prevEnd = tempEnd;
                                thresholdScore = 1000;
                                found = false;
                                break;
                            }
                        }
                    }
                    if (found) prevEnd = tempEnd;
                }
            }
        }
    }
}
