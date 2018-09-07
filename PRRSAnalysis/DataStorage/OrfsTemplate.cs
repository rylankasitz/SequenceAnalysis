using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRRSAnalysis.DataStorage
{
    public class OrfsTemplate
    {
        public string VirusName { get; set; }
        public List<OrfTemplate> Orfs { get; set; }
    }

    public class OrfTemplate
    {
        public string Name { get; set; }
        public string Sequence { get; set; }
        public int LengthAA { get; set; }
        public bool HardSet { get; set; } = false;
        public int StartPosition { get; set; } = 0;
        public int EndPosition { get; set; } = 0;
    }
}
