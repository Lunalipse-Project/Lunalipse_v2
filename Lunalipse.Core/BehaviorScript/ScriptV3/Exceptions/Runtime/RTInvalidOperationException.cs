using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime
{
    public class RTInvalidOperationException : RuntimeException
    {
        public RTInvalidOperationException():base()
        {

        }

        public RTInvalidOperationException(string msg, TokenInfo location) : base(msg, location)
        {

        }
    }
}
