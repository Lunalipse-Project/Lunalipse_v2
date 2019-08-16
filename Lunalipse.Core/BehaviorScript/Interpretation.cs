using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Utilities;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunalipse.Common;

namespace Lunalipse.Core.BehaviorScript
{
    public class Interpretation : IInterpretation
    {
        private Dictionary<int, string[]> ParameterPattern = new Dictionary<int, string[]>()
        {
            {0x0001,    // LUNA_PLAY
                new string[]{"String","Single"} },
            {0x0002,    // LUNA_PLAYN
                new string[]{"Int32", "Single" } },
            {0x0003,    // LUNA_PLAYC
                new string[]{"String","String", "Single" } },
            {0x0004,    // LUNA_EQZR
                new string[]{"Int32", "Int32", "Int32", "Int32", "Int32", "Int32", "Int32", "Int32", "Int32", "Int32"} },
            {0x0005,    // LUNA_NEXT
                null},
            {0x0006,    // LUNA_LOOP
                null},
            {0x0007,    // LUNA_SET
                new string[]{"String"} },
            {0x1001,    // SUFX_RAND
                new string[]{"Int32","Int32"} },
            {0x1002,    // SUFX_COUNT
                new string[]{"Int32"} }
        };
        private Dictionary<int, string> CustomPrefix = new Dictionary<int, string>();
        private Dictionary<int, string> CustomSuffix = new Dictionary<int, string>();
        int position = 0;
        public List<ActionToken> Interpret(List<ScriptToken> tokens)
        {
            position = 0;
            List<ActionToken> ats = new List<ActionToken>();
            foreach(ScriptToken token in tokens)
            {
                ats.Add(_interpret(token));
                position++;
            }
            return ats;
        }

        private ActionToken _interpret(ScriptToken token)
        {
            ActionToken at = new ActionToken();
            at.CommandType = (int)Cmd2DefinedCmd(token.Command);
            at.ct_args = ConvertArgs(at.CommandType, ParameterPattern[at.CommandType], token.Args, token.Command);

            at.SuffixType = (int)Suf2DefinedSuf(token.TailFix);
            at.st_args = ConvertArgs(at.SuffixType, ParameterPattern[at.SuffixType], token.TailArgs, token.Command);

            if (at.ct_args == null || at.st_args == null)
            {
                return null;
            }
            return at;
        }

        private object[] ConvertArgs(int cmdType, string[] argsType, string[] srcArg,string curCmd)
        {
            List<object> args = new List<object>();
            if (srcArg.Length > argsType.Length)
            {
                ErrorDelegation.OnErrorRaisedBSI?.Invoke("CORE_LBS_NoSuchPara", "{0}({1})".FormateEx(curCmd, string.Join(",", argsType)), position);
                return null;
            }
            for (int i = 0; i < srcArg.Length; i++)
            {
                
                Type t = Type.GetType("System." + argsType[i]);
                try
                {
                    args.Add(Convert.ChangeType(srcArg[i], t));
                }
                catch (Exception e)
                {
                    if(e is FormatException || e is OverflowException)
                    {
                        ErrorDelegation.OnErrorRaisedBSI?.Invoke("CORE_LBS_NoSuchPara", "{0}({1})".FormateEx(curCmd, string.Join(",", argsType)), position);
                    }
                    return null;
                }
                
            }
            return args.ToArray();
        }

        public bool AddParameterPattern(int type, string[] pattern)
        {
            return ParameterPattern.Add4nRep(type, pattern);
        }

        public bool RemoveParameterPattern(int type)
        {
            return ParameterPattern.Remove(type);
        }

        public string[] GetPattern(int type)
        {
            return ParameterPattern[type];
        }

        public int Cmd2DefinedCmd(string cmd)
        {
            if (cmd.StartsWith("luna.", true, null))
            {
                cmd = cmd.Replace('.', '_');
            }
            else
            {
                string _c = cmd;
                cmd = "LUNA_" + _c;
            }
            cmd = cmd.ToUpper();
            return Enum.IsDefined(typeof(DefinedCmd), cmd) ? 
                (int)(DefinedCmd)Enum.Parse(typeof(DefinedCmd), cmd) : TCustomPrefix(cmd);
        }

        public int Suf2DefinedSuf(string suf)
        {
            if (suf.StartsWith("sufx.", true, null))
            {
                suf = suf.Replace('.', '_');
            }
            else
            {
                string _c = suf;
                suf = "SUFX_" + _c;
            }
            suf = suf.ToUpper();
            return Enum.IsDefined(typeof(DefinedSuffix), suf) ? 
                (int)(DefinedSuffix)Enum.Parse(typeof(DefinedSuffix), suf) : TCustomSuffix(suf);
        }

        public string DeifnedCmd2Cmd(uint cmd)
        {
            return Enum.GetName(typeof(DefinedCmd), cmd);
        }
        public string DeifnedSuf2Suf(uint suf)
        {
            return Enum.GetName(typeof(DefinedSuffix), suf);
        }

        private int TCustomPrefix(string Func)
        {
            foreach(var v in CustomPrefix)
            {
                if(v.Value.Equals(Func))
                {
                    return v.Key;
                }
            }
            return 0x0000;
        }

        private int TCustomSuffix(string Func)
        {
            foreach (var v in CustomSuffix)
            {
                if (v.Value.Equals(Func))
                {
                    return v.Key;
                }
            }
            return 0x1000;
        }

        public bool AddPrefix(uint type, string name)
        {
            if (type >= 0x1000) return false;
            return CustomPrefix.Add4nRep((int)type, name);
        }

        public bool AddSuffix(uint type, string name)
        {
            if (type < 0x1000) return false;
            return CustomSuffix.Add4nRep((int)type, name);
        }

        public bool RemovePrefix(int type)
        {
            return CustomPrefix.Remove(type);
        }

        public bool RemoveSuffix(int type)
        {
            return CustomSuffix.Remove(type);
        }
    }
}
