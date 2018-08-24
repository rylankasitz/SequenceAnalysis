using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.DataStorage;
using PRRSAnalysis.ComponentLayouts;

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
            
        }

        private Dictionary<string, int[]> findAllOrfs(string contents)
        {
            Dictionary<string, int[]> allOrfs = new Dictionary<string, int[]>();
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
    }
}
