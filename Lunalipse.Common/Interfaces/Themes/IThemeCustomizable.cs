using Lunalipse.Common.Generic.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.Themes
{
    public interface IThemeCustomizable
    {
        void ThemeOverriding(ThemeTuple themeTuple);
        string ComponentUID();
    }
}
