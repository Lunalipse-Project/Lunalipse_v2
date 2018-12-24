using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Lunalipse.Common.Generic.Themes
{
    public class ThemeTuple
    {
        public SolidColorBrush Foreground = null;
        public SolidColorBrush Primary;
        public SolidColorBrush Secondary;
        public SolidColorBrush Surface;
        [Obsolete]
        public SolidColorBrush Accent;
    }
}
