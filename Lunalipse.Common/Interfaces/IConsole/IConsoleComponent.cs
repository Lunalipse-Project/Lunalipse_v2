using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IConsole
{
    public interface IConsoleComponent
    {
        void OnEnvironmentLoaded(ILunaConsole console);
        bool OnCommand(ILunaConsole console, params string[] args);
        ICommandRegistry GetCommandRegistry();
        string GetContextDescription();
    }
}
