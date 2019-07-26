using System.Collections.Generic;

namespace LunalipseInstaller.Procedure
{
    public delegate void ProgressReporter(string currentTask, string currentTaskDetailed, double currentProgressPercentage);

    public class ProcedureManager
    {
        public static event ProgressReporter OnProgressUpdated;

        ProgressReporter reporter;
        List<IProcedure> procedures;
        public ProcedureManager(ProgressReporter reporter)
        {
            procedures = new List<IProcedure>();
            this.reporter = reporter;
            OnProgressUpdated += ProcedureHelper_OnProgressUpdated;
        }

        private void ProcedureHelper_OnProgressUpdated(string currentTask, string currentTaskDetailed, double currentProgressPercentage)
        {
            reporter.Invoke(currentTask, currentTaskDetailed, currentProgressPercentage);
        }

        public void AddProcedure(IProcedure procedure)
        {
            procedures.Add(procedure);
        }

        public void ClearAll()
        {
            procedures.Clear();
        }

        public IEnumerator<IProcedure> GetProcedures()
        {
            return procedures.GetEnumerator();
        }

        public static void UpdateProgress(string currentTask, string currentTaskDetailed, double currentProgressPercentage)
        {
            OnProgressUpdated.Invoke(currentTask, currentTaskDetailed, currentProgressPercentage);
        }
    }
}
