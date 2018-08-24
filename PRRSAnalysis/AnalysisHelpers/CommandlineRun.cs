using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace PRRSAnalysis.AnalysisHelpers
{
    public class CommandlineRun
    {
        public string ProgramName { get; set; }
        public string Arguments { get; set; }

        private string _binPath;
        private Process _process;

        public CommandlineRun()
        {
            _binPath = "_ApplicationFiles\\";
            _process = new Process();
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _process.StartInfo.CreateNoWindow = true;
            _process.StartInfo.FileName = "cmd.exe";
        }

        public void Run()
        {
            _process.StartInfo.Arguments =  "/C " + _binPath + ProgramName + " " + Arguments;
            _process.Start();
            _process.WaitForExit();
        }
    }
}
