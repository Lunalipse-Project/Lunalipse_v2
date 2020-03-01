using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public enum RelationType
    {
        ADD,
        MINUS,
        MULT,
        DIV,
        LG_AND, // &&
        LG_NOT, // !
        LG_OR,  // ||
        CP_G,   // >
        CP_L,   // <
        CP_LE,  // <=
        CP_GE,  // >=
        CP_EQ,   // ==
        CP_NEQ   // !=
    }
}
