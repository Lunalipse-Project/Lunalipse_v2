using Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions
{
    public class DuplicateSymbolException : FrontEndExceptionBase
    {
        public ElementType LetterValueType { get; private set; }
        public DuplicateSymbolException(ElementType exisit_id_type, TokenInfo OffendingToken, string message_format):base(OffendingToken, message_format)
        {
            LetterValueType = exisit_id_type;
        }
    }
}
