using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    class LetterAssign : LetterValue
    {
        LetterVariable identifier;
        LetterValue value;
        public LetterAssign(LetterVariable identifier, LetterValue value, TokenInfo tokenInfo) : base(ElementType.ASSIGN, tokenInfo)
        {
            this.identifier = identifier;
            this.value = value;
        }

        public override void Evaluate()
        {
            if(identifier.CanAssign)
            {
                ResolvePending();
                identifier.assign(value.EvaluateAs<LetterValue>());
            }
            else
            {
                throw new RTInvalidOperationException(
                    "CORE_LBS_RT_UNABLE_TO_ASSIGN",
                    ElementTokenInfo);
            }
        }

        private void ResolvePending()
        {
            if(value.GetLetterElementType()==ElementType.PENDING)
            {
                value = (value as LetterPendingSymbol).ResolvePending();
            }
        }
    }
}
