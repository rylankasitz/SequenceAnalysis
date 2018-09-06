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
        private int orfsPerLine;
        private DataManager _dataManager;
        private CommandlineRun _commandlineRun;
       
        public OutputManager(DataManager dataManager)
        {
            Priority = 4;

            orfsPerLine = 10;
            _dataManager = dataManager;
            _commandlineRun = new CommandlineRun();
        }

        public override void Run()
        {
            // Write Data
            moveAlignmentFiles();
            writeSiteChanges();
            writeOrfsFound();

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

        private void moveAlignmentFiles()
        {
            string fileDir = _dataManager.CreateOutputDirectory("Alignments");
            foreach (KeyValuePair<string, AlignmentData> alignments in _dataManager.Alignments)
            {
                _dataManager.MoveFile(alignments.Value.FileLocation, fileDir);
            }
        }
        private void writeOrfsFound()
        {
            string fileDir = _dataManager.CreateOutputDirectory("Orfs");
            foreach(KeyValuePair<string, SequenceData> sequenceOrfPair in _dataManager.SequencesUsed)
            {
                StreamWriter writer = new StreamWriter(fileDir + sequenceOrfPair.Key + ".csv");
                writer.Write(sequenceOrfPair.Key + "\n");
                bool writeValues = false;
                for(int i = 0; i < sequenceOrfPair.Value.OtherOrfData.Count; i++)
                {
                    if(!writeValues) writer.Write("Orf" + i + ",");
                    else writer.Write(sequenceOrfPair.Value.OtherOrfData["orf" + (i + 1)].StartLocationN + "->" +
                                      sequenceOrfPair.Value.OtherOrfData["orf" + (i + 1)].EndLocationN + ",");
                    if (((i+1) % orfsPerLine) == 0)
                    {
                        if (!writeValues) i -= 10;
                        writeValues = !writeValues;
                        writer.Write("\n\n");
                    }
                }
                writer.Close();
            }
        }
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
