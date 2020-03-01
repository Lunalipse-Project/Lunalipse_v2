using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterExpression : LetterValue
    {
        List<LetterValue> RPNExpression;
        Stack<LetterValue> EvalStack;
        public LetterExpression(TokenInfo tokenInfo) : base(ElementType.EXPRESSION, tokenInfo)
        {
            RPNExpression = new List<LetterValue>();
            EvalStack = new Stack<LetterValue>();
        }

        public LetterExpression(LetterRPN RPN, TokenInfo tokenInfo) : this(tokenInfo)
        {
            RPNExpression.AddRange(RPN.getRPN());
        }

        public void AddSymbol(LetterValue elementBase)
        {
            RPNExpression.Add(elementBase);
        }

        public void SetRPNE(LetterRPN RPN)
        {
            RPNExpression.Clear();
            RPNExpression.AddRange(RPN.getRPN());
        }

        public override T EvaluateAs<T>()
        {
            return (T)EvaluateByType(typeof(T));
        }

        public override object EvaluateByType(Type type)
        {
            EvalStack.Clear();
            for (int i = 0; i < RPNExpression.Count; i++)
            {
                LetterValue value = RPNExpression[i];
                if (value.GetLetterElementType() == ElementType.PENDING)
                {
                    value = (value as LetterPendingSymbol).ResolvePending();
                    RPNExpression[i] = value;
                }
                LetterValue operand1 = null, operand2 = null;
                if (value.GetLetterElementType() == ElementType.RELATION)
                {
                    LetterRelation relation = value as LetterRelation;
                    if (!relation.isUnary)
                    {
                        operand1 = EvalStack.Pop();
                        operand2 = EvalStack.Pop();
                    }
                    else
                    {
                        operand2 = EvalStack.Pop();
                    }
                    EvalStack.Push(operand2.EvaluateWith(operand1, relation.GetRelationType()));
                }
                else
                {
                    EvalStack.Push(value);
                }
            }
            return EvalStack.Peek().EvaluateByType(type);
        }
    }

    public class LetterRPN : LetterValue
    {
        List<LetterValue> rpn;

        public LetterRPN() : base(ElementType.EXPRESSION)
        {
            rpn = new List<LetterValue>();
        }
        public LetterRPN(LetterValue firstElement) :this()
        {
            rpn.Add(firstElement);
        }

        public LetterRPN(LetterRPN op1, LetterRelation relation, LetterRPN op2) : this()
        {
            if(op1!=null)
            {
                rpn.AddRange(op1.getRPN());
            }
            rpn.AddRange(op2.getRPN());
            rpn.Add(relation);
        }

        public LetterRPN(LetterRelation relation, LetterRPN op1) : this(null, relation, op1)
        {

        }

        public LetterRPN Merge(LetterRPN letterRPN)
        {
            LetterRPN newRPN = new LetterRPN();
            newRPN.rpn.AddRange(rpn);
            newRPN.rpn.AddRange(letterRPN.rpn);
            return newRPN;
        }

        public List<LetterValue> getRPN()
        {
            return rpn;
        }
    }
}
