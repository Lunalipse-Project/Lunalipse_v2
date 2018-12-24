using Lunalipse.Common.Data.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common
{
    public class ErrorDelegation
    {
        public delegate void ErrorRaisedI18N(ErrorI18N error, params string[] args);
        public delegate void ErrorRaisedBSI(string key, string context, int indicateNum);

        public static ErrorRaisedI18N OnErrorRaisedI18N;
        public static ErrorRaisedBSI OnErrorRaisedBSI;

        public static event Action<string, string, string> GenericError;

        public static void OnRaisedGenericException(string componentID, string ExceptionMsg, string ExceptionStackTrace)
        {
            GenericError(componentID, ExceptionMsg, ExceptionStackTrace);
        }
    }
}
