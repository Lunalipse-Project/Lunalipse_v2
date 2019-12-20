using Lunalipse.Common.Interfaces.IConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Data
{
    public class ConsoleCommand
    {
        public string HelperText { get; private set; }
        public string Command { get; private set; }
        public string CommandDesc { get; private set; }
        public Func<ILunaConsole, string[], ConsoleCommand,bool> Handler { get; private set; }
        public ConsoleCommand(string Command, string HelperText, string CommandDesc, Func<ILunaConsole, string[], ConsoleCommand, bool> Handler)
        {
            this.Handler = Handler;
            this.Command = Command;
            this.CommandDesc = CommandDesc;
            this.HelperText = HelperText;
        }
    }
}
