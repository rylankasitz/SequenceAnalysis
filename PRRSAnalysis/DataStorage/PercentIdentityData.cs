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
        public List<string> Sequences { get; set; } = new List<string>();
        public List<List<float>> Data { get; set; } = new List<List<float>>();
        public Dictionary<string, Dictionary<string, float>> Dic { get; set; } = new Dictionary<string, Dictionary<string, float>>();
        public Dictionary<string, Dictionary<string, float>> DicInverse { get; set; } = new Dictionary<string, Dictionary<string, float>>();
        public Dictionary<string, Dictionary<int, Dictionary<string, string>>> SiteChanges { get; set; } = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
    }
}
