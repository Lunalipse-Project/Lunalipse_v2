using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.Console
{
    public class CommandRegistry : ICommandRegistry
    {
        Dictionary<string, ConsoleCommand> cmdRegistry = new Dictionary<string, ConsoleCommand>();
        public CommandRegistry()
        {

        }

        public void registerCommand(ConsoleCommand consoleCommand)
        {
            cmdRegistry.AddNonRepeat(consoleCommand.Command, consoleCommand);
        }

        public bool TryInvokeCommand(string[] parsedCommand, ILunaConsole currentConsole)
        {
            string command = parsedCommand[0];
            string[] args = parsedCommand.Skip(1).ToArray();
            ConsoleCommand consoleCommand;
            if ((consoleCommand = GetCommand(command)) == null)
            {
                currentConsole.WriteLine("Error. Command '{0}' not found in current context.", command);
                return false;
            }
            if(consoleCommand.Handler == null)
            {
                return false;
            }
            return consoleCommand.Handler.Invoke(currentConsole, args, consoleCommand);
        }

        public ConsoleCommand GetCommand(string command)
        {
            if (!cmdRegistry.ContainsKey(command))
            {
                return null;
            }
            return cmdRegistry[command];
        }

        public string GetAllCommandsAndDescs()
        {
            string str = "";
            foreach(var kvpair in cmdRegistry)
            {
                str += $"\t{kvpair.Key}      {kvpair.Value.CommandDesc}\n";
            }
            return str;
        }
    }
}
