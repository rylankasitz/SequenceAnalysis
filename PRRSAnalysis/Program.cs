﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PRRSAnalysis.DataStorage;
using PRRSAnalysis;

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
            
            ComponentPool analysisPool = new ComponentPool(dataManager);
            UserInterface userInterface = new UserInterface(dataManager, analysisPool.RunAnalysis);

            Application.Run(userInterface);
        }
    }
}
