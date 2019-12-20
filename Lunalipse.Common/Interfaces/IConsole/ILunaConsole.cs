using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IConsole
{
    public interface ILunaConsole
    {
        void WriteLine(string fmt, params string[] args);
        void FinishTask();
        void ClearScreen();
    }
}
