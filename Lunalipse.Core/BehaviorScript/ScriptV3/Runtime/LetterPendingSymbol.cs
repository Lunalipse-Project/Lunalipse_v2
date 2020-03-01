using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;
using Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public class LetterPendingSymbol : LetterValue
    {
        SymbolTable lookUp;
        string symbol;
        public LetterPendingSymbol(string symbol, SymbolTable table, TokenInfo tokenInfo) : base(ElementType.PENDING, tokenInfo)
        {
            this.symbol = symbol;
            lookUp = table;
        }

        public LetterValue ResolvePending()
        {
            if (lookUp[symbol].GetLetterElementType() == ElementType.PENDING) 
            {
                throw new RuntimeException("CORE_LBS_RT_NEVER_INITIALIZE", ElementTokenInfo);
            }
            return lookUp[symbol];
        }
    }
}