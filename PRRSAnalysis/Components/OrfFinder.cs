using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.DataStorage;
using PRRSAnalysis.ComponentLayouts;
using PRRSAnalysis.AnalysisHelpers;
using System.Collections.Specialized;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace PRRSAnalysis.Components
{
    public class OrfFinder : SequenceLoop
    {
        private DataManager _dataManager;
        private List<string> _currentSeqs;

        public OrfFinder(DataManager dataManager)
        {
            Priority = 1;
            _dataManager = dataManager;
            _currentSeqs = new List<string>();
        }

        public override void OnRunStart()
        {
            _currentSeqs = new List<string>();
        }

        public override void Run(string sequenceName, UpdateProgressBar updateProgressBar)
        {
            _currentSeqs.Add(sequenceName);
            string contents = _dataManager.SequencesUsed[sequenceName].Contents;

            Dictionary<string, int[]> allOrfs = findAllOrfs(contents);
            addOrfToData(_dataManager.SequencesUsed[sequenceName].OtherOrfData, allOrfs, contents);
            _dataManager.SequencesUsed[sequenceName].KnownOrfData = findKnownOrfs(_dataManager.SequencesUsed[sequenceName].OtherOrfData);
            getRidOfRepeates(_dataManager.SequencesUsed[sequenceName].KnownOrfData);

            foreach (List<string> orfs in _dataManager.CombinedOrfs)
            {
                try {
                    createCombinedOrf(orfs, sequenceName);
                }
                catch { }
            }
            matchOrfs(sequenceName);

            updateProgressBar((int) (70 / (float) _dataManager.SequenceCount));
        }

        /// <summary>
        /// Finds all of the orfs in a string of nucleotide sequences
        /// </summary>
        /// <param name="contents">Contents of file in nt</param>
        /// <returns>Dictionary of all orfs with location</returns>
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
                            if (startPosition >= previousEnd)
                            {
                                int remove = 0;
                                foreach (int stopPosition in stopCodonPair.Value)
                                {
                                    if (startPosition < stopPosition)
                                    {
                                        if (Math.Abs(stopPosition - startPosition) >= _dataManager.MinimumOrfLength)
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

        /// <summary>
        /// Compares all orfs to the orf templates to find all of the known orfs
        /// </summary>
        /// <param name="allOrfs"></param>
        /// <returns>Dictionary of orfs with location data</returns>
        private Dictionary<string, OrfData> findKnownOrfs(Dictionary<string, OrfData> allOrfs)
        {
            Dictionary<string, OrfData> knownOrfs = new Dictionary<string, OrfData>();
            Dictionary<string, List<OrfData>> potentialOrfs = new Dictionary<string, List<OrfData>>();
            OrfsTemplate orfTemplates = _dataManager.OrfTemplates[_dataManager.CurrentVirusKey];
            foreach(OrfTemplate orfTemplate in orfTemplates.Orfs)
            {
                // Find potential orfs
                foreach(KeyValuePair<string, OrfData> allOrfsPair in allOrfs)
                {
                    int lengthBuffer = Convert.ToInt32(orfTemplate.LengthAA * _dataManager.OrfLengthThreshold);
                    if(allOrfsPair.Value.LengthAA > orfTemplate.LengthAA - lengthBuffer && 
                       allOrfsPair.Value.LengthAA < orfTemplate.LengthAA + lengthBuffer)
                    {
                        if (_dataManager.PartialOrfFile || (allOrfsPair.Value.StartLocationN - _dataManager.OrfSiteRange < orfTemplate.StartSite &&
                            allOrfsPair.Value.StartLocationN + _dataManager.OrfSiteRange > orfTemplate.StartSite)){
                            if (!potentialOrfs.ContainsKey(orfTemplate.Name)) potentialOrfs.Add(orfTemplate.Name, new List<OrfData>());
                            potentialOrfs[orfTemplate.Name].Add(allOrfsPair.Value);
                        }
                    }
                }

                // Get hardset
                if (orfTemplate.HardSet && potentialOrfs.ContainsKey(orfTemplate.Name))
                {
                    for(int i = 0; i < potentialOrfs[orfTemplate.Name].Count; i++)
                    {
                        potentialOrfs[orfTemplate.Name][i].StartLocationN = potentialOrfs[orfTemplate.Name][i].EndLocationN - orfTemplate.LengthAA * 3;
                        potentialOrfs[orfTemplate.Name][i].StartLocationAA = potentialOrfs[orfTemplate.Name][i].EndLocationAA - orfTemplate.LengthAA;
                        potentialOrfs[orfTemplate.Name][i].LengthAA = orfTemplate.LengthAA;
                        potentialOrfs[orfTemplate.Name][i].LengthN = orfTemplate.LengthAA * 3;
                    }
                }

                // Find closest related orf
                bool found = false;
                KeyValuePair<float, OrfData> highestOrf = new KeyValuePair<float, OrfData>(_dataManager.OrfIdentifierPIThreshold, new OrfData());
                if (potentialOrfs.ContainsKey(orfTemplate.Name))
                {
                    float pi = 0;
                    foreach (OrfData potentialOrfPair in potentialOrfs[orfTemplate.Name])
                    {
                        pi = GlobalCalculations.CalculatePercentIdentity(potentialOrfPair.ContentsAA, orfTemplate.Sequence);                                                  
                        if (pi > highestOrf.Key)
                        {
                            highestOrf = new KeyValuePair<float, OrfData>(pi, potentialOrfPair);
                            found = true;
                        }
                    }
                    if (found) {
                        knownOrfs.Add(orfTemplate.Name, highestOrf.Value);
                        knownOrfs[orfTemplate.Name].Name = orfTemplate.Name;
                        if (!_dataManager.AnalysisNames.Contains(orfTemplate.Name + "_n"))
                            _dataManager.AnalysisNames.Add(orfTemplate.Name + "_n");
                        if (!_dataManager.AnalysisNames.Contains(orfTemplate.Name + "_aa"))
                            _dataManager.AnalysisNames.Add(orfTemplate.Name + "_aa");
                    }
                }
            }
            return knownOrfs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="knownOrfs"></param>
        private void getRidOfRepeates(Dictionary<string, OrfData> knownOrfs)
        {
            Dictionary<string, OrfData> knownOrfsCopy = new Dictionary<string, OrfData>(knownOrfs);
            Dictionary<string, OrfData> knownOrfsCopy1 = new Dictionary<string, OrfData>(knownOrfs);
            foreach (KeyValuePair<string, OrfData> orfpair in knownOrfsCopy)
            {
                var matches = knownOrfsCopy1.Where(pair => pair.Value.StartLocationN == orfpair.Value.StartLocationN);
                KeyValuePair<string, OrfData> highestRelated = new KeyValuePair<string, OrfData>(orfpair.Key, orfpair.Value);
                int templateLength = findOrfTemplate(orfpair.Key).LengthAA;
                int lowestLenDiff = templateLength;
                foreach (KeyValuePair<string, OrfData> matchOrfPair in matches)
                {
                    if(Math.Abs(highestRelated.Value.LengthAA - templateLength) < lowestLenDiff)
                    {
                        highestRelated = matchOrfPair;
                    }
                }
                foreach (KeyValuePair<string, OrfData> matchOrfPair in matches)
                {
                    if (matchOrfPair.Key != highestRelated.Key)
                    {
                        knownOrfs.Remove(matchOrfPair.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Matches all orfs so each sequence has the same orfs
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contents"></param>
        /// <param name="sequencename"></param>
        private void matchOrfs(string sequence)
        {
            List<string> unsharedOrfs = new List<string>();
            string firstKey = _dataManager.SequencesUsed.Keys.First();
            foreach(KeyValuePair<string, OrfData> orfPair in _dataManager.SequencesUsed[sequence].KnownOrfData)
            {
                int totalMatched = 0;
                foreach(string seq in _currentSeqs)
                {
                    if (_dataManager.SequencesUsed[seq].KnownOrfData.ContainsKey(orfPair.Key)) totalMatched++;
                }
                if (totalMatched != _currentSeqs.Count) unsharedOrfs.Add(orfPair.Key);
            }
            foreach(string orf in unsharedOrfs)
            {
                foreach (string seq in _currentSeqs)
                {
                    if (_dataManager.SequencesUsed[seq].KnownOrfData.ContainsKey(orf))
                    {
                        _dataManager.SequencesUsed[seq].KnownOrfData.Remove(orf);
                        _dataManager.AnalysisNames.Remove(orf + "_n");
                        _dataManager.AnalysisNames.Remove(orf + "_a");
                    }
                }
            }
        }

        /// <summary>
        /// Adds orfs the the data manager
        /// </summary>
        /// <param name="data"></param>
        /// <param name="orfs"></param>
        /// <param name="contents"></param>
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
                data[orfDataPair.Key].LengthAA = length / 3 + 1;
                data[orfDataPair.Key].ReadingFrame = orfDataPair.Value[2]; 
            }            
        }

        #region Private Helper Methods

        private void createCombinedOrf(List<string> orfs, string sequencename)
        {
            string newName = orfs[0] + "-" + orfs[orfs.Count-1];
            string contentsN = "";
            string contentsA = "";
            /*Dictionary<string, int[]> newOrf = new Dictionary<string, int[]>();
            int start = _dataManager.SequencesUsed[sequencename].KnownOrfData[startOrf].StartLocationN;
            int end = _dataManager.SequencesUsed[sequencename].KnownOrfData[endOrf].EndLocationN;
            newOrf[newName] = new int[3];
            newOrf[startOrf + "-" + endOrf][0] = start;
            newOrf[startOrf + "-" + endOrf][1] = end;
            //addOrfToData(_dataManager.SequencesUsed[sequencename].KnownOrfData, newOrf, contents);*/
            foreach(KeyValuePair<string, OrfData> orfData in _dataManager.SequencesUsed[sequencename].KnownOrfData)
            {
                if (orfs.Contains(orfData.Key))
                {
                    contentsN += orfData.Value.ContentsN;
                    contentsA += orfData.Value.ContentsAA;
                }
            }

            _dataManager.SequencesUsed[sequencename].KnownOrfData[newName] = new OrfData();
            _dataManager.SequencesUsed[sequencename].KnownOrfData[newName].ContentsN = contentsN;
            _dataManager.SequencesUsed[sequencename].KnownOrfData[newName].ContentsAA = contentsA;
            _dataManager.SequencesUsed[sequencename].KnownOrfData[newName].Name = newName;

            if (!_dataManager.AnalysisNames.Contains(newName + "_n"))
                _dataManager.AnalysisNames.Add(newName + "_n");
            if (!_dataManager.AnalysisNames.Contains(newName + "_aa"))
                _dataManager.AnalysisNames.Add(newName + "_aa");
        }
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
        private string NucleotideToAminoAcid(string contents)
        {
            string aminoSeq = "";
            for(int i = 0; i < contents.Length - 3; i+=3)
            {
                string codon = contents.Substring(i, 3);
                string aminoAcid = findAminoAcid(codon);
                aminoSeq += aminoAcid;
            }
            return aminoSeq;
        }
        private string findAminoAcid(string codon)
        {
            foreach(KeyValuePair<string, string[]> keyValuePair in _dataManager.AminoAcidChart)
            {
                foreach(string c in keyValuePair.Value)
                {
                    if (c == codon.ToLower() && keyValuePair.Key != "Start" && keyValuePair.Key != "Stop") return keyValuePair.Key;
                }
            }
            return "";
        }
        private OrfTemplate findOrfTemplate(string name)
        {
            foreach(OrfTemplate orfTemplate in _dataManager.OrfTemplates[_dataManager.CurrentVirusKey].Orfs)
            {
                if (orfTemplate.Name == name) return orfTemplate;
            }
            return null;
        }

        #endregion
    }
}
