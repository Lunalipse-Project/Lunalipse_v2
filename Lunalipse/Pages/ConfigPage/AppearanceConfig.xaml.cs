using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Core.Theme;
using Lunalipse.Pages.ConfigPage.Structures;
using Lunalipse.Presentation.LpsComponent;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        static readonly Regex _regex = new Regex("[^0-9.-]+");
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

            Unloaded += AppearanceConfig_Unloaded;
        }

        private void AppearanceConfig_Unloaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged -= Translate;
            ThemeManagerBase.OnThemeApplying -= ThemeManagerBase_OnThemeApplying;
            ThemeList.OnSelectionChanged -= ThemeList_OnSelectionChanged;
            Unloaded -= AppearanceConfig_Unloaded;
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
            LyricFontFamily.DropDownBackground = obj.Secondary;
            
            foreach (Button button in Utils.FindVisualChildren<Button>(this))
            {
                button.Background = obj.Secondary;
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
            ST_TN_F4.Text = i8c.ConvertTo(SupportedPages.CORE_APPEARANCE_SETTING, ST_TN_F4.Tag as string);

            ThemeList.Translate(i8c);
        }

        private void APPR_CFG_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            Translate(TranslationManagerBase.AquireConverter());
            ReadManifest();
            EnableSongHint.OnSwitchStatusChanged += OnSwitchStatusChanged;
            NonPassiveLyricDisp.OnSwitchStatusChanged += OnSwitchStatusChanged;
            EnableGuassianBlur.OnSwitchStatusChanged += OnSwitchStatusChanged;
            ThemeColorFollowAlbum.OnSwitchStatusChanged += OnSwitchStatusChanged;
            LyricFontFamily.OnSelectionChanged += LyricFontFamily_OnSelectionChanged;
        }

        private void LyricFontFamily_OnSelectionChanged(object obj)
        {
            FontFamily fontFamily = obj as FontFamily;
            GlobalSetting.LyricFontFamily = fontFamily.Source;
            GlobalSetting.LyricFontFamilyInternal = fontFamily;
            GlobalSetting.InvokeSettingChange("LyricFontFamily");
        }

        private void OnSwitchStatusChanged(object sender, bool status)
        {
            string name = (sender as ToggleSwitch).Name;
            switch(name)
            {
                case "EnableSongHint":
                    GlobalSetting.ShowNextSongHint = status;
                    break;
                case "NonPassiveLyricDisp":
                    GlobalSetting.LyricEnabled = status;
                    break;
                case "EnableGuassianBlur":
                    GlobalSetting.EnableGuassianBlur = status;
                    break;
                case "ThemeColorFollowAlbum":
                    GlobalSetting.ThemeColorFollowAlbum = status;
                    if (!status) themeManager.Restore();
                    break;
            }
        }

        private void ReadManifest()
        {
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
            EnableSongHint.Toggle(GlobalSetting.ShowNextSongHint);
            NonPassiveLyricDisp.Toggle(GlobalSetting.LyricEnabled);
            EnableGuassianBlur.Toggle(GlobalSetting.EnableGuassianBlur);
            ThemeColorFollowAlbum.Toggle(GlobalSetting.ThemeColorFollowAlbum);
            foreach(FontFamily fonts in Fonts.SystemFontFamilies)
            {
                LyricFontFamily.Add(fonts.Source, fonts);
            }
            LyricFontFamily.SetSelectionByVal(new FontFamily(GlobalSetting.LyricFontFamily));
            LyricFontSize.Text = GlobalSetting.LyricFontSize.ToString();
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

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void LyricFontSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
            uint TextSize = 0;
            if (uint.TryParse(LyricFontSize.Text, out TextSize))
            {
                if (TextSize >= 5 && TextSize <= 100)
                {
                    GlobalSetting.LyricFontSize = (int)TextSize;
                    GlobalSetting.InvokeSettingChange("LyricFontSize");
                }
            }
        }

        private void LyricFontSize_TextInput(object sender, TextCompositionEventArgs e)
        {
            
        }
    }
}
