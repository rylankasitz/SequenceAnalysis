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
            // Write Data
            writeSiteChanges();

            // Graph Stuff
            _dataManager.WriteJsonFile(_dataManager.SequencesUsed, "Sequences");
            _dataManager.WriteJsonFile(_dataManager.PercentIdentities, "PercentIdentities");
            _dataManager.WriteJsonFile(_dataManager.RecombinationData, "Recombination");
            _dataManager.WriteJsonFile(_dataManager.AnalysisNames, "AnalysisNames");
            _dataManager.WriteJsonFile(_dataManager.TreeData, "Trees");
            _commandlineRun.ProgramName = "BuildGraphs.exe";
            _commandlineRun.Arguments = "\"" + Path.GetFullPath(_dataManager.DataFolder) + "\"";
            _commandlineRun.Run();
        }

        #region Output Methods

        private void writeSiteChanges()
        {
            string fileDir = _dataManager.CreateOutputDirectory("DiffenceLocations");
            foreach(KeyValuePair<string, PercentIdentityData> piPair in _dataManager.PercentIdentities)
            {
                StreamWriter writer = new StreamWriter(fileDir + piPair.Key + ".csv");
                foreach(KeyValuePair<string, Dictionary<int, Dictionary<string, string>>> siteDic in piPair.Value.SiteChanges)
                {
                    writer.Write(siteDic.Key);
                    List<int> sortedSites = siteDic.Value.Keys.ToList(); sortedSites.Sort();
                    foreach (int siteLocation in sortedSites)
                    {
                        writer.Write("," + siteLocation);
                    }
                    writer.Write("\n");
                    foreach (string sequence in piPair.Value.SiteChanges.Keys)
                    {
                        if (sequence != siteDic.Key)
                        {
                            writer.Write(sequence);
                            foreach (int siteLocation in sortedSites)
                            {
                                if (siteDic.Value[siteLocation].ContainsKey(sequence))
                                    writer.Write("," + siteDic.Value[siteLocation][sequence]);
                                else
                                    writer.Write(",");
                            }
                            writer.Write("\n");
                        }
                    }
                    writer.Write("\n");
                }
                writer.Close();
            }
        }

        #endregion
    }
}
