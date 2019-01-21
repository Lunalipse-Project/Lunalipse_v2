using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.GeneralSetting;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.GlobalSetting;
using Lunalipse.I18N;
using Lunalipse.Pages.ConfigPage;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;

namespace Lunalipse.Windows
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : LunalipseDialogue , ITranslatable
    {
        string SettingHash;
        string SaveSettingTitle;
        string SaveSettingContent;
        GlobalSettingHelper<GLS> globalSettingHelper;
        public Settings()
        {
            InitializeComponent();
            RegistCatas();
            SPlanelSlider.OnSelectionChanged += SPlanelSlider_OnSelectionChanged;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            TranslationManagerBase.OnI18NEnvironmentChanged += Translate;
            Translate(TranslationManagerBase.AquireConverter());

            SettingHash = GLS.INSTANCE.ComputeHash();
            globalSettingHelper = GlobalSettingHelper<GLS>.INSTANCE;
            globalSettingHelper.UseLZ78Compress = true;
        }

        private void SPlanelSlider_OnSelectionChanged(Common.Interfaces.ILpsUI.LpsDetailedListItem selected, object tag = null)
        {
            SettingCatalogue cata = (SettingCatalogue)selected;
            switch (cata.CLASS)
            {
                case SettingCatalogues.SETTING_GENERAL:
                    SPanleViewer.ShowContent(new GeneralConfig(), true);
                    break;

                //case SettingCatalogues.SETTING_ACTION_SAVE_CFG:
                //    SPanleViewer.ShowContent(new SaveAndApply(), true);
                //    break;
            }
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Background = obj.Primary.SetOpacity(0.9).ToLuna();
            SPlanelSlider.Background = obj.Primary.SetOpacity(1).ToLuna();
        }

        public void Translate(II18NConvertor i8c)
        {
            SPlanelSlider.Translate(i8c);
            Title = i8c.ConvertTo(SupportedPages.CORE_FUNC, Title);
            SaveSettingContent = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_SETTING_SAVE_SETTING_CONTENT");
            SaveSettingTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_SETTING_SAVE_SETTING_TITLE");
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
            if(SettingHash != GLS.INSTANCE.ComputeHash())
            {
                CommonDialog UserShouldSave = new CommonDialog(SaveSettingTitle, SaveSettingContent, System.Windows.MessageBoxButton.OKCancel);
                if(UserShouldSave.ShowDialog().Value)
                {
                    globalSettingHelper.SaveSetting(GLS.INSTANCE);
                }
            }
        }
    }
}
