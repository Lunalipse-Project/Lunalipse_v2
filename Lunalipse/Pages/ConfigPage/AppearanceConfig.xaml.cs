using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Core.Theme;
using Lunalipse.Pages.ConfigPage.Structures;
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
    /// AppearanceConfig.xaml 的交互逻辑
    /// </summary>
    public partial class AppearanceConfig : Page, ITranslatable
    {
        List<ThemeContainer> themeContainers;
        LThemeManager themeManager;
        ThemeListStruc SelectedStruc;
        int SelectedIndex;
        GLS GlobalSetting;
        public AppearanceConfig()
        {
            InitializeComponent();
            //主题监听器订阅
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;

            //界面语言监听器订阅
            TranslationManagerBase.OnI18NEnvironmentChanged += Translate;

            themeManager = LThemeManager.Instance;
            themeContainers = themeManager.GetLoadedThemes();

            ThemeList.OnSelectionChanged += ThemeList_OnSelectionChanged;

            GlobalSetting = GLS.INSTANCE;
        }

        private void ThemeList_OnSelectionChanged(LpsDetailedListItem selected, object tag = null)
        {
            SelectedStruc = selected as ThemeListStruc;
            ThemeName.Content = SelectedStruc.DetailedDescription;
            ThemeDesc.Text = SelectedStruc.Description;
            if (ThemeList.SelectedIndex == 0) SetDefault.Visibility = Visibility.Hidden;
            else SetDefault.Visibility = Visibility.Visible;
            SelectedIndex = ThemeList.SelectedIndex;
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            Foreground = obj.Foreground;
            foreach (Button button in Utils.FindVisualChildren<Button>(this))
            {
                button.Background = obj.Primary;
            }
        }

        public void Translate(II18NConvertor i8c)
        {
            foreach(ContentControl contentControl in Utils.FindVisualChildren<ContentControl>(this))
            {
                if (contentControl.Tag == null) continue;
                if (!(contentControl.Tag is string)) continue;
                contentControl.Content = i8c.ConvertTo(SupportedPages.CORE_APPEARANCE_SETTING, contentControl.Tag as string);
            }
            ST_TN_F2.Text = i8c.ConvertTo(SupportedPages.CORE_APPEARANCE_SETTING, ST_TN_F2.Tag as string);
            ThemeList.Translate(i8c);
        }

        private void APPR_CFG_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            Translate(TranslationManagerBase.AquireConverter());
            ThemeContainer themeContainer = themeContainers.Find(x => x.Uid.Equals(GlobalSetting.DefaultThemeUUID));
            ThemeList.Add(new ThemeListStruc()
            {
                Description = themeContainer.Description,
                DetailedDescription = themeContainer.Name,
                isBuildIn = themeContainer.isBuildIn,
                Uid = themeContainer.Uid
            });
            themeContainers.ForEach(item =>
            {
                if (item == themeContainer) return;
                ThemeList.Add(new ThemeListStruc()
                {
                    Description = item.Description,
                    DetailedDescription = item.Name,
                    isBuildIn = item.isBuildIn,
                    Uid = item.Uid
                });
            });
        }

        private void SetDefault_Click(object sender, RoutedEventArgs e)
        {
            GlobalSetting.DefaultThemeUUID = SelectedStruc.Uid;
            LpsDetailedListItem old = ThemeList.Items[0];
            ThemeList.Items[0] = ThemeList.Items[SelectedIndex];
            ThemeList.Items[SelectedIndex] = old;
            LThemeManager.Instance.SelectTheme(SelectedStruc.Uid);
            ThemeList.SelectedIndex = 0;
        }
    }
}
