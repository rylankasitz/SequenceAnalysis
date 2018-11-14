using PRRSAnalysis.AnalysisHelpers;
using PRRSAnalysis.DataStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.ComponentLayouts;
using System.Windows.Forms;
using System.IO;

namespace PRRSAnalysis.Components
{
    public class Alignment : AnalysisLoop
    {
        private DataManager _dataManager;
        private CommandlineRun _commandlineRun;

        public Alignment(DataManager dataManager)
        {
            Priority = 2;
            _dataManager = dataManager;
            _commandlineRun = new CommandlineRun();
        }

        public override void Run(string name, UpdateProgressBar updateProgressBar)
        {         
            bool isOrfFile = false;
            if (name != "Wholegenome") isOrfFile = true;
            _dataManager.CreateOneSequenceFile(name, isOrfFile);

            string outfile = _dataManager.AnalysisFiles[name] + "_aligned.fasta";
            _commandlineRun.ProgramName = "mafft-win\\mafft";
            _commandlineRun.Arguments = getArgs() + " --out " + outfile + " " + _dataManager.AnalysisFiles[name] + ".fasta";
            _commandlineRun.Run();

            if (!File.Exists(outfile))
            {
                StreamWriter writer = new StreamWriter(outfile);
                writer.Close();
            }

            _dataManager.Alignments.Add(name, new AlignmentData());
            _dataManager.Alignments[name].Contents = _dataManager.FileToSequences(outfile);
            _dataManager.Alignments[name].FileLocation = outfile;

            updateProgressBar((int)(400 / (float)_dataManager.AnalysisCount));
        }
        private string getArgs()
        {
            if(_dataManager.MafftSettings == "Fast")
            {
                return "--retree 2 --maxiterate 2 ";
            }
            else if(_dataManager.MafftSettings == "Accurate")
            {
                return "--globalpair --maxiterate 1000";
            }
            else
            {              
                return "--retree 1 --maxiterate 0";
            }
        }
    }
}
