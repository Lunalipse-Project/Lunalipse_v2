using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Core.PlayList;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lunalipse.Core.BehaviorScript.ScriptV2
{
    public class Executor
    {
        LexicalLibs lexicalLibs = LexicalLibs.Instance;
        MusicEntity musicEntity;
        List<CodeBlock> codeBlocks;
        ICatalogue catalogue;
        public MusicEntity CurrentMusicEntity { get => musicEntity; }
        public ICatalogue CurrentCatalogue { get => catalogue; }
        public CataloguePool CataloguePool { get; set; }

        public List<CodeBlock> CodeBlocks
        {
            get => codeBlocks;
            set
            {
                codeBlocks = value;
                currentPtr = 0;
                currentInnerPtr = 0;
                blockLoopTarget = 0;
            }
        }

        public string CurrentCode { get; private set; }

        public string CurrentCodeParsed { get; private set; }

        public int currentPointer => currentPtr;

        public int currentInnerPointer => currentInnerPtr;

        public int currentPtr = 0;
        public int currentInnerPtr = 0, blockLoopTarget = 0;

        public event Action OnInstructionFinished;

        public void ResetExecutor()
        {
            currentInnerPtr = 0;
            currentPtr = 0;
            catalogue = CataloguePool.All[0];
            musicEntity = null;
        }

        public void forcedStepping()
        {
            currentPtr++;
            currentInnerPtr = 0;
            blockLoopTarget = 0;
            ExecuteNextBlock();
        }

        public bool ExecuteNextBlock()
        {
            if (CodeBlocks == null) return true;
            if (CodeBlocks.Count < currentPtr)
            {
                musicEntity = null;
                return true;
            }
            CodeBlock codeBlock = CodeBlocks[currentPtr];
            if (blockLoopTarget < 1)
            {
                if (codeBlock.repeatTimes != null)
                {
                    blockLoopTarget = (int)FuncExe((Function)codeBlock.repeatTimes);
                }
                else
                {
                    blockLoopTarget = 0;
                }
            }
            if (BlockExe(codeBlock))
            {
                currentPtr++;
                blockLoopTarget--;
                ExecuteNextBlock();
            }
            return false;
        }

        private bool BlockExe(CodeBlock codeBlock)
        {
            if (currentInnerPtr < codeBlock.functions.Count)
            {
                Function fx = codeBlock.functions[currentInnerPtr];
                FuncExe(fx);
                CurrentCode = fx.ToExpressionString();
                CurrentCodeParsed = fx.ToParsedFuncString();
                currentInnerPtr++;
                OnInstructionFinished?.Invoke();
                if ((fx.functionType & FunctionType.NON_PLAYABLE_FUNCTION) == FunctionType.NON_PLAYABLE_FUNCTION)
                {
                    return BlockExe(codeBlock);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (blockLoopTarget > 1)
                {
                    currentInnerPtr = 0;
                    blockLoopTarget--;
                    return BlockExe(codeBlock);
                }
                else
                {
                    currentInnerPtr = 0;
                    return true;    //Can go next
                }
            }
        }

        private object FuncExe(Function function)
        {
            if (function.functionType == FunctionType.PLACEHOLDER)
            {
                return null;
            }
            if (function.isFunction)
            {
                foreach (Parameter parameter in function.paras)
                {
                    if (parameter.isFunction)
                    {
                        parameter.p_value = FuncExe((Function)parameter);
                    }
                }
                return function.functionProc.Invoke(function, ref catalogue, ref currentPtr, ref musicEntity);
            }
            else
            {
                return function.p_value;
            }
        }
    }
}