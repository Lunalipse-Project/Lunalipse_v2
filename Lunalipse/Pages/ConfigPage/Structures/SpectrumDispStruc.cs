using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Common.Interfaces.IVisualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Pages.ConfigPage.Structures
{
    public class SpectrumDispStruc : LpsDetailedListItem
    {
        public SpectrumDisplayer Displayer { get; set; }
        public SpectrumDispStruc()
        {
            base.DetailedIcon = "SpectrumDisplayer";
        }
    }
}
