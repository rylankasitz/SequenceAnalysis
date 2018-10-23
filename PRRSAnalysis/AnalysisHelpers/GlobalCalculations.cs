using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using PRRSAnalysis.DataStorage;
using System.IO;

namespace PRRSAnalysis.AnalysisHelpers
{
    public abstract class GlobalCalculations
    {
        public static float CalculatePercentIdentity(string sequence1, string sequence2, string sequence2name, Dictionary<int, Dictionary<string, string>> diffSites)
        {
            int totalIdentity = 0;
            if (sequence1.Length < sequence2.Length) sequence2 = sequence2.Substring(0, sequence1.Length);
            else sequence1 = sequence1.Substring(0, sequence2.Length);
            for (int i = 0; i < sequence1.Length; i++)
            {
                if (sequence1[i] == sequence2[i])
                {
                    totalIdentity++;
                }
                else
                {
                    if (!diffSites.ContainsKey(i + 1)) diffSites[i + 1] = new Dictionary<string, string>();
                    diffSites[i + 1][sequence2name] = sequence1[i] + "->" + sequence2[i];
                }
            }
            return (float) Math.Round(totalIdentity / (float)sequence1.Length * 100, 3);
        }
        public static float CalculatePercentIdentity(string sequence1, string sequence2)
        {
            int totalIdentity = 0;
            if (sequence1.Length < sequence2.Length) sequence2 = sequence2.Substring(0, sequence1.Length);
            else sequence1 = sequence1.Substring(0, sequence2.Length);
            for (int i = 0; i < sequence1.Length; i++)
            {
                if (sequence1[i] == sequence2[i])
                {
                    totalIdentity++;
                }
            }
            return (float)Math.Round(totalIdentity / (float)sequence1.Length * 100, 3);
        }
        public static float CalculateAlignedPercentIdentity(string sequence1, string sequence2, DataManager dataManager)
        {
            CommandlineRun commandlineRun = new CommandlineRun();
            string infile = dataManager.DataFolder + "temp_pi.fasta";
            string outfile = dataManager.DataFolder + "temp_pi_result.fasta";
            StreamWriter writer = new StreamWriter(infile);
            writer.Write(">S1\n" + sequence1 + "\n>S2\n" + sequence2);
            writer.Close();
            commandlineRun.ProgramName = "mafft-win\\mafft";
            commandlineRun.Arguments = "--retree 1 --maxiterate 0 --out " + outfile + " " + infile;
            commandlineRun.Run();
            string[] result = dataManager.FileToSequences(outfile).Values.ToArray();
            return CalculatePercentIdentity(result[0], result[1]);
        }
    }  
}
