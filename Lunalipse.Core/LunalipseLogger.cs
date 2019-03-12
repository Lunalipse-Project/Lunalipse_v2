using Lunalipse.Common;
using Lunalipse.Common.Data.Errors;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lunalipse.Core
{
    public class LunalipseLogger
    {
        static volatile LunalipseLogger LLoggerInstance;
        static readonly object LLogger_Lock = new object();

        public static LunalipseLogger GetLogger()
        {
            if (LLoggerInstance == null)
            {
                lock (LLogger_Lock)
                {
                    LLoggerInstance = LLoggerInstance ?? new LunalipseLogger();
                }
            }
            return LLoggerInstance;
        }

        private string ApplicationEnvPath;
        private StreamWriter logWriter;

        public LunalipseLogger()
        {
            ErrorDelegation.GenericError += ErrorDelegation_GenericError;
            ApplicationEnvPath = Environment.CurrentDirectory;
            if (!Directory.Exists(ApplicationEnvPath + @"\Logs\"))
                Directory.CreateDirectory(ApplicationEnvPath + @"\Logs\");
            logWriter = new StreamWriter(ApplicationEnvPath + @"\Logs\" + GetLogName());
        }

        private void ErrorDelegation_GenericError(string ExceptionMsg, string ExceptionStackTrace, string componentID = "")
        {
            Error(ExceptionMsg, ExceptionStackTrace, componentID);
        }

        public void Error(string message, string trace = "", string ID = "")
        {
            string name = ID == "" ? new StackFrame(1).GetMethod().ReflectedType.Name : ID;
            logWriter.WriteLine(FormateMessage("ERROR", name, message));
            if (trace != "")
            {
                logWriter.WriteLine(trace);
            }
        }
        public void Exception(Exception e, string trace = "", string ID = "")
        {
            string name = ID == "" ? new StackFrame(1).GetMethod().ReflectedType.Name : ID;
            Error(" ======= Exception =======", name);
            Error(e.Message, name);
            Error(e.Source, name);
            Error(e.StackTrace, name);
            if (e.InnerException == null) return;
            Error(" ======= Inner Exception =======");
            Error(e.InnerException.Message, name);
            Error(e.InnerException.Source, name);
            Error(e.InnerException.StackTrace, name);
            Error(e.ToString());
        }

        public void Debug(string message, string ID = "")
        {
            string name = ID == "" ? new StackFrame(1).GetMethod().ReflectedType.Name : ID;
            logWriter.WriteLine(FormateMessage("DEBUG", name, message));
        }

        public void Info(string message, string ID = "")
        {
            string name = ID == "" ? new StackFrame(1).GetMethod().ReflectedType.Name : ID;
            logWriter.WriteLine(FormateMessage("INFO", name, message));
        }

        public void Warning(string message, string ID = "")
        {
            string name = ID == "" ? new StackFrame(1).GetMethod().ReflectedType.Name : ID;
            logWriter.WriteLine(FormateMessage("Warning", name, message));
        }

        public void Release()
        {
            logWriter.Flush();
            logWriter.Close();
        }

        private string GetLogName()
        {
            DateTime dt = DateTime.Now;
            return "Lunalipse_{0}.log".FormateEx(dt.ToString("MM-dd-yy HHmmss"));
        }

        private string FormateMessage(string TypeOfMessage, string ComponentName, string Message)
        {
            return "{0} [{1}] ({2}) : {3}".FormateEx(DateTime.Now.ToString("HH:mm:ss"), TypeOfMessage, ComponentName, Message);
        }
    }
}
