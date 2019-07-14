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
        public ThemeTuple()
        {

        }
        public ThemeTuple(Brush Forground, Brush Primary, Brush Secondary)
        {
            SetForeground(Forground);
            SetPrimary(Primary);
            SetSecondary(Secondary);
        }

        protected SolidColorBrush foreground, primary, secondary;
        protected LinearGradientBrush g_foreground, g_primary, g_secondary;
        /// <summary>
        /// Use gradient for f,p,s (foreground, primary, secondary) ?
        /// </summary>
        protected bool g_f, g_p, g_s;

        private void SetForeground(Brush f)
        {
            if(f.GetType()==typeof(LinearGradientBrush))
            {
                g_foreground = (LinearGradientBrush)f;
                g_f = true;
            }
            else
            {
                foreground = (SolidColorBrush)f;
                g_f = false;
            }
        }

        private void SetPrimary(Brush p)
        {
            if (p.GetType() == typeof(LinearGradientBrush))
            {
                g_primary = (LinearGradientBrush)p;
                g_p = true;
            }
            else
            {
                primary = (SolidColorBrush)p;
                g_p = false;
            }
        }

        private void SetSecondary(Brush s)
        {
            if (s.GetType() == typeof(LinearGradientBrush))
            {
                g_secondary = (LinearGradientBrush)s;
                g_s = true;
            }
            else
            {
                secondary = (SolidColorBrush)s;
                g_s = false;
            }
        }

        public Brush Foreground {
            get
            {
                if (g_f)
                    return g_foreground;
                else
                    return foreground;
            }
        }
        public Brush Primary
        {
            get
            {
                if (g_p)
                    return g_primary;
                else
                    return primary;
            }
        }
        public Brush Secondary
        {
            get
            {
                if (g_s)
                    return g_secondary;
                else
                    return secondary;
            }
        }
    }
}
