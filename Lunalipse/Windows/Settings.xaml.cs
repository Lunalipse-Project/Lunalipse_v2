using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.GeneralSetting;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.GlobalSetting;
using Lunalipse.I18N;
using Lunalipse.Pages;
using Lunalipse.Pages.ConfigPage;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;
using System;

namespace Lunalipse.Windows
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : LunalipseDialogue , ITranslatable
    {
        
        public Settings()
        {
            InitializeComponent();
            RegistCatas();
            SPlanelSlider.OnSelectionChanged += SPlanelSlider_OnSelectionChanged;
            TranslationManagerBase.OnI18NEnvironmentChanged += Translate;
            Translate(TranslationManagerBase.AquireConverter());

            this.Loaded += Settings_Loaded;
            Unloaded += Settings_Unloaded;
        }

        private void Settings_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SPlanelSlider.OnSelectionChanged -= SPlanelSlider_OnSelectionChanged;
            TranslationManagerBase.OnI18NEnvironmentChanged -= Translate;
            this.Loaded -= Settings_Loaded;
            Unloaded -= Settings_Unloaded;
        }

        private void Settings_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SPlanelSlider.UpdateLayout();
            SPlanelSlider.SelectedIndex = 0;
        }

        private void SPlanelSlider_OnSelectionChanged(Common.Interfaces.ILpsUI.LpsDetailedListItem selected, object tag = null)
        {
            SettingCatalogue cata = (SettingCatalogue)selected;
            Frame_Scrollviewer.ScrollToHome();
            switch (cata.CLASS)
            {
                case SettingCatalogues.SETTING_GENERAL:
                    SPanleViewer.ShowContent(new GeneralConfig(), true);
                    break;
                case SettingCatalogues.SETTING_PERFORMANCE:
                    SPanleViewer.ShowContent(new AppearanceConfig(), true);
                    break;
                case SettingCatalogues.SETTING_ABOUT:
                    SPanleViewer.ShowContent(new LunalipseAbout(), true);
                    break;
                case SettingCatalogues.SETTING_UPDATECHECK:
                    SPanleViewer.ShowContent(new UpdateCheck(), true);
                    break;
                case SettingCatalogues.SETTING_LICENSES:
                    SPanleViewer.ShowContent(new Licenses(), true);
                    break;
                    //case SettingCatalogues.SETTING_ACTION_SAVE_CFG:
                    //    SPanleViewer.ShowContent(new SaveAndApply(), true);
                    //    break;
            }
        }

        protected override void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            base.ThemeManagerBase_OnThemeApplying(obj);
            if (obj == null) return;
            Background = obj.Primary.SetOpacity(1).ToLuna();
        }

        public void Translate(II18NConvertor i8c)
        {
            SPlanelSlider.Translate(i8c);
            Title = i8c.ConvertTo(SupportedPages.CORE_FUNC, Title);
        }

        void RegistCatas()
        {
            foreach(SettingCatalogues sc in Utils.GetValues<SettingCatalogues>())
            {
                SPlanelSlider.Add(new SettingCatalogue()
                {
                    CLASS = sc,
                    DetailedIcon = sc.ToString(),
                    I18NDescription = "CORE_SETTING_" + sc.ToString(),
                    DetailedDescription = ""
                });
            }
        }

        private void LunalipseDialogue_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}
