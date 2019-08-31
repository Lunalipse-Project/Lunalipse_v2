using CSCore.DSP;
using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Audio;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Common.Interfaces.IVisualization;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Core.Markdown;
using Lunalipse.Core.PlayList;
using Lunalipse.Core.Visualization;
using Lunalipse.Pages.ConfigPage.Structures;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.LpsComponent;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
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
        VisualizationManager vManager;
        CommonDialog commonDialog;
        SequenceControllerManager controllerManager;

        const int BUTTON_NUM = 2;
        const int Label_NUM = 4;

        bool load_complete = false;
        string Alter_Title, Alter_Restart, Alter_FFTEnable;
        string HelpTitle, HelpWASAPI, HelpAudioLatency,HelpFFTEnable,HelpFFTSize;

        SupportLanguages supportLanguages = SupportLanguages.CHINESE_SIM;

        // 保存增加的Catalogue的UUID, 方便用户撤销操作
        //List<string> AddedCatalogues;

        public GeneralConfig()
        {
            InitializeComponent();

            //获取关键部件的实例
            CP = CataloguePool.Instance;
            MLP = MusicListPool.Instance();
            GlobalSetting = GLS.INSTANCE;
            Bus = EventBus.Instance;
            vManager = VisualizationManager.Instance;
            controllerManager = SequenceControllerManager.Instance;

            //变量初始化
            //AddedCatalogues = new List<string>();

            //主题监听器订阅
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;

            //界面语言监听器订阅
            TranslationManagerBase.OnI18NEnvironmentChanged += Translate;

            //其他监听器
            MusicPath.OnSelectionChanged += MusicPath_OnSelectionChanged;
            Displayers.OnSelectionChanged += Displayers_OnSelectionChanged;
            Controllers.OnSelectionChanged += Controllers_OnSelectionChanged;

            Unloaded += GeneralConfig_Unloaded;

            foreach (FftSize size in (FftSize[])Enum.GetValues(typeof(FftSize)))
            {
                SpectrumFFTSize.Add(size.ToString().Remove(0, 3), size);
            }
        }

        private void Controllers_OnSelectionChanged(Common.Interfaces.ILpsUI.LpsDetailedListItem selected, object tag = null)
        {
            SControllerStruc sController = selected as SControllerStruc;
            if (sController != null)
            {
                ControllerName.Content = sController.DetailedDescription;
                ControllerDesc.Text = sController.ControllerDesc;
                if(selected.Equals(Controllers.Items[0]))
                {
                    SetController.Visibility = Visibility.Hidden;
                }
                else
                {
                    SetController.Visibility = Visibility.Visible;
                }
            }
        }

        private void Displayers_OnSelectionChanged(Common.Interfaces.ILpsUI.LpsDetailedListItem selected, object tag = null)
        {
            SpectrumDispStruc spectrumDispStruc = selected as SpectrumDispStruc;
            SpectrumResolution.Text = spectrumDispStruc.Displayer.DesireResolution.ToString();
            SpectrumStyleSelector.SetSelectionByVal(spectrumDispStruc.Displayer.currentStyle);
        }

        private void GeneralConfigPage_Loaded(object sender, RoutedEventArgs e)
        {
            ReadManifest();
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            Translate(TranslationManagerBase.AquireConverter());
            //在调用SelectedIndex前（内部使用ControlTemplate.FindName实现）,显式更新ItemControl的布局，以免引发 InvalidOperationException
            MusicPath.UpdateLayout();
            MusicPath.SelectedIndex = 0;
            Displayers.SelectedIndex = 0;
            Controllers.SelectedIndex = 0;
            LanguageSelection.SelectedIndex = 0;
            SpectrumScalingStrategy.SetSelectionByVal(vManager.ScalingStrategy);
            LanguageSelection.OnSelectionChanged += LanguageSelection_OnSelectionChanged;           
            LangFollowSystem.OnSwitchStatusChanged += (y, x) => GlobalSetting.UseSystemDefaultLanguage = x;

            load_complete = true;
        }

        private void GeneralConfig_Unloaded(object sender, RoutedEventArgs e)
        {
            ThemeManagerBase.OnThemeApplying -= ThemeManagerBase_OnThemeApplying;
            TranslationManagerBase.OnI18NEnvironmentChanged -= Translate;
            Unloaded -= GeneralConfig_Unloaded;
            LanguageSelection.OnSelectionChanged -= LanguageSelection_OnSelectionChanged;

            load_complete = false;
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
            foreach(Button b in Utils.FindVisualChildren<Button>(this))
            {
                b.Background = obj.Secondary.ToLuna();
            }
            LanguageSelection.DropDownBackground = obj.Secondary;
            SpectrumStyleSelector.DropDownBackground = obj.Secondary;
            SpectrumScalingStrategy.DropDownBackground = obj.Secondary;
            SpectrumFFTSize.DropDownBackground = obj.Secondary;
        }

        /// <summary>
        /// 读取现有的配置，用于更新界面上的设置项
        /// </summary>
        void ReadManifest()
        {
            load_complete = false;
            MusicPath.Clear();
            Displayers.Clear();
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
            foreach(KeyValuePair<string,SpectrumDisplayer> kvp in vManager.Displayers)
            {
                SpectrumDispStruc spectrumDisplayer = new SpectrumDispStruc()
                {
                    Displayer = kvp.Value,
                    DetailedDescription = kvp.Key.Remove(0, 5)
                };
                Displayers.Add(spectrumDisplayer);
            }
            foreach(string id in controllerManager.Controllers.Keys)
            {
                if(GlobalSetting.SelectedController == id)
                {
                    Controllers.Items.Insert(0, new SControllerStruc(id));
                }
                else
                {
                    Controllers.Add(new SControllerStruc(id));
                }
            }
            SpectrumFFTSize.SetSelectionByVal(vManager.FftSize);
            SpectrumScalingStrategy.SetSelectionByVal(vManager.ScalingStrategy);
            Enum.TryParse(GlobalSetting.CurrentSelectedLanguage, out supportLanguages);
            LangFollowSystem.Toggle(GlobalSetting.UseSystemDefaultLanguage);
            EnableSpectrum.Toggle(GlobalSetting.FFTEnabled);
            EnableImmersed.Toggle(GlobalSetting.ImmerseMode);

            SpectrumFPS.Text = GlobalSetting.SpectrumFPS.ToString();
            AudioLatency.Text = GlobalSetting.AudioLatency.ToString();

            if (MusicPath.Items.Count == 0)
            {
                ST_TN_F6.Visibility = Visibility.Hidden;
            }
            else
            {
                ST_TN_F6.Visibility = Visibility.Visible;
            }

            load_complete = true;
        }

        public void Translate(II18NConvertor i8c)
        {
            foreach (ContentControl contentControl in Utils.FindVisualChildren<ContentControl>(this))
            {
                if (contentControl.Tag == null) continue;
                if (!(contentControl.Tag is string)) continue;
                contentControl.Content = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, contentControl.Tag as string);
            }
            ST_TN_F7.Text = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, ST_TN_F7.Tag as string);
            ST_TN_F8.Text = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, ST_TN_F8.Tag as string);
            ST_TN_F9.Text = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, ST_TN_F9.Tag as string);
            ST_TN_F11.Text = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, ST_TN_F11.Tag as string);
            Alter_Title = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_ALTER_TITLE");
            Alter_Restart = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_ALTER_RESTART");
            Alter_FFTEnable = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_ALTER_FFTENABLE");
            HelpTitle = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_HELP");
            HelpAudioLatency = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_AUDIO_LATENCY_DESC");
            HelpWASAPI = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_AUDIO_IMMSERED_DESC");
            HelpFFTEnable = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_SPECTRUM_ENABLE_DESC");
            HelpFFTSize = i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_SPECTRUM_FFTSIZE_DESC");
            LanguageSelection.Clear();
            SpectrumScalingStrategy.Clear();
            SpectrumStyleSelector.Clear();
            foreach (SupportLanguages supportLanguage in (SupportLanguages[])Enum.GetValues(typeof(SupportLanguages)))
            {
                LanguageSelection.Add(i8c.ConvertTo(SupportedPages.MULTI_LANG, supportLanguage.ToString()), supportLanguage);
            }
            foreach (ScalingStrategy scaling in (ScalingStrategy[])Enum.GetValues(typeof(ScalingStrategy)))
            {
                SpectrumScalingStrategy.Add(i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_SPECTRUM_SCALING_" + scaling), scaling);
            }
            foreach (string str in vManager.GetSpectrumProvidersName)
            {
                SpectrumStyleSelector.Add(i8c.ConvertTo(SupportedPages.CORE_GENERAL_SETTING, "CORE_SETTING_" + str), str);
            }
            foreach(SControllerStruc sController in Controllers.Items)
            {
                sController.ControllerDesc = i8c.ConvertTo(SupportedPages.CORE_FUNC, sController.I18NDescription + "_DESC");
            }
            MusicPath.Translate(i8c);
            Controllers.Translate(i8c);       
        }

        private void LanguageSelection_OnSelectionChanged(object obj)
        {
            //TODO 切换界面语言
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
                    ProgressDialogue dialogue = new ProgressDialogue((indicator) => MLP.AddToPool(folderChoice, indicator));
                    dialogue.ShowDialog();
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

        private void SpectrumFPS_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch(button.Name)
            {
                case "Apply_SpectrumFPS":
                    int fps = vManager.SpectrumUpdatePreSecond;
                    if(int.TryParse(SpectrumFPS.Text,out fps))
                        vManager.SpectrumUpdatePreSecond = fps;
                    else
                        SpectrumFPS.Text = fps.ToString();
                    break;
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Name)
            {
                case "Help_WASAPI":
                    (new MarkdownDialog(typeof(Markdown), HelpTitle, HelpWASAPI, MessageBoxButton.OK)).ShowDialog();
                    break;
                case "Help_AudioLatency":
                    (new MarkdownDialog(typeof(Markdown), HelpTitle, HelpAudioLatency, MessageBoxButton.OK)).ShowDialog();
                    break;
                case "Help_EnableFFT":
                    (new MarkdownDialog(typeof(Markdown), HelpTitle, HelpFFTEnable, MessageBoxButton.OK)).ShowDialog();
                    break;
                case "Help_fftsize":
                    (new MarkdownDialog(typeof(Markdown), HelpTitle, HelpFFTSize, MessageBoxButton.OK)).ShowDialog();
                    break;
            }
        }

        private void SpectrumScalingStrategy_OnSelectionChanged(object obj)
        {
            ScalingStrategy scaling = (ScalingStrategy)obj;
            if(scaling != vManager.ScalingStrategy)
            {
                vManager.ScalingStrategy = scaling;
            }
            GlobalSetting.scalingStrategy = scaling;
        }

        private void SpectrumFFTSize_OnSelectionChanged(object obj)
        {
            vManager.FftSize = (FftSize)obj;
            GlobalSetting.fftSize = vManager.FftSize;
        }

        private void SetController_Click(object sender, RoutedEventArgs e)
        {
            int selected = Controllers.SelectedIndex;
            controllerManager.SetController((Controllers.SelectedItem as SControllerStruc).ControllerID);
            Controllers.Swap(selected, 0);
            Controllers.SelectedIndex = 0;
            SetController.Visibility = Visibility.Hidden;
            GlobalSetting.SelectedController = controllerManager.CurrentControlerID;
        }

        // Apply the change of Displayer
        private void ST_TN_F10_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = Displayers.SelectedIndex;
            SpectrumDispStruc spectrumDisp = Displayers.SelectedItem as SpectrumDispStruc;
            int resolution = 0;
            if(!int.TryParse(SpectrumResolution.Text,out resolution))
            {
                SpectrumResolution.Text = spectrumDisp.Displayer.DesireResolution.ToString();
                return;
            }
            string styleID = SpectrumStyleSelector.SelectedItemValue as string;
            vManager.UpdateDisplayerConfig("DISP_" + spectrumDisp.DetailedDescription, styleID, resolution);
            (Displayers.SelectedItem as SpectrumDispStruc).Displayer = vManager.Displayers["DISP_" + spectrumDisp.DetailedDescription];
        }

        private void OnSwitchStatusChanged(object sender, bool status)
        {
            if (load_complete)
            {
                ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                switch (toggleSwitch.Name)
                {
                    case "LangFollowSystem":
                        GlobalSetting.UseSystemDefaultLanguage = status;
                        break;
                    case "EnableImmersed":
                        if(status)
                        {
                            commonDialog = new CommonDialog(Alter_Title, Alter_Restart, MessageBoxButton.YesNo);
                            if (!commonDialog.ShowDialog().Value)
                            {
                                EnableImmersed.Toggle(false);
                            }
                        }
                        GlobalSetting.ImmerseMode = status;
                        break;
                    case "EnableSpectrum":
                        if(status)
                        {
                            commonDialog = new CommonDialog(Alter_Title, Alter_FFTEnable, MessageBoxButton.YesNo);
                            if (!commonDialog.ShowDialog().Value)
                                EnableSpectrum.Toggle(false);
                        }
                        GlobalSetting.FFTEnabled = status;
                        vManager.EnableSpectrum = status;
                        break;
                }
            }
        }
    }
}
