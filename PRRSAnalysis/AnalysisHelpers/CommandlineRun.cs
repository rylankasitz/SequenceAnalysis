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
        public string Arguments { get; set; } = "";
        public string AplicationPath { get; set; } = "_ApplicationFiles\\";
        public bool CommandLine { get; set; } = false;

        private Process _process;

        public CommandlineRun()
        {
            _process = new Process();
            _process.StartInfo.UseShellExecute = true;
            _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _process.StartInfo.CreateNoWindow = false;
            _process.EnableRaisingEvents = true;      
            _process.StartInfo.FileName = "cmd.exe";
        }

        public void Run()
        {
            
            if (CommandLine)
            {
                _process.StartInfo.FileName = "cmd.exe";
                _process.StartInfo.Arguments = "/C " + AplicationPath + ProgramName + " " + Arguments;
            }
            else
            {
                _process.StartInfo.FileName = new FileInfo(AplicationPath + ProgramName).FullName;
                _process.StartInfo.Arguments = Arguments;
            }                  
            _process.Start();
            _process.WaitForExit();          
        }
    }
}
