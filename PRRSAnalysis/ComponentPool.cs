using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PRRSAnalysis.DataStorage;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.ComponentLayouts;
using System.Threading;
using System.Collections;
using PRRSAnalysis.Output;
using PRRSAnalysis.Components;
using System.Windows.Forms;

public delegate void RunDelegate(UpdateProgressBar updateProgressBar);

namespace PRRSAnalysis
{
    public class ComponentPool
    {
        public RunDelegate RunAnalysis;

        private IEnumerable<AnalysisLoop> _analysisComponents;
        private IEnumerable<SequenceLoop> _sequenceLoop;
        private IEnumerable<SingleLoop> _singleLoops;
        private DataManager _dataManager;
        private UpdateProgressBar _updateProgressBar;

        public ComponentPool(DataManager dataManager)
        {
            RunAnalysis = new RunDelegate(Run);  
            _analysisComponents = GetEnumerableOfType<AnalysisLoop>(dataManager).OrderBy(s => s.Priority);
            _sequenceLoop = GetEnumerableOfType<SequenceLoop>(dataManager).OrderBy(s => s.Priority);
            _singleLoops = GetEnumerableOfType<SingleLoop>(dataManager).OrderBy(s => s.Priority);
            _dataManager = dataManager;
            _dataManager.AnalysisNames.Add("Wholegenome");
        }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            return objects;
        }

        public void Run(UpdateProgressBar updateProgressBar)
        {
            _updateProgressBar = updateProgressBar;

            Thread thread = new Thread(new ThreadStart(ThreadProc));
            thread.Start();
        }
        public void ThreadProc()
        {
            try
            {
                DateTime startTime = DateTime.Now;
                _dataManager.SequenceCount = _dataManager.SequencesUsed.Count;
                foreach (SequenceLoop component in _sequenceLoop)
                {
                    component.OnRunStart();
                    foreach (string sequence in _dataManager.SequencesUsed.Keys)
                    {
                        component.Run(sequence, _updateProgressBar);
                    }
                }
                _dataManager.AnalysisCount = _dataManager.AnalysisNames.Count;
                foreach (AnalysisLoop component in _analysisComponents)
                {
                    foreach (string analysisName in _dataManager.AnalysisNames)
                    {
                        component.Run(analysisName, _updateProgressBar);
                    }
                }
                foreach (SingleLoop singleLoop in _singleLoops)
                {    
                    singleLoop.Run(_updateProgressBar);
                }
                _dataManager.RunTime = DateTime.Now - startTime;  
                MessageBox.Show("Analysis Finished\n" + "Time Elapesed: " + _dataManager.RunTime.ToString(@"hh\:mm\:ss"));              
            }
            catch(Exception e)
            {
                MessageBox.Show("Analysis Failed\n" + e.Message);
                _updateProgressBar(1000);
            }
        }
    }
}
