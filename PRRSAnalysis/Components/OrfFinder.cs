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
            //if (sequenceName == "Wholegenome")
           // {
                Alignment alignment = new Alignment(_dataManager);
                alignment.Run("Wholegenome", updateProgressBar);
                //return;
            //}

            //_currentSeqs.Add(sequenceName);
            
            Dictionary<string, int[]> orfAlignedLocations = findOrfAlignmentLocations("P129_Partent_of_Fostera");
            foreach (string seqName in _dataManager.SequencesUsed.Keys)
            {
                string contents = _dataManager.SequencesUsed[seqName].Contents;
                string alignedContents = _dataManager.Alignments["Wholegenome"].Contents[seqName];
                Dictionary<string, int[]> allOrfs = findAllOrfs(alignedContents, true);
                _dataManager.SequencesUsed[seqName].KnownOrfData = findKnownOrfs(seqName, orfAlignedLocations, alignedContents);
                _dataManager.SequencesUsed[seqName].OtherOrfData = findUnknownOrfs(allOrfs, contents);
                foreach (List<string> orfs in _dataManager.CombinedOrfs)
                {
                    try
                    {
                        createCombinedOrf(orfs, seqName);
                    }
                    catch { }
                }
            }

            //Dictionary<string, int[]> allOrfs = findAllOrfs(alignedContents);
            //addOrfToData(_dataManager.SequencesUsed[sequenceName].OtherOrfData, allOrfs, contents);
            //_dataManager.SequencesUsed[sequenceName].KnownOrfData = findKnownOrfs(_dataManager.SequencesUsed[sequenceName].OtherOrfData);
            //getRidOfRepeates(_dataManager.SequencesUsed[sequenceName].KnownOrfData);

            
            //matchOrfs(sequenceName);

            updateProgressBar((int)(70 / (float)_dataManager.SequenceCount));
        }

        private Dictionary<string, int[]> findAllOrfs(string contents, bool removeGaps = false)
        {
            int seqeuenceLength = contents.Length;
            Dictionary<string, int[]> allOrfs = new Dictionary<string, int[]>();

            Dictionary<int, List<int>> StartCodons = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> StopCodons = new Dictionary<int, List<int>>();

            /*// Get complement sequence
            string complement = switchChars(contents, 'a', 'g');
            complement = switchChars(complement, 't', 'c');
            complement = reverse(complement);*/
            int frameCount = 0;
            int gapCount = 0;

            // Get all start and stop codons
            for (int i = 0; i < contents.Length - 2; i++)
            {
                string codon = ""; int codonCount = 0;
                //string rcodon = "---";
                if (contents[i] == '-')
                {
                    if (removeGaps) gapCount++;
                    continue;                  
                }
                if (frameCount < 3) frameCount++;
                else frameCount = 1;
            
                while (codon.Length < 3)
                {
                    if ((i + codonCount) >= contents.Length) break;
                    if (contents[i + codonCount] != '-')
                    {
                        codon += contents[i + codonCount];
                    }
                    codonCount++;
                }

                foreach (string c in _dataManager.AminoAcidChart["Start"])
                {
                    if (c == codon)
                    {
                        if (!StartCodons.ContainsKey(frameCount)) StartCodons.Add(frameCount, new List<int>());
                        StartCodons[frameCount].Add(i - gapCount);
                    }
                    /*else if (c == rcodon && _dataManager.RunReverseFrames)
                    {
                        if (!StartCodons.ContainsKey(-((i % 3) + 1))) StartCodons.Add(-((i % 3) + 1), new List<int>());
                        StartCodons[-((i % 3) + 1)].Add(i);
                    }*/
                }
                foreach (string c in _dataManager.AminoAcidChart["Stop"])
                {
                    if (c == codon)
                    {
                        if (!StopCodons.ContainsKey(frameCount)) StopCodons.Add(frameCount, new List<int>());
                        StopCodons[frameCount].Add(i - gapCount);
                    }
                    /*else if (c == rcodon && _dataManager.RunReverseFrames)
                    {
                        if (!StopCodons.ContainsKey(-((i % 3) + 1))) StopCodons.Add(-((i % 3) + 1), new List<int>());
                        StopCodons[-((i % 3) + 1)].Add(i);
                    }*/
                }
            }

            // Find all orfs
            int orfCount = 1;
            foreach (KeyValuePair<int, List<int>> startCondonPair in StartCodons)
            {
                foreach (KeyValuePair<int, List<int>> stopCodonPair in StopCodons)
                {
                    if (startCondonPair.Key == stopCodonPair.Key)
                    {
                        int previousEnd = 0;
                        foreach (int startPosition in startCondonPair.Value)
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

        private Dictionary<string, OrfData> findKnownOrfs(string sequence, Dictionary<string, int[]> locations, string alignedContents)
        {
            Dictionary<string, OrfData> orfData = new Dictionary<string, OrfData>();
            foreach (KeyValuePair<string, int[]> location in locations)
            {
                string orfContents_n = alignedContents.Substring(location.Value[0], location.Value[1] - location.Value[0] + 3).Replace("-", "");
                string orfContents_aa = NucleotideToAminoAcid(orfContents_n);
                int startPos_n = location.Value[0] - alignedContents.Substring(0, location.Value[0] + 1).Count(x => x == '-') + 1;
                int endPos_n = location.Value[1] - alignedContents.Substring(0, location.Value[1] + 1).Count(x => x == '-') + 3;
                int startPos_aa = startPos_n / 3;
                int endPos_aa = endPos_n / 3;
                orfData[location.Key] = new OrfData()
                {
                    Name = location.Key,
                    ContentsAA = orfContents_aa,
                    ContentsN = orfContents_n,
                    StartLocationAA = startPos_aa,
                    EndLocationAA = endPos_aa,
                    StartLocationN = startPos_n,
                    EndLocationN = endPos_n,
                    LengthAA = endPos_aa - startPos_aa,
                    LengthN = endPos_n - startPos_n
                };            
            }
            return orfData;
        }

        private Dictionary<string, OrfData> findUnknownOrfs(Dictionary<string, int[]> orfData, string contents)
        {
            Dictionary<string, OrfData> unknownData = new Dictionary<string, OrfData>();
            foreach (KeyValuePair<string, int[]> orf in orfData)
            {
                string contents_n = contents.Substring(orf.Value[0], orf.Value[1] - orf.Value[0]);
                string contents_aa = NucleotideToAminoAcid(contents_n);
                unknownData[orf.Key] = new OrfData()
                {
                    Name = orf.Key,
                    StartLocationN = orf.Value[0] + 1,
                    EndLocationN = orf.Value[1],
                    StartLocationAA = (orf.Value[0] + 1) / 3,
                    EndLocationAA = (orf.Value[1]) / 3,
                    ContentsN = contents_n,
                    ContentsAA = contents_aa,
                    LengthAA = contents_aa.Length,
                    LengthN = contents_n.Length,
                    ReadingFrame = orf.Value[2]
                };
            }
            return unknownData;
        }
        
        /// <summary>
        /// Compares all orfs to the orf templates to find all of the known orfs
        /// </summary>
        /// <param name="allOrfs"></param>
        /// <returns>Dictionary of orfs with location data</returns>
        /*private Dictionary<string, OrfData> findKnownOrfs(Dictionary<string, OrfData> allOrfs)
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
        }*/
        
        private Dictionary<string, int[]> findOrfAlignmentLocations(string relatedSequence)
        {
            Dictionary<string, int[]> locations = new Dictionary<string, int[]>();
            OrfsTemplate orfTemplates = _dataManager.OrfTemplates[_dataManager.CurrentVirusKey];
            string contents = _dataManager.Alignments["Wholegenome"].Contents[relatedSequence];
            Dictionary<string, int[]> allOrfs = findAllOrfs(contents);
            foreach (OrfTemplate orfTemplate in orfTemplates.Orfs)
            {              
                int[] closestOrf = new int[3];
                float highestPI = 0;
                foreach(int[] orf in allOrfs.Values)
                {
                    int start = orf[0];
                    if (orfTemplate.HardSet)
                    {
                        start = findStartPos(contents.Replace("-","").Substring(orfTemplate.StartSite - 1, orfTemplate.LengthAA * 3), contents) - 2;
                        if (start > orf[1]) start = 0;
                    }        
                    string newOrfContents = NucleotideToAminoAcid(contents.Substring(start, orf[1] - start).Replace("-", ""));
                    float pi = GlobalCalculations.CalculatePercentIdentity(newOrfContents, orfTemplate.Sequence);
                    if (pi > highestPI) { highestPI = pi; closestOrf = new int[3] { start, orf[1], orf[2] }; }
                }
                locations[orfTemplate.Name] = closestOrf;
                if (!_dataManager.AnalysisNames.Contains(orfTemplate.Name + "_n"))
                    _dataManager.AnalysisNames.Add(orfTemplate.Name + "_n");
                if (!_dataManager.AnalysisNames.Contains(orfTemplate.Name + "_aa"))
                    _dataManager.AnalysisNames.Add(orfTemplate.Name + "_aa");
            }
            return locations;
        }

        /// <summary>
        /// Gets rid of all repeated orfs in a dictionary
        /// </summary>
        /// <param name="knownOrfs"></param>
        /*private void getRidOfRepeates(Dictionary<string, OrfData> knownOrfs)
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
        }*/

        /// <summary>
        /// Matches all orfs so each sequence has the same orfs
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contents"></param>
        /// <param name="sequencename"></param>
        /*private void matchOrfs(string sequence)
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
                        _dataManager.AnalysisNames.Remove(orf + "_aa");
                    }
                }
            }
        }*/

        /// <summary>
        /// Adds orfs the the data manager
        /// </summary>
        /// <param name="data"></param>
        /// <param name="orfs"></param>
        /// <param name="contents"></param>
        /*private void addOrfToData(Dictionary<string, OrfData> data, Dictionary<string, int[]> orfs, string contents)
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
        }*/

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
        /*private OrfTemplate findOrfTemplate(string name)
        {
            foreach(OrfTemplate orfTemplate in _dataManager.OrfTemplates[_dataManager.CurrentVirusKey].Orfs)
            {
                if (orfTemplate.Name == name) return orfTemplate;
            }
            return null;
        }*/
        private int findStartPos(string comparedSeq, string alignedSeq)
        {
            int thres = 10;
            for(int i = 0; i < alignedSeq.Length; i++)
            {
                int c = 0; int nc = 0;
                while(nc < thres)
                {
                    if(alignedSeq[i + c] != '-')
                    {
                        if(alignedSeq[i + c] == comparedSeq[nc])
                        {
                            nc++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    c++;
                    if (nc == thres) return i;
                }
            }
            return 0;
        }

        #endregion
    }
}
