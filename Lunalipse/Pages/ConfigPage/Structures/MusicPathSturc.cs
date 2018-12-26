using Lunalipse.Common.Interfaces.ILpsUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Pages.ConfigPage.Structures
{
    public class MusicPathSturc : LpsDetailedListItem
    {
        public TimeSpan TotalTime { get; set; }
        public int FileCount { get; set; }
        public MusicPathSturc()
        {
            base.DetailedIcon = "FolderStar";
        }
    }
}
