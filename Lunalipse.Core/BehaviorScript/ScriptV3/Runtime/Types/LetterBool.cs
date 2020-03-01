using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterBool : LetterValue, ICloneable
    {
        bool value;
        public LetterBool(bool value) : base(ElementType.BOOL)
        {
            this.value = value;
        }

        public object Clone()
        {
            return new LetterBool(value);
        }

        public bool IsHold()
        {
            return value;
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            if(operand == null)
            {
                if(relationType == RelationType.LG_NOT)
                {
                    return new LetterBool(!IsHold());
                }
            }
            else if(operand.GetLetterElementType()== ElementType.BOOL)
            {
                LetterBool boolean = operand as LetterBool;
                switch (relationType)
                {
                    case RelationType.CP_EQ:
                        return new LetterBool(boolean.IsHold() == IsHold());
                    case RelationType.CP_NEQ:
                        return new LetterBool(boolean.IsHold() != IsHold());
                    case RelationType.LG_AND:
                        return new LetterBool(boolean.IsHold() && IsHold());
                    case RelationType.LG_OR:
                        return new LetterBool(boolean.IsHold() || IsHold());
                }
            }
            throw new RuntimeException("CORE_LBS_RT_INVALID_OPERATION", ElementType.BOOL.ToString(), relationType.ToString());
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
            return Convert.ChangeType(value, type);
        }
    }
}
