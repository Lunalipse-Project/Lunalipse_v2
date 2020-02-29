using Lunalipse.Common.Interfaces.IBehaviorScript;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3
{
    public class LpsInterpreter:IInterpreter
    {
        LpsFrontEnd frontEnd;
        LetterToPrincessLuna toPrincessLuna;

        Stack<ExecutionContext> ExecutionStack;

        Thread ExecutionThread;

        public event Action<RuntimeException> OnRuntimeExceptionThrown;
        public event Action OnProgramComplete;
        public event Action OnInstructionFinished;

        public InterpreterConfig Configuration { get; private set; }

        public LpsInterpreter(InterpreterConfig config)
        {
            frontEnd = new LpsFrontEnd(Configuration.GlobalSymbolTable);
            ExecutionStack = new Stack<ExecutionContext>();
            Configuration = config;

            RegisterAction(LetterActionType.ACT_DO_CHECKLIST, new Action<LetterParagraph>(paragraph =>
            {
                EnterNewContext(paragraph);
            }));

            RegisterSpell("DoTimeTravel", new Action(() =>
            {
                // Did you know that Star Swirl had invented an unknow, brand-new time travel spell ?
                // And it is much powerful than the one that Starlight Glimmer modified.
                // Idea is simple. It pop all current call stack frame from execution stack
                // (Starlight Glimmer only pops couple of them!)
                // Then push a stack frame of main program into it.
                // So we are back to the very initial time far far away before Equestria was founded.
                ExecutionStack.Clear();
                EnterNewContext(toPrincessLuna.MainProgram);
            }));
        }
        public bool IsHalted { get; private set; } = false;
        public bool IsStopped { get; private set; } = true;

        
        public SymbolTable GetProgramSymbolTable
        {
            get
            {
                if (toPrincessLuna == null) return Configuration.GlobalSymbolTable;
                return toPrincessLuna.SymbolTable;
            }
        }

        public void RegisterSymbolAsGlobal(string identifier, LetterValue elementBase)
        {
            Configuration.GlobalSymbolTable.AddSymbol(identifier, elementBase);
        }

        public void RegisterAction(LetterActionType actionType, Delegate actionDelegation)
        {
            LetterStarSwirlSpell starSwirlSpell = new LetterStarSwirlSpell(actionDelegation);
            RegisterSymbolAsGlobal(actionType.ToString(), starSwirlSpell);
        }

        public void RegisterSpell(string identifier, Delegate spell)
        {
            LetterStarSwirlSpell starSwirlSpell = new LetterStarSwirlSpell(spell);
            RegisterSymbolAsGlobal(identifier, starSwirlSpell);
        }

        public void Prepare(string path)
        {
            frontEnd.InitializeLexer(path);
            frontEnd.InitializeParser();
            toPrincessLuna = frontEnd.Parse();
            ExecutionStack.Clear();
        }

        public void StopExecution()
        {
            IsStopped = true;
        }

        public void HaltExecution()
        {
            IsHalted = true;
        }

        public void ResumeExecution()
        {
            IsHalted = false;
        }

        public void Execute()
        {
            if(toPrincessLuna == null)
            {
                //TODO throw : no program to run
                throw new RuntimeException("CORE_LBS_RT_NO_PROGRAM");
            }
            if(ExecutionThread==null || ExecutionThread.ThreadState == ThreadState.Aborted)
            {
                IsStopped = false;
                ExecutionStack.Clear();
                EnterNewContext(toPrincessLuna.MainProgram);
                ExecutionThread = new Thread(new ThreadStart(_executeProgram));
                ExecutionThread.Start();
            }
        }

        private void _executeProgram()
        {
            while (ExecutionStack.Count > 0 && !IsStopped)
            {
                if(!IsHalted)
                {
                    if (ExecutionStack.Count >= Configuration.MaximumContextDepth)
                    {
                        throw new RuntimeException("CORE_LBS_RT_MAX_RECURSION");
                    }
                    ExecutionContext context = ExecutionStack.Peek();
                    if (context.IsEndOfContext())
                    {
                        ExecutionStack.Pop();
                        continue;
                    }
                    try
                    {
                        context.RunNextInstruction();
                    }
                    catch(RuntimeException rte)
                    {
                        OnRuntimeExceptionThrown?.Invoke(rte);
                        if (!Configuration.AutoRuntimeErrorRecovery)
                            break;
                    }
                    OnInstructionFinished?.Invoke();
                }
                Thread.Sleep(1000 / Configuration.InstructionsPreSecond);
            }
            ExecutionStack.Clear();
            OnProgramComplete?.Invoke();
            IsStopped = true;
        }

        public string GetWriter
        {
            get
            {
                return toPrincessLuna == null ? "" : toPrincessLuna.Writer.EvaluateAs<string>();
            }
        }

        public string GetTitle
        {
            get
            {
                return toPrincessLuna == null ? "" : toPrincessLuna.Title.EvaluateAs<string>();
            }
        }

        public int ExecutionStackDepth
        {
            get => ExecutionStack.Count;
        }

        public int CurrentStackPointer
        {
            get
            {
                if (ExecutionStack.Count == 0)
                {
                    return -1;
                }
                return ExecutionStack.Peek().ContextPointer;
            }
        }

        public string CurrentContextIdentifier
        {
            get
            {
                if (ExecutionStack.Count == 0)
                {
                    return string.Empty;
                }
                return ExecutionStack.Peek().ContextIdentifier;
            }
        }

        public InterpreterStatus GetInterpreterStatus
        {
            get
            {
                if (IsStopped)
                {
                    return InterpreterStatus.STOPPED;
                }
                else if(IsHalted)
                {
                    return InterpreterStatus.PAUSED;
                }
                else
                {
                    return InterpreterStatus.RUNNING;
                }
            }
        }

        private void EnterNewContext(LetterParagraph context)
        {
            ExecutionStack.Push(new ExecutionContext(context));
        }
    }
}
