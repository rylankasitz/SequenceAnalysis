using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PRRSAnalysis.DataStorage;
using PRRSAnalysis;
using PRRSAnalysis.AnalysisHelpers;
using System.Diagnostics;
using System.IO;

namespace PRRSAnalysis
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DataManager dataManager = new DataManager();
            dataManager.LoadData();
            dataManager.DeserializeJsonFiles();

            // Install RDP4
            if (!dataManager.RDP4Installed)
            {
                try
                {
                    Process install = new Process();
                    install.StartInfo.FileName = "_ApplicationFiles\\RDP4.96BetaBig.exe";
                    install.StartInfo.Arguments = "";
                    install.Start();
                    install.WaitForExit();
                    Console.WriteLine(install.ExitCode);
                    if(install.ExitCode == 0) dataManager.RDP4Installed = true;
                }
                catch
                {
                    MessageBox.Show("Unable to install RDP4");
                }
            }
            // Initialize File Folders
            if (!dataManager.FilesInitialized)
            {
                try
                {
                    dataManager.CreateDirectory(dataManager.MainOutputFolder);
                    dataManager.CreateDirectory(Path.GetDirectoryName(dataManager.VaccineLocation));

                    DirectoryInfo dir = new DirectoryInfo("_VaccineFiles\\");
                    FileInfo[] files = dir.GetFiles("*.fasta");
                    foreach (FileInfo f in files)
                        dataManager.MoveFile(f.FullName, new FileInfo(dataManager.VaccineLocation).DirectoryName);
                    dataManager.FilesInitialized = true;
                }
                catch
                {
                    MessageBox.Show("Unable to initialize files");
                }
                
            }
            

            ComponentPool analysisPool = new ComponentPool(dataManager);
            UserInterface userInterface = new UserInterface(dataManager, analysisPool.RunAnalysis);

            Application.Run(userInterface);
        }
    }
}
