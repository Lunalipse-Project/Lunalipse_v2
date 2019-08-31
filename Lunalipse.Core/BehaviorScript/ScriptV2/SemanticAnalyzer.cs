using System;
using System.Collections.Generic;
using System.Text;

namespace Lunalipse.Core.BehaviorScript.ScriptV2
{
    public class SemanticAnalyzer
    {
        LexicalLibs lexicalLibs = LexicalLibs.Instance;
        public void CheckSemanticAvailability(ref List<CodeBlock> codeBlocks)
        {
            for (int i = 0; i < codeBlocks.Count; i++)
            {
                if (codeBlocks[i].repeatTimes != null)
                {
                    if(CheckFunction((Function)codeBlocks[i].repeatTimes) != ReturnTypes.Int)
                    {
                        continue;
                    }
                }
                int fcount = codeBlocks[i].functions.Count;
                for (int j = 0; j < fcount; j++)
                {
                    if (!codeBlocks[i].functions[j].isFunction)
                    {
                        codeBlocks[i].functions.RemoveAt(j);
                        fcount--;
                    }
                    else
                    {
                        if (CheckFunction(codeBlocks[i].functions[j]) != ReturnTypes.Void)
                        {
                            codeBlocks[i].functions.RemoveAt(j);
                            fcount--;
                        }
                    }
                }
                if (fcount == 0)
                {
                    codeBlocks.RemoveAt(i);
                }
            }
        }

        bool CheckParams(Parameter parameter, Parameter Accepted)
        {
            return parameter.data_type == Accepted.data_type;
        }
        ReturnTypes CheckFunction(Function function)
        {
            int ith_para = 0;
            if (!function.isFunction)
            {
                return function.data_type;
            }
            Function DefinedFunction = lexicalLibs.GetFunction(function.functionName);
            if (DefinedFunction == null)
            {
                throw new ScriptException("CORE_BSCRIPTV2_ERROR_FUNCNDEF", ScriptExceptionType.SEMANTIC, function.functionName);
            }
            if (DefinedFunction.paras.Count < function.paras.Count)
            {
                throw new ScriptException("CORE_BSCRIPTV2_ERROR_MOREARGS", ScriptExceptionType.SEMANTIC, 
                    function.functionName, DefinedFunction.paras.Count, function.paras.Count);
            }
            if (DefinedFunction.paras.Count != function.paras.Count && function.paras.Count == 0)
            {
                if(DefinedFunction.paras.Count>=1 && !DefinedFunction.paras[0].ignorable)
                {
                    throw new ScriptException("CORE_BSCRIPTV2_ERROR_FEWARGS", ScriptExceptionType.SEMANTIC);
                }
            }
            if (DefinedFunction.functionProc == null && DefinedFunction.functionType != FunctionType.PLACEHOLDER)
            {
                throw new ScriptException("CORE_BSCRIPTV2_ERROR_DELEG_NDEF", ScriptExceptionType.SEMANTIC, DefinedFunction.functionName);
            }
            function.data_type = DefinedFunction.data_type;
            function.functionType = DefinedFunction.functionType;
            function.functionProc = DefinedFunction.functionProc;
            if(DefinedFunction.functionType != FunctionType.PLACEHOLDER)
            {
                foreach (Parameter parameter in function.paras)
                {
                    Parameter DesireParameter = DefinedFunction.paras[ith_para];
                    if (parameter.isFunction)
                    {
                        Function function_para = (Function)parameter;
                        if (CheckFunction(function_para) != DesireParameter.data_type && function_para.functionType != FunctionType.PLACEHOLDER)
                        {
                            throw new ScriptException("CORE_BSCRIPTV2_ERROR_TYPEERR", ScriptExceptionType.SEMANTIC,
                                ith_para + 1, parameter.data_type.ToString(), DefinedFunction.functionName);
                        }
                    }
                    else
                    {
                        if (!CheckParams(parameter, DesireParameter))
                        {
                            throw new ScriptException("CORE_BSCRIPTV2_ERROR_TYPEERR", ScriptExceptionType.SEMANTIC,
                                ith_para + 1, parameter.data_type.ToString(), DefinedFunction.functionName);
                        }
                    }
                    ith_para++;
                }
            }
            return function.data_type;
        }
    }
}
