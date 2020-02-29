using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions
{
    public class UnmatchedParamter : FrontEndExceptionBase
    {
        int param_expected, param_actual;
        public UnmatchedParamter(int param_expected, int param_actual, TokenInfo OffendingToken, string errMsgFormat) : base(OffendingToken, errMsgFormat)
        {
            this.param_actual = param_actual;
            this.param_expected = param_expected;
        }
    }
}
