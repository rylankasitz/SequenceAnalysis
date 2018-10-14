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
using System.Windows.Forms;

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

        public override void Run(UpdateProgressBar updateProgressBar)
        {
            // Write Data    
            moveAlignmentFiles();
            writeSiteChanges();
            string fileDir = _dataManager.CreateOutputDirectory("UnknownOrfs");
            foreach (KeyValuePair<string, SequenceData> sequenceOrfPair in _dataManager.SequencesUsed)
                writeOrfsFound(fileDir + sequenceOrfPair.Key + ".csv", sequenceOrfPair.Value.OtherOrfData);
            fileDir = _dataManager.CreateOutputDirectory("KnownOrfs");
            foreach (KeyValuePair<string, SequenceData> sequenceOrfPair in _dataManager.SequencesUsed)
                writeOrfsFound(fileDir + sequenceOrfPair.Key + ".csv", sequenceOrfPair.Value.KnownOrfData);
            fileDir = _dataManager.CreateOutputDirectory("NSPData");
            writeNSPS(fileDir + "nsplocations.csv");


            // Graph Stuff
            _dataManager.WriteJsonFile(_dataManager.SequencesUsed, "Sequences");
            _dataManager.WriteJsonFile(_dataManager.PercentIdentities, "PercentIdentities");
            _dataManager.WriteJsonFile(_dataManager.RecombinationData, "Recombination");
            _dataManager.WriteJsonFile(_dataManager.AnalysisNames, "AnalysisNames");
            _dataManager.WriteJsonFile(_dataManager.TreeData, "Trees");
            _commandlineRun.ProgramName = "BuildGraphs.exe";
            _commandlineRun.Arguments = ("--i \"" + Path.GetDirectoryName(_dataManager.DataFolder)  + "\" --out \"" +
                                        Path.GetDirectoryName(_dataManager.OutputFolder) + "\"");
            Console.WriteLine(_commandlineRun.Arguments);
            _commandlineRun.Run();

            updateProgressBar(1000);
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
        private void writeOrfsFound(string filedir, Dictionary<string, OrfData> sequenceOrfPair)
        {

            StreamWriter writer = new StreamWriter(filedir);
            foreach(KeyValuePair<string, OrfData> orfDataPair in sequenceOrfPair)
            {
                writer.Write(orfDataPair.Value.Name + "," + orfDataPair.Value.StartLocationN + "," + orfDataPair.Value.EndLocationN + "\n");
            }
            writer.Close();
        }
        private void writeNSPS(string filedir)
        {
            StreamWriter writer = new StreamWriter(filedir);
            writer.Write("Name,Start (nt), End (nt)\n");
            foreach (KeyValuePair<string, int[]> nspPair in _dataManager.NSPLocations)
            {
                writer.Write(nspPair.Key + "," + nspPair.Value[0] + "," + nspPair.Value[1] + ",\n");
            }
            writer.Close();
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
