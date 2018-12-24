using Lunalipse.Common.Generic.GeneralSetting;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Pages.ConfigPage;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;

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
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void SPlanelSlider_OnSelectionChanged(Common.Interfaces.ILpsUI.LpsDetailedListItem selected, object tag = null)
        {
            SettingCatalogue cata = (SettingCatalogue)selected;
            switch (cata.CLASS)
            {
                case SettingCatalogues.SETTING_GENERAL:
                    SPanleViewer.ShowContent(new GeneralConfig(), true);
                    break;
            }
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Background = obj.Primary.SetOpacity(0.9).ToLuna();
        }

        public void Translate(II18NConvertor i8c)
        {
            SPlanelSlider.Translate(i8c);
            Title = i8c.ConvertTo("CORE_FUNC", Title);
        }

        void RegistCatas()
        {
            foreach(SettingCatalogues sc in Utils.GetValues<SettingCatalogues>())
            {
                SPlanelSlider.Add(new SettingCatalogue()
                {
                    CLASS = sc,
                    DetailedIcon = sc.ToString(),
                    DetailedDescription = "CORE_SETTING_" + sc.ToString()
                });
            }
        }
    }
}
