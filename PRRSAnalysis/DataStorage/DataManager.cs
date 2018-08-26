using PRRSAnalysis.DataStorage;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace PRRSAnalysis.DataStorage
{
    public class DataManager
    {  

        #region FileLocations

        public string DataFolder { get; set; } = "_TempData\\";
        public string OrfTempateFolder { get; set; } = Path.GetFullPath("_OrfTemplates\\");

        #endregion

        #region OrfData

        public Dictionary<string, string[]> AminoAcidChart { get; set; }
        public Dictionary<string, OrfsTemplate> OrfTemplates { get; set; }

        #endregion

        #region Analysis Data


        public Dictionary<string, SequenceData> SequencesUsed { get; set; } = new Dictionary<string, SequenceData>();
        public Dictionary<string, SequenceData> SequencesLoaded { get; set; } = new Dictionary<string, SequenceData>();
        public List<string> AnalysisNames { get; set; } = new List<string>();
        public Dictionary<string, string> AnalysisFiles { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, AlignmentData> Alignments { get; set; } = new Dictionary<string, AlignmentData>();
        public Dictionary<string, Dictionary<string, PercentIdentityData>> PercentIdentities { get; set; } = new Dictionary<string, Dictionary<string, PercentIdentityData>>();
        public Dictionary<string, TreeData> TreeData { get; set; } = new Dictionary<string, TreeData>();

        #endregion

        #region Settings Data

        public string MafftSettings { get; set; } = "Fast";
        public bool RunReverseFrames { get; set; } = false;
        public int MinimumOrfLength { get; set; } = 75;
 
        #endregion

        #region Data Conversion Methods

        public Dictionary<string, string> FileToSequences(string fileLocation)
        {
            Dictionary<string, string> sequences = new Dictionary<string, string>();
            string type = Path.GetExtension(fileLocation);
            StreamReader reader = new StreamReader(fileLocation);
            if (type == ".fasta")
            {
                try
                {
                    string line;
                    string name = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line != "")
                        {
                            if (line[0] == '>')
                            {
                                name = line.Substring(1, line.Length - 1);
                                if (!sequences.ContainsKey(name))
                                {
                                    sequences.Add(name, "");
                                }
                            }
                            else
                            {
                                sequences[name] += line.Replace("\n", "");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Was not able to read file, it might not be in proper fasta format");
                }
            }
            reader.Close();
            return sequences;
        }
        public string FastaToPhyiFile(string fileLocation)
        {
            Dictionary<string, string> fastaContents = FileToSequences(fileLocation);
            FileInfo fileInfo = new FileInfo(fileLocation);
            string outfile = fileInfo.DirectoryName + "\\" + fileInfo.Name.Split('.')[0] + ".phy";
            StreamWriter writer = new StreamWriter(outfile);
            writer.WriteLine(fastaContents.Count + " " + fastaContents.Keys.First().Length);
            foreach (KeyValuePair<string, string> fastaContent in fastaContents)
            {
                writer.WriteLine(fastaContent.Key + "\t\t\t" + fastaContent.Value);
            }
            writer.Close();
            return outfile;
        }

        #endregion

        #region Reading / Writing Methods

        public void AddSequencesFromFile(string fileLocation)
        {
            Dictionary<string, string> sequences = FileToSequences(fileLocation);
            foreach(KeyValuePair<string, string> sequence in sequences)
            {
                if (!SequencesLoaded.ContainsKey(sequence.Key))
                {
                    SequencesUsed.Add(sequence.Key, new SequenceData());
                    SequencesUsed[sequence.Key].Name = sequence.Key;
                    SequencesUsed[sequence.Key].Contents = sequence.Value;
                    SequencesLoaded.Add(sequence.Key, new SequenceData());
                    SequencesLoaded[sequence.Key].Name = sequence.Key;
                    SequencesLoaded[sequence.Key].Contents = sequence.Value;
                }
            }
        }
        public void AddLoadedSeqeunce(string name)
        {
            SequenceData sequenceData;
            if(SequencesLoaded.TryGetValue(name, out sequenceData)) {
                SequencesUsed.Add(name, sequenceData);
            }
            else
            {
                throw new Exception("Cannot find sequence " + name + " in loaded sequence files");
            }
        }
        public void RemoveLoadedSequence(string name)
        {
            if (SequencesUsed.ContainsKey(name))
            {
                SequencesUsed.Remove(name);
            }
            else
            {
                throw new Exception("Connot find sequence " + name + " to remove");
            }
        }
        public void CreateOneSequenceFile(string name, bool orfFile = false)
        {
            try
            {
                if (!orfFile)
                {
                    StreamWriter writer = new StreamWriter(DataFolder + name + "_n.fasta");
                    foreach (SequenceData sequenceData in SequencesUsed.Values)
                    {
                        writer.WriteLine(">" + sequenceData.Name);
                        writer.WriteLine(sequenceData.Contents);
                    }
                    writer.Close();
                    AnalysisFiles.Add(name, DataFolder + name + "_n");
                }
                else
                {
                    StreamWriter writerAA = new StreamWriter(DataFolder + name + "_aa");
                    StreamWriter writerN = new StreamWriter(DataFolder + name + "_n");
                    foreach (SequenceData sequenceData in SequencesUsed.Values)
                    {
                        OrfData orfData;
                        if (sequenceData.KnownOrfData.TryGetValue(name, out orfData))
                        {
                            writerAA.WriteLine(">" + orfData.Name);
                            writerAA.WriteLine(orfData.ContentsAA);
                            writerN.WriteLine(">" + orfData.Name);
                            writerN.WriteLine(orfData.ContentsN);
                        }
                    }
                    writerAA.Close();
                    writerN.Close();
                    AnalysisFiles.Add(name, name + "_aa");
                    AnalysisFiles.Add(name, name + "_n");
                }
            }
            catch
            {
                throw new Exception("Was unable to write sequence file for " + name);
            }
        }
        public void ClearAllSequenceData()
        {
            SequencesUsed = new Dictionary<string, SequenceData>();
            SequencesLoaded = new Dictionary<string, SequenceData>();
        }

        #endregion

        public void DeserializeJsonFiles()
        {
            AminoAcidChart = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(OrfTempateFolder + "AminoAcidTemplate.json"));

            OrfTemplates = JsonConvert.DeserializeObject<Dictionary<string, OrfsTemplate>>(File.ReadAllText(OrfTempateFolder + "PRRS_Orf_Template.json"));
        }
    }
}