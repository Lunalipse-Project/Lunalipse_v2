using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LunalipseUpdate.Procedures
{
    public class Checking : IProcedure
    {
        public void Main()
        {
            ProcedureHelper.UpdateProgress("等待Lunalipse主程序退出...", -1);
            while(IsProcessRunning("lunalipse"))
            {
                Thread.Sleep(1000);
            }
        }

        private bool IsProcessRunning(string name)
        {
            string path = Environment.CurrentDirectory;
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName.ToLower().Equals(name) && Path.GetDirectoryName(process.MainModule.FileName) == path)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
