using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
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
            throw new RuntimeException("CORE_LBS_RT_UNDEFINED");
        }

        public override T getValueAt<T>(int i)
        {
            throw new RuntimeException("CORE_LBS_RT_UNDEFINED");
        }

        public override LetterValue EvaluateWith(LetterValue operand, RelationType relationType)
        {
            throw new RuntimeException("CORE_LBS_RT_UNDEFINED");
        }

        public override object EvaluateByType(Type type)
        {
            throw new RuntimeException("CORE_LBS_RT_UNDEFINED");
        }

        public override void Evaluate()
        {
            throw new RuntimeException("CORE_LBS_RT_UNDEFINED");
        }

        public override object getValueByTypeAt(int i, Type t)
        {
            throw new RuntimeException("CORE_LBS_RT_UNDEFINED");
        }

        public override string ToString()
        {
            return "undefiend";
        }
    }
}
