using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterUndefined : LetterValue
    {
        public LetterUndefined() : base(ElementType.GAMMY)
        {

        }

        public override T EvaluateAs<T>()
        {
            throw new NullReferenceException("Undefined");
        }

        public override T getValueAt<T>(int i)
        {
            throw new NullReferenceException("Undefined");
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            throw new NullReferenceException("Undefined");
        }

        public override string ToString()
        {
            return "undefiend";
        }
    }
}
