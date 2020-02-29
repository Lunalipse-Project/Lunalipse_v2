using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.SyntaxParser
{
    public class TokenInfo
    {
        public int Line { get; private set; }
        public int Column { get; private set; }
        public string TokenText { get; private set; }

        public TokenInfo(int Line, int Column, string TokenText)
        {
            this.Line = Line;
            this.Column = Column;
            this.TokenText = TokenText;
        }

        public static TokenInfo CreateTokenInfo(IToken token)
        {
            return new TokenInfo(token.Line, token.Column, token.Text);
        }
    }
}
