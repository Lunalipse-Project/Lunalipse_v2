using Lunalipse.Common;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Core.Markdown;
using System.Windows;
using System.Windows.Controls;

namespace Lunalipse.Pages
{
    /// <summary>
    /// ViewLicensePage.xaml 的交互逻辑
    /// </summary>
    public partial class ViewLicensePage : Page , IDialogPage
    {
        Markdown markdown = new Markdown();
        string licenseType;
        public ViewLicensePage(string licenseType)
        {
            InitializeComponent();
            Loaded += ViewLicensePage_Loaded;
            this.licenseType = licenseType;
        }

        private void ViewLicensePage_Loaded(object sender, RoutedEventArgs e)
        {
            string license = "";
            switch (licenseType)
            {
                case "MSPL":
                    license = AppConst.LICENSE_MS_PL_CSCORE;
                    break;
                case "GNU_GPL":
                    license = AppConst.LICENSE_GUNGPL_LUNALIPSE;
                    break;
                case "GNU_LGPL":
                    license = AppConst.LICENSE_GUNLGPL_TAGLIB;
                    break;
                case "MIT":
                    license = AppConst.LICENSE_MIT_JSON;
                    break;
                default:
                    license = AppConst.LICENSE_GUNGPL_LUNALIPSE;
                    break;
            }
            LicenseDocument.Document = markdown.CreateDocument(markdown.Parse(license));
        }

        public bool NegativeClicked()
        {
            return false;
        }

        public bool PositiveClicked()
        {
            return true;
        }

        public void UnifiedTheme(ThemeTuple themeTuple)
        {
            markdown.DocumentForeground = themeTuple.Foreground;
        }
    }
}
