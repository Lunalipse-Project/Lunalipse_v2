using Lunalipse.Common.Interfaces.ILpsUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Pages.ConfigPage.Structures
{
    public class ThemeListStruc : LpsDetailedListItem
    {
        bool buildin = false;

        public bool isBuildIn
        {
            get => buildin;
            set {
                if (value)
                {
                    DetailedIcon = "ThemeTemplate_BuildIn";
                }
                buildin = value;
            }
        }

        public string Description { get; set; }
        public string Uid { get; set; }
        public ThemeListStruc()
        {
            base.DetailedIcon = "ThemeTemplate";
        }
    }
}
