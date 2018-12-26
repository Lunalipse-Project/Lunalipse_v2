using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.PlayList;
using Lunalipse.I18N;
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
    /// GeneralConfig.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralConfig : Page, ITranslatable
    {
        CataloguePool cp;
        GLS GlobalSetting;

        const int BUTTON_NUM = 2;
        const int Label_NUM = 4;

        public GeneralConfig()
        {
            InitializeComponent();

            cp = CataloguePool.INSATNCE;
            GlobalSetting = GLS.INSTANCE;

            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());

            TranslationManager.OnI18NEnvironmentChanged += Translate;
            Translate(TranslationManager.AquireConverter());

            MusicPath.OnSelectionChanged += MusicPath_OnSelectionChanged;

            
        }

        

        private void MusicPath_OnSelectionChanged(Common.Interfaces.ILpsUI.LpsDetailedListItem selected, object tag = null)
        {
            MusicPathSturc mps = selected as MusicPathSturc;
            FullPath.Content = mps.DetailedDescription;
            FolderName.Content = System.IO.Path.GetFileNameWithoutExtension(mps.DetailedDescription);
            FileCount.Content = mps.FileCount;
            TimeDuration.Content = mps.TotalTime.ToString(@"hh\:mm\:ss");
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Foreground = obj.Foreground;
            for(int i = 5; i < 5 + BUTTON_NUM; i++)
            {
                Button b = FindName("ST_TN_F" + i) as Button;
                b.Background = obj.Surface.ToLuna();
            }
        }

        void ReadManifest()
        {
            foreach(Catalogue c in cp.GetLocationClassified())
            {
                MusicPath.Add(new MusicPathSturc()
                {
                    TotalTime = c.getTotalElapse(),
                    DetailedDescription = c.Name,
                    FileCount = c.GetCount()
                });
            }
            //MusicPath.SelectedIndex = 0;
        }

        public void Translate(II18NConvertor i8c)
        {
            for(int i = 1; i <= BUTTON_NUM + Label_NUM; i++)
            {
                ContentControl l = FindName("ST_TN_F" + i) as ContentControl;
                if (l == null) continue;
                l.Content = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, l.Tag as string);
            }
        }

        private void GeneralConfigPage_Loaded(object sender, RoutedEventArgs e)
        {
            ReadManifest();
        }
    }
}
