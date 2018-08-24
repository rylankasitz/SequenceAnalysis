using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRRSAnalysis.DataStorage
{
    public class PercentIdentityData
    {
        public string Name { get; set; }
        public Dictionary<string, float> PercentIdentityComparisons { get; set; } = new Dictionary<string, float>();
    }
}
