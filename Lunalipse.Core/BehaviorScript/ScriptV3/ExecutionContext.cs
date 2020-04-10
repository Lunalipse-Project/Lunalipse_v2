using Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements;

namespace Lunalipse.Core.BehaviorScript.ScriptV3
{
    public class ExecutionContext
    {
        public LetterParagraph Context { get; private set; }
        public int ContextPointer { get; set; }
        public ContextType Type { get; }

        public string ContextIdentifier
        {
            get
            {
                if (Context.ElementTokenInfo == null)
                {
                    return string.Empty;
                }
                return Context.ElementTokenInfo.TokenText;
            }
        }

        public int LoopCount { get; private set; } = 1;

        public ExecutionContext(LetterParagraph context, ContextType type)
        {
            Context = context;
            ContextPointer = -1;
            Type = type;
        }

        public bool IsEndOfContext()
        {
            return ContextPointer >= Context.GetStatementsCount();
        }

        public LetterValue getCurrentContextStatement()
        {
            return Context.GetStatementAt(ContextPointer);
        }

        public void RunNextInstruction(LpsInterpreter interpreter)
        {
            if(LoopCount > 1)
            {
                ExecuteStatement(interpreter);
                LoopCount--;
                return;
            }
            ContextPointer++;
            if(!IsEndOfContext())
            {
                LoopCount = ExecuteStatement(interpreter);
            }
            else if(Type == ContextType.Loop)
            {
                ContextPointer = -1;
                LoopCount = 1;
            }
        }

        private int ExecuteStatement(LpsInterpreter interpreter)
        {
            int loopCount = 1;
            LetterValue letterElement = this.getCurrentContextStatement();
            if (letterElement.GetLetterElementType() == ElementType.FUNCTION_CALL)
            {
                LetterFunctionCall functionCall = letterElement as LetterFunctionCall;
                foreach (SuffixAction action in functionCall.GetSuffixActions().GetSuffixActions())
                {
                    switch (action.GetSuffixType)
                    {
                        case LetterActionType.ACT_REPEAT_TIME:
                            loopCount = action.GetSuffixActionParams.EvaluateAs<int>();
                            break;
                        default:
                            ExecuteAction(action);
                            break;
                    }
                }
            }
            else if(letterElement.GetLetterElementType() == ElementType.LOOP)
            {
                LetterLoop loop = letterElement as LetterLoop;
                interpreter.EnterNewContext(loop.Paragraph, ContextType.Loop);
            }
            else if(letterElement.GetLetterElementType() == ElementType.IF_ELSE)
            {
                LetterParagraph context = (letterElement as LetterIf).Decision();
                if(context != null)
                {
                    interpreter.EnterNewContext(context, ContextType.GAMMY);
                }
            }
            letterElement.Evaluate();
            return loopCount;
        }

        private void ExecuteAction(SuffixAction action)
        {
            switch (action.GetSuffixType)
            {
                case LetterActionType.ACT_SET_VOL:
                    double volume = action.GetSuffixActionParams.EvaluateAs<double>();
                    //TODO change volume
                    break;
            }
        }
    }

    public enum ContextType
    {
        TwilightChecklist,
        Loop,
        Main,
        GAMMY
    }
}
