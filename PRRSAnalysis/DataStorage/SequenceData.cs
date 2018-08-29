using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRRSAnalysis.DataStorage
{
    public class SequenceData
    {
        public string Name { get; set; }
        public string Contents { get; set; }
        public Dictionary<string, OrfData> KnownOrfData { get; set; } = new Dictionary<string, OrfData>();
        public Dictionary<string, OrfData> OtherOrfData { get; set; } = new Dictionary<string, OrfData>();
    }

    public class OrfData
    {
        public string Name { get; set; }
        public string ContentsAA { get; set; }
        public string ContentsN { get; set; }
        public int StartLocationAA { get; set; } = 0;
        public int EndLocationAA { get; set; } = 0;
        public int StartLocationN { get; set; } = 0;
        public int EndLocationN { get; set; } = 0;
        public int LengthAA { get; set; } = 0;
        public int LengthN { get; set; } = 0;
        public int ReadingFrame { get; set; } = 1;
    }
}
