﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseUpdate.Procedures
{
    public delegate void ProgressReporter(string currentTask, double currentProgressPercentage);
    public class ProcedureHelper
    {
        /*
         * lrss : 未压缩的LRSS资源文件
         * lrs  : 压缩的LRSS资源文件
         */
        public static string baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static string updateFolder = baseDir + "/mcdata/";
        public static string updateCompressedPackage = baseDir + "/mcdata/pack.lrs";
        public static string updateManifest = baseDir + "/mcdata/manifest.xml";


        public static event ProgressReporter OnProgressUpdated;

        ProgressReporter reporter;
        List<IProcedure> procedures;
        public ProcedureHelper(ProgressReporter reporter)
        {
            
            procedures = new List<IProcedure>();
            this.reporter = reporter;
            OnProgressUpdated += ProcedureHelper_OnProgressUpdated;
            if (!Directory.Exists(updateFolder)) Directory.CreateDirectory(updateFolder);
        }

        private void ProcedureHelper_OnProgressUpdated(string currentTask, double currentProgressPercentage)
        {
            reporter.Invoke(currentTask, currentProgressPercentage);
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

        public static void UpdateProgress(string currentTask, double currentProgressPercentage)
        {
            OnProgressUpdated.Invoke(currentTask, currentProgressPercentage);
        }

        
    }
}
