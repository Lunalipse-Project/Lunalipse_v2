using Lunalipse.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IConsole
{
    public interface ICommandRegistry
    {
        void registerCommand(ConsoleCommand consoleCommand);
        bool TryInvokeCommand(string[] parsedCommand, ILunaConsole currentConsole);
        ConsoleCommand GetCommand(string command);
        string GetAllCommandsAndDescs();

    }
}
