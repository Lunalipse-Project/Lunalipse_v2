using Lunalipse.Utilities;
using Lunalipse.Common.Interfaces.IConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.Console
{
    public class ConsoleAdapter : IConsoleAdapter
    {
        static volatile ConsoleAdapter CA_INSTANCE;
        static readonly object CA_LOCK = new object();

        public static ConsoleAdapter Instance
        {
            get
            {
                if (CA_INSTANCE == null)
                {
                    lock (CA_LOCK)
                    {
                        CA_INSTANCE = CA_INSTANCE ?? new ConsoleAdapter();
                    }
                }
                return CA_INSTANCE;
            }
        }

        private Dictionary<string, ComponentHandler> Handler;
        private ConsoleAdapter() { Handler = new Dictionary<string, ComponentHandler>(); }

        public bool InvokeCommand(string cmd, params string[] args)
        {
            return Handler[cmd].OnCommand(args);
        }

        public bool RegisterComponent(string component, ComponentHandler CH)
        {
            return Handler.AddNonRepeat(component, CH);
        }

        public bool UnregisterComponent(string component)
        {
            return Handler.Remove(component);
        }

        public ComponentHandler getComponent(string component)
        {
            if (Handler.ContainsKey(component))
                return null;
            return Handler[component];
        }
    }
}
