using Lunalipse.Common;
using Lunalipse.Common.Data;
using Lunalipse.Common.Data.Errors;
using Lunalipse.Utilities;
using Lunalipse.Utilities.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public LogLevel LogLevel;

        readonly object LOCKER = new object();

        public LunalipseLogger()
        {
            ErrorDelegation.GenericError += ErrorDelegation_GenericError;
            ApplicationEnvPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (!Directory.Exists(ApplicationEnvPath + @"\Logs\"))
                Directory.CreateDirectory(ApplicationEnvPath + @"\Logs\");
            logWriter = new StreamWriter(ApplicationEnvPath + @"\Logs\" + GetLogName());
#if DEBUG
            LogLevel = LogLevel.DEBUG;
#else
            LogLevel = LogLevel.INFO;
#endif
            // Fix: WMI service not found on windows 7.
            // Log processor info only on windows 10
            if (Environment.OSVersion.Version.Major == 10) 
            {
                logWriter.Write(SystemHelper.GetProcessorInfo());
                logWriter.Write(SystemHelper.GetOSInfo());
            }
            logWriter.WriteLine();
            logWriter.Flush();
        }

        private void ErrorDelegation_GenericError(string ExceptionMsg, string ExceptionStackTrace, string componentID = "")
        {
            Error(ExceptionMsg, ExceptionStackTrace, componentID);
        }

        public void Error(string message, string trace = "", string ID = "")
        {
            try
            {
                if (LogLevel <= LogLevel.ERROR)
                {
                    string name = ID == "" ? new StackFrame(1).GetMethod().ReflectedType.Name : ID;
                    WriteStuff(FormatingMessage("ERROR", name, message));
                    if (trace != "")
                    {
                        WriteStuff(trace);
                    }
                }
            }
            catch
            {

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
            logWriter.Flush();
        }

        public void Debug(string message, string ID = "")
        {
            if(LogLevel==LogLevel.DEBUG)
            {
                string name = ID == "" ? new StackFrame(1).GetMethod().ReflectedType.Name : ID;
                WriteStuff(FormatingMessage("DEBUG", name, message));
            }
        }

        public void Info(string message, string ID = "")
        {
            if(LogLevel<=LogLevel.INFO)
            {
                string name = ID == "" ? new StackFrame(1).GetMethod().ReflectedType.Name : ID;
                WriteStuff(FormatingMessage("INFO", name, message));
            }
        }

        public void Warning(string message, string ID = "")
        {
            if(LogLevel<=LogLevel.WARNING)
            {
                string name = ID == "" ? new StackFrame(1).GetMethod().ReflectedType.Name : ID;
                WriteStuff(FormatingMessage("Warning", name, message));
            }
        }

        public void WriteStuff(string content)
        {
            lock(LOCKER)
            {
                logWriter.WriteLine(content);
            }
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

        private string FormatingMessage(string TypeOfMessage, string ComponentName, string Message)
        {
            return "{0} [{1}] ({2}) : {3}".FormateEx(DateTime.Now.ToString("HH:mm:ss.ffff"), TypeOfMessage, ComponentName, Message);
        }
    }
}
