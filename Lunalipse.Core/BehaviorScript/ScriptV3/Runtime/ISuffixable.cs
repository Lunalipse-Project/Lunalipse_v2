using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    public interface ISuffixable
    {
        void SetSuffixActions(LetterSuffixActions suffixActions);
        LetterSuffixActions GetSuffixActions();
    }
}
