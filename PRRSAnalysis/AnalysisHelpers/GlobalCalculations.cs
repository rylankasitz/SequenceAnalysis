using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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
    }  
}
