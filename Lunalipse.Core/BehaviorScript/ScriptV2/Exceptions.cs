using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV2
{
    public class ScriptException : Exception
    {
        public object[] Args { get; private set; }
        public ScriptExceptionType ExceptionType { get; private set; }
        public ScriptException(string i18nformat, ScriptExceptionType exceptionType, params object[] args) : base(i18nformat)
        {
            Args = args;
            ExceptionType = exceptionType;
        }
    }

    public enum ScriptExceptionType
    {
        LEXICAL,
        SEMANTIC,
        RUNTIME
    }
}
