using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core;
using Lunalipse.Core.Markdown;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Utilities;
using LunaNetCore;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Lunalipse.Pages.ConfigPage
{
    /// <summary>
    /// UpdateCheck.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateCheck : Page, ITranslatable
    {
        string UpdateFound, Latest, Checking;
        UpdateHelper updateHelper;
        ReleaseInfo updateInfo;
        Markdown markdownParser = new Markdown();
        DoubleAnimation ExpandDocView = new DoubleAnimation(0, 340, new Duration(TimeSpan.FromMilliseconds(450)));
        EventBus eventBus;

        string Reminding_Title, Reminding_Content;
        string InstallNow, InstallLater;

        double percentage = 0.0;
        public UpdateCheck()
        {
            InitializeComponent();
            //主题监听器订阅
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying; ;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            //界面语言监听器订阅
            TranslationManagerBase.OnI18NEnvironmentChanged += Translate;
            Translate(TranslationManagerBase.AquireConverter());
            Loaded += UpdateCheck_Loaded;
            updateHelper = new UpdateHelper();
            updateHelper.OnQueryCompleted += UpdateHelper_OnQueryCompleted;

            eventBus = EventBus.Instance;

            Unloaded += UpdateCheck_Unloaded;
        }

        private void UpdateCheck_Unloaded(object sender, RoutedEventArgs e)
        {
            ThemeManagerBase.OnThemeApplying -= ThemeManagerBase_OnThemeApplying;
            TranslationManagerBase.OnI18NEnvironmentChanged -= Translate;
            Loaded -= UpdateCheck_Loaded;
            updateHelper.OnQueryCompleted -= UpdateHelper_OnQueryCompleted;
            Unloaded -= UpdateCheck_Unloaded;
        }

        private void UpdateHelper_OnQueryCompleted()
        {
            bool hasUpdate = (updateInfo = updateHelper.UpdateAvailability()) != null;
            if (hasUpdate)
            {
                UpdateIndicator(hasUpdate, updateInfo.Tag);
            }
        }

        private void UpdateCheck_Loaded(object sender, RoutedEventArgs e)
        {

            AvailabilityIndicator.Visibility = Visibility.Hidden;
            StatusDisplay.Content = Checking;
            updateHelper.QueryLatestUpdate();
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            
            Foreground = obj.Foreground;
            Spinning.SpinnerDotRadius = 5;
            markdownParser.DocumentForeground = obj.Foreground;
            DonwloadUpdate.Background = obj.Secondary.ToLuna();
        }

        private void DonwloadUpdate_Click(object sender, RoutedEventArgs e)
        {
            Asset pack = updateHelper.FindPackDownloadAssets(updateInfo);
            if(!string.IsNullOrEmpty(pack.DownloadURL))
            {
                string argument = "-s:u \"{0}\" -s:v {1} -s:s {2}";
                GLS.INSTANCE.UpdateArguments = argument.FormateEx(pack.DownloadURL, updateInfo.Tag, pack.FileSize);
                CommonDialog commonDialog = new CommonDialog(Reminding_Title, Reminding_Content, MessageBoxButton.YesNo, InstallNow, InstallLater);
                if (commonDialog.ShowDialog().Value)
                {
                    //立刻开始安装
                    eventBus.Boardcast(EventBusTypes.ON_ACTION_START, "END_SESSION");
                }
            }
        }

        public void Translate(II18NConvertor i8c)
        {
            UpdateFound = i8c.ConvertTo(SupportedPages.CORE_UPDATE_CHECKER, "CORE_UPDATECHECKER_FOUND");
            Latest = i8c.ConvertTo(SupportedPages.CORE_UPDATE_CHECKER, "CORE_UPDATECHECKER_LATEST");
            Checking = i8c.ConvertTo(SupportedPages.CORE_UPDATE_CHECKER, "CORE_UPDATECHECKER_CHECKING");
            Reminding_Title = i8c.ConvertTo(SupportedPages.CORE_UPDATE_CHECKER, "CORE_UPDATECHECKER_REMINDTITLE");
            Reminding_Content = i8c.ConvertTo(SupportedPages.CORE_UPDATE_CHECKER, "CORE_UPDATECHECKER_REMINDCONTENT");
            InstallNow = i8c.ConvertTo(SupportedPages.CORE_UPDATE_CHECKER, "CORE_UPDATECHECKER_NOW");
            InstallLater = i8c.ConvertTo(SupportedPages.CORE_UPDATE_CHECKER, "CORE_UPDATECHECKER_LATER");
            DonwloadUpdate.Content = i8c.ConvertTo(SupportedPages.CORE_UPDATE_CHECKER, "CORE_UPDATECHECKER_DONWLOAD");

        }

        void UpdateIndicator(bool hasUpdate,string current)
        {
            Spinning.StopSpinning();
            Dispatcher.Invoke(() =>
            {
                AvailabilityIndicator.Content = FindResource(hasUpdate ? "SETTING_ABOUT" : "Tick");
                AvailabilityIndicator.Visibility = Visibility.Visible;
                StatusDisplay.Content = hasUpdate ? UpdateFound.FormateEx(current) : Latest;
                Spinning.Visibility = Visibility.Hidden;
                ReleaseNote.Document = markdownParser.CreateDocument(markdownParser.Parse(updateInfo.body));
                DocContainer.BeginAnimation(HeightProperty, ExpandDocView);
            });
        }
    }
}
