using Lunalipse.Common.Interfaces.ILpsUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Pages.ConfigPage.Structures
{
    public class SControllerStruc: LpsDetailedListItem
    {
        public string ControllerID { get; private set; }
        public string ControllerDesc { get; set; }
        public SControllerStruc(string id)
        {
            ControllerID = id;
            DetailedIcon = "Sequence";
            DetailedDescription = string.Empty;
            I18NDescription = "CORE_SEQCTRLER_" + id;
        }
    }
}
