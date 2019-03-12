﻿using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.PlayList;
using Lunalipse.Pages.ConfigPage.Structures;
using Lunalipse.Utilities;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace Lunalipse.Pages.ConfigPage
{
    /// <summary>
    /// GeneralConfig.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralConfig : Page, ITranslatable
    {
        CataloguePool CP;
        GLS GlobalSetting;
        MusicListPool MLP;
        EventBus Bus;

        const int BUTTON_NUM = 2;
        const int Label_NUM = 4;

        // 保存增加的Catalogue的UUID, 方便用户撤销操作
        //List<string> AddedCatalogues;

        public GeneralConfig()
        {
            InitializeComponent();

            //获取关键部件的实例
            CP = CataloguePool.INSATNCE;
            MLP = MusicListPool.INSATNCE();
            GlobalSetting = GLS.INSTANCE;
            Bus = EventBus.Instance;

            //变量初始化
            //AddedCatalogues = new List<string>();

            //主题监听器订阅
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());

            //界面语言监听器订阅
            TranslationManagerBase.OnI18NEnvironmentChanged += Translate;
            Translate(TranslationManagerBase.AquireConverter());

            //其他监听器
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

        /// <summary>
        /// 读取现有的配置，用于更新界面上的设置项
        /// </summary>
        void ReadManifest()
        {
            MusicPath.Clear();
            foreach(Catalogue c in CP.GetLocationClassified())
            {
                MusicPath.Add(new MusicPathSturc()
                {
                    TotalTime = c.getTotalElapse(),
                    DetailedDescription = c.Name,
                    UUID = c.UUID,
                    FileCount = c.GetCount()
                });
                //AddedCatalogues.Add(c.UUID);
            }
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
            //在调用SelectedIndex前（内部使用ControlTemplate.FindName实现）,显式更新ItemControl的布局，以免引发 InvalidOperationException
            MusicPath.UpdateLayout();
            MusicPath.SelectedIndex = 0;
        }

        // Add a new location
        private void ST_TN_F5_Click(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog folderBrowser = new WinForms.FolderBrowserDialog();
            if(folderBrowser.ShowDialog() == WinForms.DialogResult.OK)
            {
                string folderChoice = folderBrowser.SelectedPath;
                if (!GlobalSetting.MusicBaseDirs.Exists((x) => folderChoice == x))
                {
                    GlobalSetting.MusicBaseDirs.Add(folderChoice);
                    // 保存新增的Catalogue的uuid。
                    MLP.AddToPool(folderChoice);
                    ReadManifest();
                    // 发送全局广播，标志着添加动作已完成
                    // C_UPD : Catalogues以添加，其他界面可以进行刷新。
                    Bus.Boardcast(EventBusTypes.ON_ACTION_COMPLETE, "C_UPD");
                }
            }
        }

        // 移除一个位置
        private void ST_TN_F6_Click(object sender, RoutedEventArgs e)
        {
            MusicPathSturc mps = MusicPath.SelectedItem as MusicPathSturc;
            //AddedCatalogues.Remove(mps.UUID);
            string directory = mps.DetailedDescription;
            // 从所有歌曲结合中移除
            MLP.Musics.RemoveAll(x => System.IO.Path.GetDirectoryName(x.Path).Equals(directory));
            // 移除所有派生的歌单
            CP.RemoveChildrenCatalogue(mps.UUID);
            // 从所有Catalogue集合中移除
            CP.RemoveCatalogue(mps.UUID);
            // 从列表中移除
            MusicPath.Remove(mps);
            GlobalSetting.MusicBaseDirs.Remove(mps.DetailedDescription);
            Bus.Boardcast(EventBusTypes.ON_ACTION_COMPLETE, "C_UPD");
        }
    }
}