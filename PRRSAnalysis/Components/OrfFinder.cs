using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.DataStorage;
using PRRSAnalysis.ComponentLayouts;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;

namespace PRRSAnalysis.Components
{
    public class OrfFinder : SequenceLoop
    {
        private DataManager _dataManager;

        public OrfFinder(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public override void Run(string sequenceName)
        {
            string contents = _dataManager.SequencesUsed[sequenceName].Contents;
            Dictionary<string, int[]> allOrfs = findAllOrfs(contents);
            addOrfToData(_dataManager.SequencesUsed[sequenceName].OtherOrfData, allOrfs, contents);

            string json = JsonConvert.SerializeObject(_dataManager.SequencesUsed[sequenceName].OtherOrfData, Formatting.Indented);
            File.WriteAllText(_dataManager.DataFolder + sequenceName + "_allOrfs.json", json);
        }

        private Dictionary<string, int[]> findAllOrfs(string contents)
        {
            int seqeuenceLength = contents.Length;
            Dictionary<string, int[]> allOrfs = new Dictionary<string, int[]>();

            Dictionary<int, List<int>> StartCodons = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> StopCodons = new Dictionary<int, List<int>>();

            // Get complement sequence
            string complement = switchChars(contents, 'a', 'g');
            complement = switchChars(complement, 't', 'c');
            complement = reverse(complement);

            // Get all start and stop codons
            for (int i = 0; i < contents.Length - 2; i++)
            {
                string codon = contents.Substring(i, 3).ToLower();
                string rcodon = complement.Substring(i, 3).ToLower();
                foreach (string c in _dataManager.AminoAcidChart["Start"]) {
                    if (c == codon)
                    {
                        if (!StartCodons.ContainsKey((i % 3) + 1)) StartCodons.Add((i % 3) + 1, new List<int>());
                        StartCodons[(i % 3) + 1].Add(i);
                    }
                    else if (c == rcodon && _dataManager.RunReverseFrames)
                    {
                        if (!StartCodons.ContainsKey(-((i % 3) + 1))) StartCodons.Add(-((i % 3) + 1), new List<int>());
                        StartCodons[-((i % 3) + 1)].Add(i);
                    }
                }
                foreach (string c in _dataManager.AminoAcidChart["Stop"])
                {
                    if (c == codon)
                    {
                        if (!StopCodons.ContainsKey((i % 3) + 1)) StopCodons.Add((i % 3) + 1, new List<int>());
                        StopCodons[(i % 3) + 1].Add(i);
                    }
                    else if (c == rcodon && _dataManager.RunReverseFrames)
                    {
                        if (!StopCodons.ContainsKey(-((i % 3) + 1))) StopCodons.Add(-((i % 3) + 1), new List<int>());
                        StopCodons[-((i % 3) + 1)].Add(i);
                    }
                }
            }

            // Find all orfs
            int orfCount = 1;
            foreach(KeyValuePair<int, List<int>> startCondonPair in StartCodons)
            {
                foreach(KeyValuePair<int, List<int>> stopCodonPair in StopCodons)
                {
                    if(startCondonPair.Key == stopCodonPair.Key)
                    {
                        int previousEnd = 0;
                        foreach(int startPosition in startCondonPair.Value)
                        {
                            if (startPosition > previousEnd)
                            {
                                int remove = 0;
                                foreach (int stopPosition in stopCodonPair.Value)
                                {
                                    if (startPosition < stopPosition)
                                    {
                                        if (stopPosition - startPosition >= _dataManager.MinimumOrfLength)
                                        {
                                            allOrfs.Add("orf" + orfCount, new int[3] { startPosition, stopPosition, startCondonPair.Key });
                                            orfCount++;
                                        }
                                        remove = stopPosition;
                                        previousEnd = stopPosition;
                                        break;
                                    }
                                }
                                if (remove != 0) stopCodonPair.Value.Remove(remove);
                            }
                        }
                    }
                }
            }
            return allOrfs;   
        }
        private Dictionary<string, int[]> findKnownOrfs()
        {
            Dictionary<string, int[]> knownOrfs = new Dictionary<string, int[]>();
            return knownOrfs;
        }
        private string NucleotideToAminoAcid(string contents)
        {
            return "";
        }
        private void addOrfToData(Dictionary<string, OrfData> data, Dictionary<string, int[]> orfs, string contents)
        {
            foreach(KeyValuePair<string, int[]> orfDataPair in orfs)
            {
                if (!data.ContainsKey(orfDataPair.Key)) data.Add(orfDataPair.Key, new OrfData());
                int length = Math.Abs(orfDataPair.Value[1] - orfDataPair.Value[0]);
                string orfSequence = contents.Substring(orfDataPair.Value[0], length);
                data[orfDataPair.Key].Name = orfDataPair.Key;
                data[orfDataPair.Key].ContentsN = orfSequence;
                data[orfDataPair.Key].ContentsAA = NucleotideToAminoAcid(orfSequence);
                data[orfDataPair.Key].StartLocationN = orfDataPair.Value[0] + 1;
                data[orfDataPair.Key].EndLocationN = orfDataPair.Value[1] + 3;
                data[orfDataPair.Key].StartLocationAA = orfDataPair.Value[0] / 3;
                data[orfDataPair.Key].EndLocationAA = orfDataPair.Value[1] / 3;
                data[orfDataPair.Key].LengthN = length + 3;
                data[orfDataPair.Key].LengthA = length / 3 + 1;
                data[orfDataPair.Key].ReadingFrame = orfDataPair.Value[2];
            }            
        }

        #region Private Helper Methods

        private string switchChars(string s, char c1, char c2)
        {
            var result = s.Select(x => x == c1 ? c2 : (x == c2 ? c1 : x)).ToArray();
            return new string(result);
        }
        private static string reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        #endregion
    }
}
