using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Presentation.LpsWindow;
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

namespace Lunalipse.Pages.ConfigPage
{
    /// <summary>
    /// Licenses.xaml 的交互逻辑
    /// </summary>
    public partial class Licenses : Page, ITranslatable
    {
        string dialogueTitle = "";
        public Licenses()
        {
            InitializeComponent();

            TranslationManagerBase.OnI18NEnvironmentChanged += Translate;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;

            Loaded += Licenses_Loaded;

            Unloaded += Licenses_Unloaded;
        }

        private void Licenses_Unloaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged -= Translate;
            ThemeManagerBase.OnThemeApplying -= ThemeManagerBase_OnThemeApplying;
            Loaded -= Licenses_Loaded;
            Unloaded -= Licenses_Unloaded;
        }

        private void Licenses_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            Translate(TranslationManagerBase.AquireConverter());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            Foreground = obj.Foreground;
            Brush LunaedBackground = obj.Secondary.ToLuna();
            foreach (Button b in Utils.FindVisualChildren<Button>(this))
            {
                b.Background = LunaedBackground;
            }
        }

        public void Translate(II18NConvertor i8c)
        {
            foreach (ContentControl b in Utils.FindVisualChildren<ContentControl>(this))
            {
                if (b.Tag != null)
                {
                    b.Content = i8c.ConvertTo(SupportedPages.CORE_OPENSOURCE_LICENSE, b.Tag as string);
                }
            }
            License_desc.Text = i8c.ConvertTo(SupportedPages.CORE_OPENSOURCE_LICENSE, License_desc.Tag as string);
            License_lps_desc.Text = i8c.ConvertTo(SupportedPages.CORE_OPENSOURCE_LICENSE, License_lps_desc.Tag as string);
            dialogueTitle = i8c.ConvertTo(SupportedPages.CORE_OPENSOURCE_LICENSE, "CORE_OPENSOURCE_LICENSE_DIALOGUE_TITLE");
        }

        
        private void ViewLicense_Click(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Name;
            ViewLicensePage viewLicensePage = null;
            switch (name)
            {
                case "ViewLicense_Taglib":
                    viewLicensePage = new ViewLicensePage("GNU_LGPL");
                    break;
                case "ViewLicense_Json":
                    viewLicensePage = new ViewLicensePage("MIT");
                    break;
                case "ViewLicense_Cscore":
                case "ViewLicense_NVorbis":
                    viewLicensePage = new ViewLicensePage("MSPL");
                    break;
                case "ViewLicense_Lunalipse":
                    viewLicensePage = new ViewLicensePage("GNU_GPL");
                    break;
            }
            if (viewLicensePage != null)
            {
                UniversalDailogue licenseDialogue = new UniversalDailogue(viewLicensePage, dialogueTitle, MessageBoxButton.OK);
                licenseDialogue.ShowDialog();
            }
        }
    }
}
