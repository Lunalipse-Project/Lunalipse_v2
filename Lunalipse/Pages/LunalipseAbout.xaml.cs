using Lunalipse.Common;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Utilities;
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

namespace Lunalipse.Pages
{
    /// <summary>
    /// LunalipseAbout.xaml 的交互逻辑
    /// </summary>
    public partial class LunalipseAbout : Page
    {
        VersionHelper versionHelper;
        public LunalipseAbout()
        {
            InitializeComponent();
            versionHelper = VersionHelper.Instance;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(Common.Interfaces.II18N.II18NConvertor obj)
        {
            LpsVersion.Content = obj.ConvertTo(SupportedPages.CORE_ABOUT_SETTING, LpsVersion.Tag as string).FormateEx(versionHelper.getFullVersion());
#if BUILD
            LpsCode.Content = obj.ConvertTo(SupportedPages.CORE_ABOUT_SETTING, LpsCode.Tag as string).FormateEx(versionHelper.getGenerationTypedVersion(LunalipseGeneration.Build));
#elif ALPHA
            LpsCode.Content = obj.ConvertTo(SupportedPages.CORE_ABOUT_SETTING, LpsCode.Tag as string).FormateEx(versionHelper.getGenerationTypedVersion(LunalipseGeneration.Alpha));
#elif BETA
            LpsCode.Content = obj.ConvertTo(SupportedPages.CORE_ABOUT_SETTING, LpsCode.Tag as string).FormateEx(versionHelper.getGenerationTypedVersion(LunalipseGeneration.Beta));
#elif RELEASE
            LpsCode.Content = obj.ConvertTo(SupportedPages.CORE_ABOUT_SETTING, LpsCode.Tag as string).FormateEx(versionHelper.getGenerationTypedVersion(LunalipseGeneration.Release));
#endif
            LpsBuildDate.Content = obj.ConvertTo(SupportedPages.CORE_ABOUT_SETTING, LpsBuildDate.Tag as string).FormateEx(versionHelper.LinkerTime.ToString("MMMM dd yyyy HH:mm:ss"));
            LpsLicense.Content = obj.ConvertTo(SupportedPages.CORE_ABOUT_SETTING, LpsLicense.Tag as string);
            LpsFor.Content = obj.ConvertTo(SupportedPages.CORE_ABOUT_SETTING, LpsFor.Tag as string);
            LpsCopyright.Content = obj.ConvertTo(SupportedPages.CORE_ABOUT_SETTING, LpsCopyright.Tag as string);
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            Foreground = obj.Foreground;
        }
    }
}
