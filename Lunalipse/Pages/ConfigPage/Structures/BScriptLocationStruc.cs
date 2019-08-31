using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Core.BehaviorScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Pages.ConfigPage.Structures
{
    public class BScriptLocationStruc : LpsDetailedListItem
    {
        public BScriptLocation bScriptLocation { get; private set; }
        public BScriptLocationStruc(BScriptLocation bScriptLocation)
        {
            DetailedIcon = "BScript_File";
            DetailedDescription = bScriptLocation.ScriptName;
            this.bScriptLocation = bScriptLocation;
        }

    }
}
