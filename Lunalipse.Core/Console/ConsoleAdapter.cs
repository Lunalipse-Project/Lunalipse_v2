using Lunalipse.Utilities;
using Lunalipse.Utilities.Misc;
using Lunalipse.Common.Interfaces.IConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunalipse.Common.Data;

namespace Lunalipse.Core.Console
{
    public class ConsoleAdapter : IConsoleAdapter
    {
        public event Action<ConsoleEnvironment> OnEnvironmentChanged;

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

        public void RequireMother()
        {
            CurrentEnv = MotherEnvironment;
            LunaConsole = MotherEnvironment.Console;
            OnEnvironmentChanged?.Invoke(CurrentEnv);
        }

        private Dictionary<string, IConsoleComponent> Environments;
        private List<Tuple<string,ConsoleEnvironment>> LoadedEnvironments;
        private int env_pointer = -1;
        public ILunaConsole LunaConsole { get; set; }
        ConsoleEnvironment MotherEnvironment;
        private ConsoleEnvironment CurrentEnv { get; set; } = null;
        private CommandRegistry commandRegistry;

        const string DescTextCtx = "Manage the current console context.";
        const string HelperTextCtx = "Manage the current console context.\n" +
                                      "This command is derived from mother context and avaliable in any other child context\n" +
                                      "Usage:\n" +
                                        "\tctx [OPTIONS] [CTXNAME] \n" +
                                        "\t\tCTXNAME               Name of the (child) context which you wish to eneter.\n\n" +
                                        "\t\tOPTIONS defined as follow:\n" +
                                        "\t\t-s, --switch           Switch to ENV_NAME\n" +
                                        "\t\t-n, --new              Create ENV_NAME and enter it\n" +
                                        "\t\t-l, --list             List all avaliable (child) context\n" +
                                        "\t\t-lld, --listLoaded     List all loaded (child) context\n" +
                                        "\t\t-q, --quit             Dispose all opened (child) context and back to mother context\n" +
                                        "\t\t-b, --back             Navigate to previous (child) context";
        const string DescTextrfInvoke = "Manage functions in current context";
        const string HelperTextrfInvoke = "Reflection Call.\n" +
                                          "Manage functions in current context\n" +
                                            "Usage:\n" +
                                            "\trfcall [OPTIONS] [-f, --function FUNC_NAME] [-p, --paras (ARGS,..)] [CTXNAME]\n\n" +
                                            "\tFUNC_NAME            Name of calling function\n" +
                                            "\tCTXNAME              Name of context, use when '-ctx, --context' is specified\n" +
                                            "\t(ARGS,..)            Array of arguments in form (arg1,arg2,...) for function\n\n" +
                                            "\t\tOPTIONS:\n" +
                                            "\t\t-l, --list         List all functions availiable\n" +
                                            "\t\t-h, --help         Print this information\n" +
                                            "\t\t-f, --function     Name for callee.\n" +
                                            "\t\t-p, --paras        Arguments for callee\n" +
                                            "\t\t-ctx, --context    Specify a context. If no context is specified, then use the current.";
        const string DescTextClear = "Clear the screen and buffer.";
        const string HelperTextClear = "Clear the screen and anything in the output buffer.\n" +
                                       "Usage:\n" +
                                       "\t clear [-h,--help]\n\n" +
                                       "\t\t-h,--help         Diplay this message.";
        const string DescTextHelp = "Help you command in any context.";
        const string HelperTextHelp = "This command allow you to see the detailed usage of any command in any context.\n" +
                                        "Usage:\n" +
                                        "\thelp [-h, --help] [-n, --name COMMAND] [-ctx, --context CTXNAME]\n\n" +
                                        "\t-n, --name         Specify a command you interested." +
                                        "\tCOMMAND            Name of the command, use together with '-n, --name'\n" +
                                        "\t-ctx, --context    Specify a context which COMMAND located.\n" +
                                        "\tCTXNAME            Name of the context, use together with '-ctx, --context'.\n" +
                                        "\t-h, --help         Display this message.";

        private ConsoleAdapter()
        {
            Environments = new Dictionary<string, IConsoleComponent>();
            LoadedEnvironments = new List<Tuple<string, ConsoleEnvironment>>();
            MotherEnvironment = new ConsoleEnvironment(null, "LpsConsole");
            commandRegistry = new CommandRegistry();
            commandRegistry.registerCommand(new ConsoleCommand("ctx", HelperTextCtx, DescTextCtx, CommandEnvInvoke));
            commandRegistry.registerCommand(new ConsoleCommand("rfcall", HelperTextrfInvoke,DescTextrfInvoke, CommandrfInvoke));
            commandRegistry.registerCommand(new ConsoleCommand("clear", HelperTextClear,DescTextClear, CommandClearInvoke));
            commandRegistry.registerCommand(new ConsoleCommand("help", HelperTextHelp, DescTextHelp, CommandHelpInvoke));
        }



        public void InvokeCommand(string cmd, params string[] args)
        {
            Environments[cmd].OnCommand(LunaConsole, args);
        }

        public bool RegisterComponent(string component, IConsoleComponent CH)
        {
            return Environments.AddNonRepeat(component, CH);
        }

        public bool UnregisterComponent(string component)
        {
            return Environments.Remove(component);
        }

        public IConsoleComponent getComponent(string component)
        {
            if (!Environments.ContainsKey(component))
                return null;
            return Environments[component];
        }


        public void PrintAllComponent()
        {
            string hint = "";
            foreach(var kvpair in Environments)
            {
                hint += "\t" + kvpair.Key + "\n";
            }
            LunaConsole.WriteLine(hint);
        }

        public void runCommand(string command)
        {
            try
            {
                if (!LpsCommandParser.CheckQuote(command))
                {
                    // Invalid Command: Quotes don't match
                    LunaConsole.WriteLine("Invalid Command: quotes don't match");
                    return;
                }
                CurrentEnv.currentCommand = command;
                string[] cmds = LpsCommandParser.ParseCommand(command);
                if (cmds.Length > 0)
                {
                    bool has_executed = false;
                    if (!(has_executed = commandRegistry.TryInvokeCommand(cmds, LunaConsole)))
                    {
                        if (CurrentEnv != null && CurrentEnv.Env != null)
                        {
                            has_executed = CurrentEnv.Env.OnCommand(LunaConsole, cmds);
                        }
                    }
                    if (!has_executed)
                    {
                        LunaConsole.WriteLine("Error. Fail to execute command '{0}'", cmds[0]);
                    }
                }
            }
            catch(Exception e)
            {
                LunaConsole.WriteLine("Something went wrong!\nDon't worry, Lunalipse has got the error message:\n\t{0}", e.Message);
            }
            LunaConsole.FinishTask();
        }

        private bool CommandEnvInvoke(ILunaConsole console, string[] args, ConsoleCommand command)
        {
            if (args.Length == 0)
            {
                LunaConsole.WriteLine("{0}: Missing operand.\nTry \"{0} --help\" for more information.", command.Command);
                return true;
            }
            if (args[0] == "-h" || args[0] == "--help")
            {
                LunaConsole.WriteLine(command.HelperText);
            }
            else if (args[0] == "-l" || args[0] == "--list")
            {
                LunaConsole.WriteLine("The following evironments are loaded by Lunalipse.");
                PrintAllComponent();
            }
            else if (args[0] == "-q" || args[0] == "--quit")
            {
                QuitEnvironment();
            }
            else if (args[0] == "-b" || args[0] == "--back")
            {
                NavigatePrevious();
            }
            else if (args[0] == "-lld" || args[0] == "--listLoaded")
            {
                string hint = "";
                foreach (var tups in LoadedEnvironments)
                {
                    hint += "\t" + tups.Item1 + "\n";
                }
                LunaConsole.WriteLine(hint);
            }
            else if (args[0] == "-s" || args[0] == "--switch")
            {
                SwitchToEnv(args[1]);
            }
            else if (args[0] == "-n" || args[0] == "--new")
            {
                LoadEnvironment(args[1]);
            }
            else
            {
                console.WriteLine("Unrecognized option: {0}", args[0]);
            }
            return true;
        }

        private bool CommandHelpInvoke(ILunaConsole currentConsole, string[] args, ConsoleCommand command)
        {
            string cmdname = "", context = "";
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-n":
                    case "--name":
                        cmdname = args[++i];
                        break;
                    case "-ctx":
                    case "--context":
                        context = args[++i];
                        break;
                    case "-h":
                    case "--help":
                        currentConsole.WriteLine(command.HelperText);
                        return true;
                    default:
                        currentConsole.WriteLine("Unrecognized option: {0}", args[i]);
                        break;
                }
            }
            object ctx = setContext(ref context, currentConsole);
            string description = "This is Mother Context that host all context and console activites.";
            ICommandRegistry cmdr = commandRegistry;
            if (ctx.GetType().GetInterface("IConsoleComponent") != null)
            {
                cmdr = ((IConsoleComponent)ctx).GetCommandRegistry();
                description = ((IConsoleComponent)ctx).GetContextDescription();
            }
            if(cmdname != "")
            {
                ConsoleCommand consoleCommand = cmdr.GetCommand(cmdname);
                if (consoleCommand == null)
                {
                    currentConsole.WriteLine("Error. Command '{0}' do not exist in the context.");
                }
                else
                {
                    currentConsole.WriteLine(consoleCommand.HelperText);
                }
            }
            else
            {
                currentConsole.WriteLine("{0} [{1}]", context == "" ? "LpsConsole" : ctx.GetType().Name, ctx.GetType().ToString());
                currentConsole.WriteLine(description);
                currentConsole.WriteLine("Commands:");
                currentConsole.WriteLine(cmdr.GetAllCommandsAndDescs());
            }
            return true;
        }

        private bool CommandrfInvoke(ILunaConsole console, string[] args, ConsoleCommand command)
        {
            string func_name = "", func_args = "", context = "";
            bool requireListing = false;
            if (args.Length == 0)
            {
                LunaConsole.WriteLine("{0}: Missing operand.\nTry \"{0} --help\" for more information.", command.Command);
                return true;
            }
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-l":
                    case "--list":
                        requireListing = true;
                        break;
                    case "-h":
                    case "--help":
                        console.WriteLine(command.HelperText);
                        return true;
                    case "-f":
                    case "--function":
                        func_name = args[++i];
                        break;
                    case "-p":
                    case "--paras":
                        func_args = args[++i];
                        break;
                    case "-ctx":
                    case "--context":
                        context = args[++i];
                        break;
                    default:
                        console.WriteLine("Unrecognized option: {0}", args[i]);
                        break;
                }
            }
            object ctx = setContext(ref context, console);
            if (requireListing)
            {
                string listFunc = ReflectionHelper.GetFormatedCommandList(ctx.GetType(), true, typeof(AttrConsoleSupportable));
                if (string.IsNullOrEmpty(listFunc))
                {
                    console.WriteLine("There are no function avaliable under {0}.",context=="" ? "current context": $"context '{context}'");
                }
                else
                {
                    console.WriteLine(listFunc);
                }
                return true;
            }
            if (func_name != "" || func_args != "")
            {
                string[] parsedArgs = LpsCommandParser.ParseCommand(func_args, ',');
                var returnVal = ReflectionHelper.InvokeMethod(ctx.GetType(), ctx, func_name, parsedArgs);
                if (returnVal.Item1)
                {
                    if (returnVal.Item2 != null)
                    {
                        console.WriteLine("Function invoked successfully, value returned:");
                        console.WriteLine(returnVal.Item2.ToString());
                    }
                    else
                    {
                        console.WriteLine("Function invoked successfully, no value returned.");
                    }
                }
                else
                {
                    console.WriteLine("Error. Fail to invoke. Function not exist or arguments do not match.");
                }
                return true;
            }
            return false;
        }

        private bool CommandClearInvoke(ILunaConsole currentConsole, string[] args, ConsoleCommand ccd)
        {
            if (args.Length == 0)
            {
                currentConsole.ClearScreen();
            }
            else
            {
                if(args[0]=="-h" || args[0]=="--help")
                {
                    currentConsole.WriteLine(ccd.HelperText);
                }
                else
                {
                    currentConsole.WriteLine("Unrecognized option: {0}", args[0]);
                }
            }
            return true;
        }

        private object setContext(ref string context, ILunaConsole console)
        {
            object ctx = CurrentEnv.Env;
            if (context != "")
            {
                ctx = getComponent(context);
                if (ctx == null)
                {
                    console.WriteLine("Context '{0}' do not exist, using current one.", context);
                    context = "";
                    ctx = CurrentEnv.Env;
                }
            }
            if (ctx == null)
            {
                ctx = this;
            }
            return ctx;
        }
        private void LoadEnvironment(string env)
        {
            IConsoleComponent consoleComponent = getComponent(env);
            if (consoleComponent == null)
            {
                LunaConsole.WriteLine("Error. Context '{0}' not loaded by lunalipse.", env);
                return;
            }
            CurrentEnv = new ConsoleEnvironment(consoleComponent, env);
            LunaConsole = CurrentEnv.Console;
            LoadedEnvironments.Add(new Tuple<string, ConsoleEnvironment>(env, CurrentEnv));
            env_pointer++;
            CurrentEnv.Env?.OnEnvironmentLoaded(LunaConsole);
            OnEnvironmentChanged?.Invoke(CurrentEnv);
        }

        private void SwitchToEnv(string env)
        {
            int index = LoadedEnvironments.FindIndex(x => x.Item1 == env);
            if (index >= 0)
            {
                CurrentEnv = LoadedEnvironments[index].Item2;
                LunaConsole = CurrentEnv.Console;
                env_pointer = index;
                OnEnvironmentChanged?.Invoke(CurrentEnv);
            }
            else
            {
                LunaConsole.WriteLine("Error. Context '{0}' not activate.", env);
            }
        }

        private void NavigatePrevious()
        {
            if (env_pointer == 0)
            {
                LunaConsole.WriteLine("Error. No context before the first environment.");
                return;
            }
            else if (env_pointer < 0)
            {
                LunaConsole.WriteLine("Error. No context is loaded.");
                return;
            }
            env_pointer--;
            CurrentEnv = LoadedEnvironments[env_pointer].Item2;
            LunaConsole = CurrentEnv.Console;
            OnEnvironmentChanged?.Invoke(CurrentEnv);
        }

        private void QuitEnvironment()
        {
            if (LoadedEnvironments.Count == 0)
            {
                LunaConsole.WriteLine("Error. You can't quit the mother context.");
                return;
            }
            LoadedEnvironments.RemoveAt(env_pointer);
            env_pointer = LoadedEnvironments.Count - 1;
            CurrentEnv = env_pointer < 0 ? MotherEnvironment : LoadedEnvironments[env_pointer].Item2;
            LunaConsole = CurrentEnv.Console;
            OnEnvironmentChanged?.Invoke(CurrentEnv);
        }

    }
}
