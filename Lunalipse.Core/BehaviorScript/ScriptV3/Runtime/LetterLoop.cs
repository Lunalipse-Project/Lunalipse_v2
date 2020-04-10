using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterLoop : LetterValue
    {
        public LetterLoop(LetterParagraph loopStatement) : base(ElementType.LOOP)
        {
            Paragraph = loopStatement;
        }

        public LetterParagraph Paragraph { get; }

        public override void Evaluate()
        {
            
        }
    }
}
