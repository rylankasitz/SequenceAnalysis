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
        public Dictionary<string, OrfData> OrfData { get; set; } = new Dictionary<string, OrfData>();
    }

    public class OrfData
    {
        public string Name { get; set; }
        public string ContentsAA { get; set; }
        public string ContentsN { get; set; }
        public int StartLocationAA { get; set; } = 0;
        public int EndLocationAA { get; set; } = 0;
        public int StartLocationNN { get; set; } = 0;
        public int EndLocationNN { get; set; } = 0;
        public int Length { get; set; } = 0;
    }
}
