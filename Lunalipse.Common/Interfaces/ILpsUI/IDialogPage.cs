using Lunalipse.Common.Generic.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.ILpsUI
{
    public interface IDialogPage
    {
        bool PositiveClicked();
        bool NegativeClicked();
        void UnifiedTheme(ThemeTuple themeTuple);
    }
}
