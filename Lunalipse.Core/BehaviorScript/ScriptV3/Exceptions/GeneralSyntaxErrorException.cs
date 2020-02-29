using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions
{
    public class GeneralSyntaxErrorException : Exception
    {
        public string MismatchedToken { get; private set; }
        public int ErrorLine { get; private set; }
        public int ErrorColumn { get; private set; }

        public GeneralSyntaxErrorException(string MismatchedToken, int ErrorLine, int ErrorColumn) : base()
        {
            this.MismatchedToken = MismatchedToken;
            this.ErrorLine = ErrorLine;
            this.ErrorColumn = ErrorColumn;
        }
    }
}
