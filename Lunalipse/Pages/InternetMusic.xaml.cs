using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
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
    /// Interaction logic for InternetMusic.xaml
    /// </summary>
    public partial class InternetMusic : Page
    {
        public InternetMusic()
        {
            InitializeComponent();
        }

        private void InternetMusicSearch_Loaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManager_OnI18NEnvironmentChanged;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            TranslationManager_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            Foreground = obj.Foreground;
            SearchIt.Background = obj.Secondary.ToLuna();
            SearchBox.BorderBrush = obj.Secondary;
            SearchIt.BorderBrush = obj.Secondary;
        }

        private void TranslationManager_OnI18NEnvironmentChanged(II18NConvertor i8c)
        {
            foreach (ContentControl contentControl in Utils.FindVisualChildren<ContentControl>(this))
            {
                if (contentControl.Tag == null) continue;
                if (!(contentControl.Tag is string)) continue;
                contentControl.Content = i8c.ConvertTo(SupportedPages.CORE_CLOUD_LIB, contentControl.Tag as string);
            }
            
        }

        private void InternetMusicSearch_Unloaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged -= TranslationManager_OnI18NEnvironmentChanged;
            ThemeManagerBase.OnThemeApplying -= ThemeManagerBase_OnThemeApplying;
        }
    }
}
