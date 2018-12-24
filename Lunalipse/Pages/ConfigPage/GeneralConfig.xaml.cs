using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lunalipse.Pages.ConfigPage
{
    /// <summary>
    /// GeneralConfig.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralConfig : Page, ITranslatable
    {
        public GeneralConfig()
        {
            InitializeComponent();

            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Foreground = obj.Foreground;
        }

        public void Translate(II18NConvertor i8c)
        {
            throw new NotImplementedException();
        }
    }
}
