using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRRSAnalysis.ComponentLayouts
{
    public abstract class SingleLoop
    {
        public int Priority { get; set; } = 3;
        public abstract void Run(); 
    }
    public abstract class SequenceLoop
    {
        public int Priority { get; set; } = 3;
        public abstract void Run(string sequenceName);
    }
    public abstract class AnalysisLoop
    {
        public int Priority { get; set; } = 3;
        public abstract void Run(string analysisItem);
    }
}
