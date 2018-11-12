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
using PRRSAnalysis.Properties;

namespace PRRSAnalysis.DataStorage
{
    public class DataManager
    {

        #region FileLocations

        public string DataFolder { get; set; } = "_TempData\\";
        public string OrfTempateFolder { get; set; } = Path.GetFullPath("_OrfTemplates\\");
        public string OutputFolder { get; set; } = "";

        #endregion

        #region OrfData

        public Dictionary<string, string[]> AminoAcidChart { get; set; }
        public Dictionary<string, OrfsTemplate> OrfTemplates { get; set; }
        public float OrfLengthThreshold { get; } = .5f;
        public float OrfIdentifierPIThreshold { get; } = 0;
        public float OrfSiteRange { get; } = 4000;
        public Dictionary<string, NSPTemplate> NSPTemplate { get; set; }

        #endregion

        #region Analysis Data

        public int SequenceCount { get; set; } = 0;
        public int AnalysisCount { get; set; } = 0;
        public TimeSpan RunTime { get; set; }
        public Dictionary<string, SequenceData> SequencesUsed { get; set; } = new Dictionary<string, SequenceData>();
        public Dictionary<string, SequenceData> SequencesLoaded { get; set; } = new Dictionary<string, SequenceData>();
        public List<string> AnalysisNames { get; set; } = new List<string>();
        public Dictionary<string, string> AnalysisFiles { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, AlignmentData> Alignments { get; set; } = new Dictionary<string, AlignmentData>();
        public Dictionary<string, PercentIdentityData> PercentIdentities { get; set; } = new Dictionary<string, PercentIdentityData>();
        public Dictionary<string, TreeData> TreeData { get; set; } = new Dictionary<string, TreeData>();
        public Dictionary<string, List<RecombinationData>> RecombinationData { get; set; } = new Dictionary<string, List<RecombinationData>>();
        public Dictionary<string, int[]> NSPLocations { get; set; } = new Dictionary<string, int[]>();

        #endregion

        #region Settings Data

        /// <summary>
        /// Key of virsus specified in orf template to use
        /// </summary>
        public string CurrentVirusKey { get; set; } = "PRRS";
        /// <summary>
        /// Location of vaccine strands to compare to
        /// </summary>
        public string VaccineLocation { get; set; } = "";
        /// <summary>
        /// Output Location
        /// </summary>
        public string MainOutputFolder { get; set; } = "";
        /// <summary>
        /// Speed setting for Mafft alignment program
        /// </summary>
        public string MafftSettings { get; set; } = "Fast";
        /// <summary>
        /// Whether or not RDP4 is installed
        /// </summary>
        public bool RDP4Installed { get; set; } = false;
        /// <summary>
        /// Location of program RDP4
        /// </summary>
        public string RDPLocation { get; set; } = "";
        /// <summary>
        /// Whether or not file folders have been initialized
        /// </summary>
        public bool FilesInitialized { get; set; } = false;
        /// <summary>
        /// Whether or not to run the reverse reads to find new orfs
        /// </summary>
        public bool RunReverseFrames { get; set; } = false;
        /// <summary>
        /// Minimun length a found orf can be
        /// </summary>
        public int MinimumOrfLength { get; set; } = 75;
        /// <summary>
        /// Whether or not read in files are paritial orfs
        /// </summary>
        public bool PartialOrfFile { get; set; } = false;
        /// <summary>
        /// List of Combined orfs to run analysis on
        /// </summary>
        public List<List<string>> CombinedOrfs { get; set; } = new List<List<string>>()
        {
            new List<string>()
            {
                "Orf2b", "Orf3", "Orf4", "Orf5a"
            }
        };
        /// <summary>
        /// Maximun length that a sequence name will be when being displayed in the program
        /// </summary>
        public int MaximunNameLength { get; set; } = 40;
 
        #endregion

        #region Data Conversion Methods

        public Dictionary<string, string> FileToSequences(string fileLocation)
        {
            Dictionary<string, string> sequences = new Dictionary<string, string>();
            string type = Path.GetExtension(fileLocation);
            StreamReader reader;
            try
            {
                reader = new StreamReader(fileLocation);
            }
            catch
            {
                throw new Exception("Cannot find file: " + fileLocation);
            }
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
                                name = CutName(line.Substring(1, line.Length - 1));
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
                catch
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
            string[] fileparts = fileLocation.Split('.');
            string outfile = fileparts[fileparts.Length - 2] + ".phy";
            StreamWriter writer = new StreamWriter(outfile);
            writer.WriteLine(fastaContents.Count + " " + fastaContents.Values.First().Length);
            foreach (KeyValuePair<string, string> fastaContent in fastaContents)
            {
                writer.WriteLine(fastaContent.Key + "\t\t\t" + fastaContent.Value);
            }
            writer.Close();
            return outfile;
        }
        public string CutName(string name)
        {
            if (Char.IsDigit(name[0])) name = "s-" + name;
            if (name.Length > MaximunNameLength) name = name.Split(' ')[0];
            if (name.Length > MaximunNameLength) name = name.Substring(0, MaximunNameLength);
            name = name.Replace("(", "[");
            name = name.Replace(")", "]");
            return name;
        }

        #endregion

        #region Reading / Writing Methods

        public void AddSequencesFromFile(string fileLocation, bool vaccine = false)
        {
            Dictionary<string, string> sequences = FileToSequences(fileLocation);
            foreach(KeyValuePair<string, string> sequence in sequences)
            {
                if (!SequencesLoaded.ContainsKey(sequence.Key))
                {
                    SequencesUsed.Add(sequence.Key, new SequenceData());
                    SequencesUsed[sequence.Key].Name = sequence.Key;
                    SequencesUsed[sequence.Key].Contents = sequence.Value;
                    SequencesUsed[sequence.Key].Vaccine = vaccine;
                    SequencesLoaded.Add(sequence.Key, new SequenceData());
                    SequencesLoaded[sequence.Key].Name = sequence.Key;
                    SequencesLoaded[sequence.Key].Contents = sequence.Value;
                    SequencesLoaded[sequence.Key].Vaccine = vaccine;
                }
            }
        }
        public void CreateOneSequenceFile(string name, bool orfFile = false)
        {
            try
            {
                if (!orfFile)
                {
                    StreamWriter writer = new StreamWriter(DataFolder + name + ".fasta");
                    foreach (SequenceData sequenceData in SequencesUsed.Values)
                    {
                        writer.WriteLine(">" + sequenceData.Name);
                        writer.WriteLine(sequenceData.Contents);
                    }
                    writer.Close();
                    AnalysisFiles.Add(name, DataFolder + name);
                }
                else
                {
                    StreamWriter writer = new StreamWriter(DataFolder + name + ".fasta");
                    foreach (SequenceData sequenceData in SequencesUsed.Values)
                    {
                        OrfData orfData;
                        if (sequenceData.KnownOrfData.TryGetValue(name.Split('_')[0], out orfData))
                        {
                            writer.WriteLine(">" + sequenceData.Name);
                            if (name.Split('_')[1] == "aa") writer.WriteLine(orfData.ContentsAA);
                            else writer.WriteLine(orfData.ContentsN);
                        }
                    }
                    writer.Close();
                    AnalysisFiles.Add(name, DataFolder + name);
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
        public void WriteJsonFile(object o, string fileName)
        {
            string json = JsonConvert.SerializeObject(o);
            File.WriteAllText(DataFolder + fileName + ".json", json);
        }
        public void DeserializeJsonFiles()
        {
            AminoAcidChart = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(OrfTempateFolder + "AminoAcidTemplate.json"));
            OrfTemplates = JsonConvert.DeserializeObject<Dictionary<string, OrfsTemplate>>(File.ReadAllText(OrfTempateFolder + "PRRS_Orf_Template.json"));
            NSPTemplate = JsonConvert.DeserializeObject<Dictionary<string, NSPTemplate>>(File.ReadAllText(OrfTempateFolder + "PRRS_NSP_Template.json"));
        }
        public string CreateOutputDirectory(string name)
        {
            if (!Directory.Exists(OutputFolder + name)) {
                try
                {
                    Directory.CreateDirectory(OutputFolder + name);
                }
                catch
                {
                    throw new Exception("Output folder" + name + " does not exist");
                }
            }
            return OutputFolder + name + "\\";
        }
        public string CreateDirectory(string name)
        {
            if (!Directory.Exists(OutputFolder + name))
            {
                try
                {
                    Directory.CreateDirectory(name);
                }
                catch
                {
                    throw new Exception("Cannot create directory " + name);
                }
            }
            return OutputFolder + name + "\\";
        }
        public void MoveFile(string src, string dest)
        {
            try
            {
                string fileName = Path.GetFileName(src);
                string destFile = Path.Combine(dest, fileName);
                File.Copy(src, destFile, true);
            }
            catch
            {
                throw new Exception("Unable to move file " + src + " to " + dest);
            }
        }

        public void RemoveAllFiles(string src)
        {
            DirectoryInfo di = new DirectoryInfo(src);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        #endregion

        #region Data Saving

        public void SaveData()
        {
            Settings.Default.VaccineLocation = VaccineLocation;
            Settings.Default.MafftSettings = MafftSettings;
            Settings.Default.MinimumOrfLength = MinimumOrfLength;
            Settings.Default.PartialOrfFile = PartialOrfFile;
            Settings.Default.OutputFolder = MainOutputFolder;
            Settings.Default.RDP4 = RDPLocation;
            Settings.Default.InitialRun = RDP4Installed;
            Settings.Default.FilesInitialized = FilesInitialized;
            Settings.Default.Save();
        }
        public void LoadData()
        {            
            MafftSettings = Settings.Default.MafftSettings;
            MinimumOrfLength = Settings.Default.MinimumOrfLength;
            PartialOrfFile = Settings.Default.PartialOrfFile;
            MainOutputFolder = Settings.Default.OutputFolder;
            VaccineLocation = Settings.Default.VaccineLocation;
            RDPLocation = Settings.Default.RDP4;
            RDP4Installed = Settings.Default.InitialRun;
            FilesInitialized = Settings.Default.FilesInitialized;

            if (VaccineLocation == "")
                VaccineLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SequenceAnalysisProgram\\VaccineFiles\\" + CurrentVirusKey + ".fasta";
            if (MainOutputFolder == "")
                MainOutputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SequenceAnalysisProgram\\Output\\";
            if (RDPLocation == "")
                RDPLocation = "C:\\Program Files (x86)\\RDP4";
        }

        #endregion

        public void ResetVariablesForRun()
        {
            AnalysisFiles = new Dictionary<string, string>();
            AnalysisNames = new List<string>(); AnalysisNames.Add("Wholegenome");
            Alignments = new Dictionary<string, AlignmentData>();
            PercentIdentities = new Dictionary<string, PercentIdentityData>();
            TreeData = new Dictionary<string, TreeData>();
            RecombinationData = new Dictionary<string, List<RecombinationData>>();
        }
    }
}