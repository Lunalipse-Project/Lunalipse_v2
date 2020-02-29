using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterString : LetterValue,ICloneable
    {
        string str_content;
        public LetterString(string str) : base(ElementType.STRING)
        {
            str_content = str.Trim('"');
        }

        public static LetterValue Create(ITerminalNode node)
        {
            TokenInfo tokenInfo = new TokenInfo(node.Symbol.Line, node.Symbol.Column, node.Symbol.Text);
            return new LetterString(node.Symbol.Text);
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            if(operand is LetterString)
            {
                return new LetterString(str_content + (operand as LetterString).str_content);
            }
            return new LetterString(str_content + operand.EvaluateAs<string>());
        }
        
        public override T EvaluateAs<T>()
        {
            Type t = typeof(T);
            return (T)EvaluateByType(t);
        }

        public override object EvaluateByType(Type type)
        {
            if (type == GetType() || type == typeof(LetterValue))
            {
                return Clone();
            }
            return Convert.ChangeType(str_content, type);
        }
        public override string ToString()
        {
            return str_content;
        }

        public object Clone()
        {
            return new LetterString(str_content);
        }
    }
}
