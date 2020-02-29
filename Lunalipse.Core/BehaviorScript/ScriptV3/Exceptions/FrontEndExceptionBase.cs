using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions
{
    public class FrontEndExceptionBase : Exception
    {
        public TokenInfo OffendingToken { get; private set; }
        protected string customMessageFormat;

        public FrontEndExceptionBase(TokenInfo OffendingToken, string messageFormat)
        {
            this.OffendingToken = OffendingToken;
            customMessageFormat = messageFormat;
        }
    }
}
