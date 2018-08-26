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
        public int Start { get; set; }
        public int End { get; set; }
        public string Sequence { get; set; }

    }
}
