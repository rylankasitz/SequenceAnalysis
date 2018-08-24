using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PRRSAnalysis.DataStorage;
using System.Text;
using System.Threading.Tasks;
using PRRSAnalysis.ComponentLayouts;
using System.Threading;

public delegate void RunDelegate();

namespace PRRSAnalysis
{
    public class ComponentPool
    {
        public RunDelegate RunAnalysis;

        private IEnumerable<AnalysisLoop> _analysisComponents;
        private IEnumerable<SequenceLoop> _sequenceLoop;
        private DataManager _dataManager;

        public ComponentPool(DataManager dataManager)
        {
            RunAnalysis = new RunDelegate(Run);
            _analysisComponents = GetEnumerableOfType<AnalysisLoop>(dataManager);
            _sequenceLoop = GetEnumerableOfType<SequenceLoop>(dataManager);
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

        public void Run()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;           
                foreach (SequenceLoop component in _sequenceLoop)
                {
                    foreach (string sequence in _dataManager.SequencesUsed.Keys)
                    {
                        component.Run(sequence);
                    }
                }
                foreach (AnalysisLoop component in _analysisComponents)
                {
                    foreach(string analysisName in _dataManager.AnalysisNames)
                    {
                        component.Run(analysisName);
                    }     
                }
            }).Start();
        }
    }
}
