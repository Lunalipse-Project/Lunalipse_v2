using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Utilities;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV1
{
    public class Parser : IParser
    {
        public Parser()
        {
            Whole = "";
            Tokens = new List<ScriptToken>();
        }

        public string RootPath { get; set; }
        public string Whole { get; protected set; }
        public string ID { get; protected set; }
        public List<ScriptToken> Tokens { get; protected set; }

        internal Action<PRAGMA, string[]> MarcoProcessor;
        /// <summary>
        /// Deliver either errors or warning.
        /// </summary>
        public Action<string, string, int> ErrorOccured;

        Regex preExract = new Regex(@"(.*?)[(?=(\()]|[^:]*[0-9]",RegexOptions.Compiled);
        Regex MLCommentRemoval = new Regex(@"\!\-.*\-\!", RegexOptions.Singleline | RegexOptions.Compiled);
        Regex SLCommentRemoval = new Regex(@"\!\!.*", RegexOptions.Compiled);
        Regex argExract = new Regex(@"([^,]+)", RegexOptions.Compiled);

        public bool Load(string id, bool append = false)
        {
            string absPath = "{0}/{1}.lbs".FormateEx(RootPath, id);
            if (!append)
            {
                Whole = "";
                Tokens.Clear();
            }
            if (append)
            {
                if (id == ID) throw new StackOverflowException();
                Whole += _load(absPath);
            }
            Whole = _load(absPath);
            ID = id;
            return Whole.AvailableEx();
        }

        public bool LoadPath(string path, bool append = false)
        {

            if (!append)
            {
                Tokens.Clear();
                Whole = "";
            }
            if (append)
                Whole += _load(path);
            Whole = _load(path);
            return Whole.AvailableEx();
        }
        public bool Parse()
        {
            int pos = 0;
            foreach (string each in Whole.Split('\n'))
            {
                pos++;
                if (!each.AvailableEx()) continue;
                try
                {
                    if (each.StartsWith("#pragma", true, null))
                    {
                        ParsingMarco(each);
                        continue;
                    }
                    Tokens.Add(Tokenize(each));
                }
                catch(FormatException)
                {
                    ErrorOccured?.Invoke("CORE_LBS_SynatxError", each, pos);
                    return false;
                }
                catch(StackOverflowException)
                {
                    ErrorOccured?.Invoke("CORE_LBS_InfiniteCall", each, pos);
                    return false;
                }
                catch(KeyNotFoundException)
                {
                    ErrorOccured?.Invoke("CORE_LBS_PragmaNotFound", each, pos);
                    return false;
                }
            }
            return true;
        }

        public ScriptToken Tokenize(string line)
        {
            ScriptToken st = new ScriptToken();
            if (!Validator(line) || !preExract.IsMatch(line))
            {
                throw new FormatException();
            }
            MatchCollection mc = preExract.Matches(line);
            st.Command = mc[0].Groups[1].Value;
            string[] _cmd = st.Command.Split('.');
            if (_cmd.Length<=1)
                st.Command = _cmd[0];
            else
            {
                st.Prefix = _cmd[0];
                st.Command = _cmd[1];
            }

            if (mc.Count > 2)
            {
                st.TailFix = mc[2].Groups[1].Value.AvailableEx() ? 
                    ScriptUtil.SanitaizeParameter(mc[2].Groups[1].Value) : "COUNT";
            }
            List<string> args = new List<string>();
            foreach(Match m in argExract.Matches(mc[1].Groups[1].Value))
            {
                args.Add(ScriptUtil.SanitaizeParameter(m.Groups[0].Value));
            }
            args.RemoveAll((x) =>
            {
                return x == "" || x =="\"";
            });
            st.Args = args.ToArray();
            args.Clear();

            if (mc.Count > 3)
            {
                foreach (Match m in argExract.Matches(mc[3].Groups[1].Value))
                {
                    args.Add(ScriptUtil.SanitaizeParameter(m.Groups[0].Value));
                }
            }
            else if(mc.Count > 2)
            {
                args.Add(mc[2].Value);
            }
            else
            {
                st.TailFix = "COUNT";
                args.Add("1");
            }
            st.TailArgs = args.ToArray();
            args.Clear();
            return st;
        }

        public bool Validator(string line)
        {
            Stack<int> check = new Stack<int>();
            foreach(char c in line)
            {
                if (c == '(') check.Push(1);
                else if (c == ')') check.Pop();
            }
            return check.Count == 0;
        }

        private string _load(string absPath)
        {
            try
            {
                string read = "";
                using (StreamReader sr = new StreamReader(absPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            line = SLCommentRemoval.Replace(line, string.Empty);
                            line = MLCommentRemoval.Replace(line, string.Empty);
                            if (!string.IsNullOrEmpty(line))
                            {
                                read += line + "\n";
                            }
                        }
                    }
                }
                read = MLCommentRemoval.Replace(read, string.Empty);
                return read;
            }
            catch(Exception)
            {
                ErrorOccured?.Invoke("CORE_LBS_FileNotFound", absPath, -1);
                return "";
            }
        }

        private void ParsingMarco(string MarcoLine)
        {
            MarcoLine = MarcoLine.Remove(0, 8);
            MarcoLine = MarcoLine.Remove(MarcoLine.Length - 1, 1);
            string[] args = MarcoLine.Split(' ');
            if (args.Length < 1) return;
            if (args[0] == "INSERT")
            {
                try
                {
                    Load(args[1], true);
                    Parse();
                }
                catch (StackOverflowException sofw)
                {
                    throw sofw;
                }
            }
            else
            {
                if (!Enum.IsDefined(typeof(PRAGMA), args[0])) throw new KeyNotFoundException();
                MarcoProcessor?.Invoke((PRAGMA)Enum.Parse(typeof(PRAGMA), args[0]), args);
            }
        }
    }
}
