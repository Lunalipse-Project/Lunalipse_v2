using Lunalipse.Common.Interfaces.IConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.Console
{
    public class LunaConsole : ILunaConsole
    {
        public event Action<string> OnConsoleBufferUpdate;

        /// <summary>
        /// Current task is complete, wait for next command input
        /// </summary>
        public Action OnTaskCompleted;
        public Action OnClearScreen;

        public string CurrentEnvironment { get; set; }
        public void WriteLine(string fmt, params string[] args)
        {
            OnConsoleBufferUpdate?.Invoke(string.Format(fmt, args));
        }

        public void FinishTask()
        {
            OnTaskCompleted?.Invoke();
        }

        public void ClearScreen()
        {
            OnClearScreen?.Invoke();
        }
    }
}
