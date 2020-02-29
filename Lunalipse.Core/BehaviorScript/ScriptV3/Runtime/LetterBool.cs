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
