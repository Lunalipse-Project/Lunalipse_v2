using Lunalipse.Common;
using Lunalipse.Common.Interfaces.IConsole;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.Console
{
    public class ConsoleEnvironment : INotifyPropertyChanged
    {
        string command = "";
        string prompt = "";
        int history_ptr = -1;

        public LunaConsole Console { get; set; }
        public IConsoleComponent Env { get; private set; }
        ObservableCollection<string> CommandOutput = new ObservableCollection<string>();
        List<string> HistoryCommand = new List<string>();
        public event PropertyChangedEventHandler PropertyChanged;
        public Action TaskComplete
        {
            get => Console.OnTaskCompleted;
            set
            {
                Console.OnTaskCompleted = value;
            }
        }

        public ConsoleEnvironment(IConsoleComponent Env, string env_name)
        {
            Console = new LunaConsole();
            Console.OnConsoleBufferUpdate += LunaConsole_OnConsoleBufferUpdate;
            Console.OnClearScreen = LunaConsole_OnConsoleScreenClear;
            this.Env = Env;
            prompt = env_name;
            Console.WriteLine("Lunalipse Music Player [Version: {0}]\n(c) 2019 Lunaixsky. All rights reserved.\n\nUse \"help\" for more information.\n", VersionHelper.Instance.getFullVersion());
        }
        public string currentCommand
        {
            get => command;
            set
            {
                command = value;
                HistoryCommand.Add(value);
                history_ptr = HistoryCommand.Count - 1;
                Console.WriteLine("#{0}> {1}", prompt, command);
            }
        }

        public string HistoryNavigateBackward()
        {
            if (history_ptr == 0)
            {
                return HistoryCommand[history_ptr];
            }
            else if(history_ptr < 0)
            {
                return "";
            }
            return HistoryCommand[history_ptr--];
        }

        public string HistoryNavigateForward()
        {
            if (history_ptr + 1 >= HistoryCommand.Count)
            {
                return HistoryCommand[history_ptr];
            }
            else if (history_ptr < 0)
            {
                return "";
            }
            return HistoryCommand[++history_ptr];
        }

        public string GetPromptFormated()
        {
            return string.Format("#{0}>", prompt);
        }

        private void LunaConsole_OnConsoleBufferUpdate(string obj)
        {
            CommandOutput.Add(obj);
        }

        private void LunaConsole_OnConsoleScreenClear()
        {
            CommandOutput.Clear();
        }

        public ObservableCollection<string> Outputs
        {
            get
            {
                return CommandOutput;
            }
            set
            {
                CommandOutput = value;
                OnPropertyChanged("Outputs");
            }
        }

        void OnPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
