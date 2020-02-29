using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions
{
    public class GeneralSemanticException : FrontEndExceptionBase
    {
        public GeneralSemanticException(TokenInfo OffendingToken, string errMsgFormat) : base(OffendingToken, errMsgFormat)
        {
            
        }
    }
}
