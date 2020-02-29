using Antlr4.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser
{
    public class ParseExceptionStrategy : DefaultErrorStrategy
    {
        public override void Recover(Parser recognizer, RecognitionException e)
        {
            IToken token = e.OffendingToken;
            throw new GeneralSyntaxErrorException(token.Text, token.Line, token.Column);
        }

        public override IToken RecoverInline(Parser recognizer)
        {
            IToken token = recognizer.CurrentToken;
            throw new GeneralSyntaxErrorException(token.Text, token.Line, token.Column);
        }

        public override void Sync(Parser recognizer)
        {

        }
    }
}
