using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRRSAnalysis.DataStorage
{
    public class RecombinationData
    {
        public int StartSite { get; set; }
        public int EndSite { get; set; }
        public int SequenceLength { get; set; }
        public string MajorParent { get; set; }
        public string MinorParent { get; set; }
    }
}
