using Lunalipse.Common;
using Lunalipse.Common.Data;
using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Core.Console;
using Lunalipse.Core.PlayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Lunalipse.Core.BehaviorScript
{
    public class Interpreter : ComponentHandler , IInterpreter
    {
        static volatile Interpreter INT_INSTANCE;
        static readonly object LOCK_OBJ = new object();

        public static Interpreter INSTANCE(string BasePath)
        {
            if (INT_INSTANCE == null)
            {
                lock (LOCK_OBJ)
                {
                    INT_INSTANCE = INT_INSTANCE ?? new Interpreter(BasePath);
                }
            }
            return INT_INSTANCE;
        }

        public delegate MusicEntity CommandExecutor(int type, object[] args, CataloguePool cp, ref Catalogue chosen, ref int ptr);
        public delegate bool SuffixExecutor(int type, object[] args, ref int Count);

        Parser ScriptParser;
        Interpretation Helper;
        CataloguePool CataPool;
        KeyboardProxy KbProxy;

        [Cachable]
        public List<ActionToken> Actions;
        public string BasePath
        {
            get
            {
                return ScriptParser.RootPath;
            }
            set
            {
                ScriptParser.RootPath = value;
            }
        }
        public bool LBSLoaded { get; private set; }

        event CommandExecutor onCExecutionRequest;
        event SuffixExecutor  onSExecutionRequest;

        int Pointer = 0;
        int singleStepCount = 1;
        bool RandomPlay = false;
        bool Switchable = false;
        MusicEntity cache;
        Catalogue chosenCatalogue;
        Random randomControl;

        protected Interpreter()
        {
            ScriptParser = new Parser();
            KbProxy = KeyboardProxy.INSTANCE;
            CataPool = CataloguePool.INSATNCE;
            Helper = new Interpretation();
            ScriptParser.ErrorOccured += (x, y, z) =>
            {
                ErrorDelegation.OnErrorRaisedBSI?.Invoke(x, y, z);
            };
            ScriptParser.MarcoProcessor = MarcoHandler;

            //Register Lunalipse Behavior Script Instructions
            onCExecutionRequest += InstructionProc.PROC_LUNA_PLAY;
            onCExecutionRequest += InstructionProc.PROC_LUNA_PLAYN;
            onCExecutionRequest += InstructionProc.PROC_LUNA_PLAYC;
            onCExecutionRequest += InstructionProc.PROC_LUNA_NEXT;
            onCExecutionRequest += InstructionProc.PROC_LUNA_LLOOP;
            onCExecutionRequest += InstructionProc.PROC_LUNA_SET;

            //Register Lunalipse Behavior Script Suffix Instructions
            onSExecutionRequest += InstructionProc.PROC_SUFX_COUNT;
            onSExecutionRequest += InstructionProc.PROC_SUFX_RAND;

            //Register Key Press Event
            KbProxy.RegistKeyEvent(new KeyEventProc()
            {
                Name = "CORE_LBS_SWITCHRAND_KEYPRESS",
                ModifierKey = (int)Keys.Alt,
                SubKey = (int)Keys.Z,
                WaitRelease = false,
                ProcInvoke_Down = KeyPressed
            });

            //Register for Lunalipse prompt
            ConsoleAdapter.INSTANCE.RegisterComponent("lbsi", this);
        }
        protected Interpreter(string basePath)
            : this()
        {
            ScriptParser.RootPath = basePath;
        }

        public bool Load(string ScriptID)
        {
            if (!ScriptParser.Load(ScriptID))
            {
                LBSLoaded = false;
                return false;
            }
            return LoadFinal();
        }

        public bool LoadPath(string ScriptPath)
        {
            Switchable = RandomPlay = false;
            if (!ScriptParser.LoadPath(ScriptPath))
            {
                return false;
            }
            return LoadFinal();
        }

        public MusicEntity Stepping()
        {
            try
            {
                if (Pointer >= Actions.Count) return null;
                ActionToken atoken = Actions[Pointer];
                if (singleStepCount == 1)
                {
                    foreach (Delegate delg in onCExecutionRequest.GetInvocationList())
                        if ((cache = ((CommandExecutor)delg).Invoke(atoken.CommandType, atoken.ct_args, CataPool, ref chosenCatalogue, ref Pointer)) 
                                != null)
                            if (((SuffixExecutor)delg).Invoke(atoken.SuffixType, atoken.st_args, ref singleStepCount))
                                break;
                    //foreach (Delegate delg in onSExecutionRequest.GetInvocationList())
                    //    if (((SuffixExecutor)delg).Invoke(atoken.SuffixType, atoken.st_args, ref singleStepCount))
                    //        break;
                    singleStepCount++;
                    if (RandomPlay)
                        Pointer = randomControl.Next(0, Actions.Count);
                    else
                        Pointer++;
                }
                if (cache == null) Stepping();
                else
                {
                    singleStepCount = singleStepCount > 1 ? singleStepCount - 1 : singleStepCount;
                    if (singleStepCount > 1)
                    {
                        return cache;
                    }
                    else if (singleStepCount < 1)
                    {
                        singleStepCount = 1;
                    }
                }
                //If no LLOOP defined and pointer comes to last.
                //Execution completed.
                if (Pointer >= Actions.Count)
                {
                    LBSLoaded = false;
                    Actions.Clear();
                    return null;
                }
                return cache;
            }
            catch (StackOverflowException)
            {
                ErrorDelegation.OnErrorRaisedBSI?.Invoke("CORE_LBS_InvalidCommandExecutor", null, Pointer);
                return null;
            }
        }

        public override bool OnCommand(params string[] args)
        {
            return base.OnCommand(args);
        }

        public bool SaveAs(string path)
        {
            //GeneralExporter<Interpreter> ge = new GeneralExporter<Interpreter>(path);
            //return ge.Export(this, "Actions");
            throw new NotImplementedException();
        }

        private bool LoadFinal()
        {
            Pointer = 0;
            singleStepCount = 1;
            if (!ScriptParser.Parse())
            {
                return LBSLoaded = false;
            }
            Actions = Helper.Interpret(ScriptParser.Tokens);
            if (!Actions.All(x => x != null)) return false;
            //Notify the mainframe that the script is ready to execute
            LpsAudio.AudioDelegations.PlayingFinished?.Invoke();
            randomControl = new Random();
            return LBSLoaded = true;
        }

        private void MarcoHandler(PRAGMA p, string[] args)
        {
            switch(p)
            {
                case PRAGMA.MOD_LINEAR:
                    RandomPlay = false;
                    break;
                case PRAGMA.MOD_RANDOM:
                    RandomPlay = true;
                    break;
                case PRAGMA.MOD_SWITCH:
                    Switchable = true;
                    break;
            }
        }

        private void KeyPressed()
        {
            if(Switchable)
            {
                if (RandomPlay) RandomPlay = false;
                else RandomPlay = true;
            }
        }
    }
}
