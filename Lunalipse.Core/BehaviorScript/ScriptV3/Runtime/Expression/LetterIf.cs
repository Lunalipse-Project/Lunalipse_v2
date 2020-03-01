using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterIf : LetterValue
    {
        LetterParagraph IfBranch;
        LetterParagraph ElseBranch;
        LetterExpression Condition;
        public LetterIf(LetterParagraph IfBranch, LetterParagraph ElseBranch, LetterExpression Condition, TokenInfo tokenInfo) : base(ElementType.IF_ELSE, tokenInfo)
        {
            this.IfBranch = IfBranch;
            this.ElseBranch = ElseBranch;
            this.Condition = Condition;
        }

        public LetterParagraph Decision()
        {
            bool result = Condition.EvaluateAs<bool>();
            if (result)
            {
                return IfBranch;
            }
            else
            {
                return ElseBranch;
            }
        }

        public override void Evaluate()
        {
            
        }
    }
}
