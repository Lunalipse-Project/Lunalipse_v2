
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IConsole
{
    public interface IConsoleAdapter
    {
        bool RegisterComponent(string component, IConsoleComponent CH);
        bool UnregisterComponent(string component);
        IConsoleComponent getComponent(string component);
        void InvokeCommand(string cmd, params string[] args);
    }
}
