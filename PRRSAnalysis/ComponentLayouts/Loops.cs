﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRRSAnalysis.ComponentLayouts
{
    public abstract class InitializeRun
    {
        public abstract void Initialize();
    }
    public abstract class InitialLoop
    {
        public int Priority { get; set; } = 3;
        public abstract void Run(string name, UpdateProgressBar updateProgressBar);
    }
    public abstract class SingleLoop
    {
        public int Priority { get; set; } = 3;
        public abstract void Run(UpdateProgressBar updateProgressBar); 
    }
    public abstract class SequenceLoop
    {     
        public int Priority { get; set; } = 3;
        public abstract void OnRunStart();
        public abstract void Run(string sequenceName, UpdateProgressBar updateProgressBar);
    }
    public abstract class AnalysisLoop
    {
        public int Priority { get; set; } = 3;
        public abstract void Run(string analysisItem, UpdateProgressBar updateProgressBar);
    }
}
