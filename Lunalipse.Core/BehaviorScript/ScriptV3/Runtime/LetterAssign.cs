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
                identifier.assign(value.EvaluateAs<LetterValue>());
            }
            else
            {
                throw new RTInvalidOperationException(
                    $"[Error]: You can't change your Pinkie promise once you have made!",
                    ElementTokenInfo);
            }
        }
    }
}
