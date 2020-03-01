using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterNumber: LetterValue, ICloneable
    {
        double number;
        public LetterNumber() : base(ElementType.NUMBER, null)
        {

        }
        public LetterNumber(string numberString) : this()
        {
            number = double.Parse(numberString);
        }

        public LetterNumber(double number) : this()
        {
            this.number = number;
        }

        public override T EvaluateAs<T>()
        {
            Type t = typeof(T);
            return (T)EvaluateByType(t);
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            if (operand == null)
            {
                switch(relationType)
                {
                    case RelationType.ADD:
                        return new LetterNumber(number);
                    case RelationType.MINUS:
                        return new LetterNumber(-number);
                }
            }
            else if(operand is LetterNumber)
            {
                LetterNumber op = operand as LetterNumber;
                switch(relationType)
                {
                    case RelationType.ADD:
                        return new LetterNumber(number + op.number);
                    case RelationType.MINUS:
                        return new LetterNumber(number - op.number);
                    case RelationType.MULT:
                        return new LetterNumber(number * op.number);
                    case RelationType.DIV:
                        return new LetterNumber(number / op.number);
                    case RelationType.CP_EQ:
                        return new LetterBool(number == op.number);
                    case RelationType.CP_NEQ:
                        return new LetterBool(number != op.number);
                    case RelationType.CP_G:
                        return new LetterBool(number > op.number);
                    case RelationType.CP_L:
                        return new LetterBool(number < op.number);
                    case RelationType.CP_GE:
                        return new LetterBool(number >= op.number);
                    case RelationType.CP_LE:
                        return new LetterBool(number <= op.number);
                }
            }
            throw new RuntimeException("CORE_LBS_RT_INVALID_OPERATION", GetLetterElementType().ToString(), relationType.ToString());
        }

        public override object EvaluateByType(Type type)
        {
            if (type == GetType() || type == typeof(LetterValue))
            {
                return Clone();
            }
            return Convert.ChangeType(number, type);
        }

        public override bool Equals(object obj)
        {
            if(obj is LetterNumber)
            {
                return number == (obj as LetterNumber).number;
            }
            return number.CompareTo(obj) == 0;
        }

        public static LetterValue Create(ITerminalNode node)
        {
            string num = node != null ? node.Symbol.Text : "0";
            TokenInfo tokenInfo = new TokenInfo(node.Symbol.Line, node.Symbol.Column, node.Symbol.Text);
            return new LetterNumber(num);
        }

        public object Clone()
        {
            return new LetterNumber(number);
        }
    }
}
