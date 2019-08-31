using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Lunalipse.Core.BehaviorScript.ScriptV2
{
    public class LexicalAnalyzer
    {
        Regex ExtractBlock = new Regex(@"((?<=[;|\n]\().+?(?=\)\=\>))|(?<=(\=\>))\{.*?\}", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        Regex LambdaBlock = new Regex(@"\=\>\{.*?\}", RegexOptions.Compiled | RegexOptions.Singleline);
        Regex LambdaNotation = new Regex(@"\=\>", RegexOptions.Compiled | RegexOptions.Singleline);
        Regex LambdaLeading = new Regex(@"(?<=[;|\n]\().+?(?=\)\=\>\{)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        Regex ExtractFunction = new Regex(@"^(.+?(?=\())", RegexOptions.Compiled);
        Regex ExtractInner = new Regex(@"(?<=\().+(?=\))", RegexOptions.Compiled);
        Regex ExtractParas = new Regex(@"([^,]+\(.+?\)(?=,))|([^,]+)", RegexOptions.Compiled);
        Regex MLCommentRemoval = new Regex(@"\/\*.*?\*\/", RegexOptions.Singleline | RegexOptions.Compiled);
        Regex SLCommentRemoval = new Regex(@"\/\/.*", RegexOptions.Compiled);

        public List<CodeBlock> ParseScript(string path)
        {
            List<CodeBlock> codeBlocks = new List<CodeBlock>();
            foreach (ScriptStructure scriptStructure in structuralize(readScript(path)))
            {
                codeBlocks.Add(GenerateCodeBlock(scriptStructure));
            }
            return codeBlocks;
        }

        private string readScript(string path)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = SLCommentRemoval.Replace(line, string.Empty);
                    line = MLCommentRemoval.Replace(line, string.Empty);
                    if(line!=string.Empty)
                    {
                        stringBuilder.AppendLine(line);
                    }
                }
            }
            string finalize = stringBuilder.ToString();
            finalize = MLCommentRemoval.Replace(finalize, string.Empty);
            return finalize.Trim();
        }

        private List<ScriptStructure> structuralize(string script)
        {
            List<ScriptStructure> structures = new List<ScriptStructure>();
            int lambda_counts = LambdaNotation.Matches(script).Count;
            int lambda_n_counts = LambdaBlock.Matches(script).Count;
            int lambda_leading = LambdaLeading.Matches(script).Count;
            if (lambda_counts != lambda_n_counts)
            {
                // Exception
                // A code block is defined in incompleted form
                throw new ScriptException("CORE_BSCRIPTV2_ERROR_LEXINCOMCB", ScriptExceptionType.LEXICAL);
            }
            if (lambda_counts != lambda_leading)
            {
                // Exception
                // Invalid lambda leading, it should be number or function with numeric return value
                throw new ScriptException("CORE_BSCRIPTV2_ERROR_LEXINCOMCB", ScriptExceptionType.LEXICAL);
            }
            MatchCollection matches = ExtractBlock.Matches(script);
            for (int i = 0; i < matches.Count-1;)
            {
                ScriptStructure scriptStructure = new ScriptStructure();
                scriptStructure.leading = matches[i].Value;
                scriptStructure.body = matches[i + 1].Value.Trim('{', '}');
                structures.Add(scriptStructure);
                i += 2;
            }
            script = ExtractBlock.Replace(script, string.Empty);
            string[] lines = script.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != "()=>\r" && lines[i] != "()=>")
                {
                    structures.Insert(i, new ScriptStructure()
                    {
                        body = lines[i].Trim('\r', '\n')
                    });
                }
            }
            return structures;
        }

        private CodeBlock GenerateCodeBlock(ScriptStructure scriptStructure)
        {
            CodeBlock codeBlock = new CodeBlock();
            List<Function> functions = new List<Function>();
            if (scriptStructure.leading != null)
            {
                codeBlock.repeatTimes = ParseFunction(scriptStructure.leading);
            }
            foreach(string line in scriptStructure.body.Split(';'))
            {
                string function = line.Trim('\r', '\n', '\t', ' ');
                if (string.IsNullOrEmpty(function))
                {
                    continue;
                }
                functions.Add(ParseFunction(function));
            }
            codeBlock.functions = functions;
            return codeBlock;
        }

        private Function ParseFunction(string function)
        {
            Function fx = new Function();
            if (!ExtractFunction.IsMatch(function))
            {
                fx.isFunction = false;
                fx.data_type = LexicalUtils.getType(ref function);
                fx.p_value = LexicalUtils.ConvertTo(fx.data_type, function);
            }
            else
            {
                fx.isFunction = true;
                fx.functionName = ExtractFunction.Match(function).Value.Trim();
                fx.data_type = ReturnTypes.UNCERTAIN;
                function = function.Remove(0, fx.functionName.Length);
                Match match = ExtractInner.Match(function);
                if(match != Match.Empty)
                {
                    fx.paras = ParseParameters(match.Value);
                }
                else
                {
                    fx.paras = new List<Parameter>();
                }
            }
            return fx;
        }

        private List<Parameter> ParseParameters(string parameterBody)
        {
            List<Parameter> parameters = new List<Parameter>();
            if (!string.IsNullOrEmpty(parameterBody))
            {
                Match match = ExtractParas.Match(parameterBody);
                while (match.Success)
                {
                    parameters.Add(ParseFunction(match.Value));
                    match = match.NextMatch();
                }
            }
            return parameters;
        }
    }

    class ScriptStructure
    {
        public string leading;
        public string body;
    }
}
