using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lunalipse.Core;
using Lunalipse.Core.PlayList;
using Lunalipse.Core.Metadata;
using Lunalipse.Core.Cache;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Core.Theme;
using Lunalipse.Core.GlobalSetting;
using Lunalipse.Core.Visualization;
using Lunalipse.Common;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.AudioControlPanel;
using Lunalipse.Common.Generic.Catalogue;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.IVisualization;
using Lunalipse.Common.Bus.Event;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Pages;
using Lunalipse.Windows;
using Lunalipse.Auxiliary;
using Lunalipse.Utilities;
using Lunalipse.Utilities.Misc;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Core.BehaviorScript.ScriptV3;
using Lunalipse.Core.BehaviorScript.ScriptV3.Exceptions.Runtime;

namespace Lunalipse
{
    /*
     *                                                                                                                                         
                                                                                                                                                                                
                        -:=>;="`                                                                                                                                
                   =TMB@@@@@@@0k=`                                                                                                                              
                !K#@@@@@@@@B]'                                                                                                                                  
              ,Z@@@@@@@@@@D`                     .}yyyVVclv                                                                                                     
             ^@@@@@@@@@@@#-                        m@@@@Y_         ~gQ8$*     .w6ObHr  `"<)}y=  ^OBBBQo*   *wzv``r*^=:-`       :*!:r:         ``                
            *@@@@@@@@@@@@#-                         8@@m      "O##@)]@@O :5QQQ8*M@@O`  "a@@@@M   `Q@@)    `#@@@) !g@@@@@BV` `V#@$OQ@T`8@###QQQ@Z                
            R@@@@@@@@@@@@@8-              _r        b@@k        #@# _@@K   0@@@#v@@?    v@@@@#    s@@,    `Z##8x  _@@Q`h@@d !@@@BMT^` 'Q@@MuZrUd                
            8@@@@@@@@@@@@@@@d|`         rO@H        b@@Z    `   O@@`.#@w   }@@#@@@@r    Q@#g@@!   I@@*  :_ :@@R   _@@@@#0z:.^]d8R0@@H  I@@@@@^                  
            }@@@@@@@@@@@@@@@@@@gUTx\xVO#@@@x        d@@Q  .3@;  Y@@Ue@@x   6@@|d@@@V  `U@@D6@@H   M@@GvI@e Y@@#r  "@@#-    `c#@@lD@#\ .Q@@XiI}0y                
             V@@@@@@@@@@@@@@@@@@@@@@@@@@@@R        *#@@@#@@@@}   *OBQZ*  -wdMGk,Q@@#}`PQQBWr@@@I`U@##BBQQj"???r* -KBBBY      .z8B$V' `g###BBBQQy                
              v#@@@@@@@@@@@@@@@@@@@@@@@@Bv        -WWPmIzyVuT!                  `:-`       ePUV}_                                                               
               `uB@@@@@@@@@@@@@@@@@@@@gL                                                                                                                        
                  ,udB@@@@@@@@@@@@QU|-                                                                                                                          
                      `_^r?\(r>:`                                                                                                                               
                                                                                                                                                                              
                                                                           Copyright Lunaixsky 2019
     */

    public partial class MainWindow : LunalipseMainWindow, ITranslatable
    {
        const int sliderSize = 200;

        private MusicListPool mlp;
        private GlobalSettingHelper<GLS> globalSettingHelper;
        private CataloguePool CPOOL;
        private MediaMetaDataReader mmdr;
        private CacheHub cacheSystem;
        private EventBus Bus;
        private PlaylistGuard playlistGuard;
        private LpsCore core;
        private GLS GlobalSetting = GLS.INSTANCE;
        private VersionHelper versionHelper;
        private LThemeManager themeManager;
        private BitmapAnalyser bitmapAnalyser;
        private VisualizationManager vManager;
        private BehaviorScriptManager bsManager;
        private SequenceControllerManager controllerManager;
        private II18NConvertor i18NConvertor;
        

        private MusicDetail musicDetailPage;

        private CatalogueShowCase showcase;
        private MusicSelected musicList;
        private InternetMusic internetMusicPage;

        private Duration elapseTime = new Duration(TimeSpan.FromMilliseconds(250));
        private DoubleAnimation ExpandPanel;
        private DoubleAnimation MinimizePanel;

        List<Catalogue> ByLocation, ByArtist, ByAlbum, ByUserDefined;
        private DesktopDisplay desktopDisplay;

        private string LinearMode, SingleLoop, ShuffleMode,NextSongHint;
        private string SettingHash;
        private string SaveSettingTitle;
        private string SaveSettingContent;
        private string BasePath;

        /// <summary>
        /// 检测<seealso cref="DesktopDisplay"/>是否还活着
        /// </summary>
        public static event Action DesktopDisplayIndicator;

        public MainWindow() : base()
        {
            BasePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            InitializeComponent();
            InitializeModules();
            RegisteringCommands();
            ExpandPanel = new DoubleAnimation(48, sliderSize, elapseTime);
            MinimizePanel = new DoubleAnimation(sliderSize, 48, elapseTime);

            EventBus.OnBoardcastRecieved += EventBus_OnBoardcastRecieved;
            

            TranslationManagerBase.OnI18NEnvironmentChanged += Translate;
            Translate(TranslationManagerBase.AquireConverter());

            ProgressDialogue progressDialogue = new ProgressDialogue(indicator =>
            {
                foreach (string path in GLS.INSTANCE.MusicBaseDirs)
                {
                    mlp.AddToPool(path, indicator, true);
                }
                indicator.Complete();
            });
            progressDialogue.ShowDialog();
            playlistGuard.Restore();

            CataloguesRefleshAll();

            this.EnableBlur = GlobalSetting.EnableGuassianBlur;
            SettingHash = GlobalSetting.ComputeHash();
        }

        private void EventBus_OnBoardcastRecieved(EventBusTypes busTypes, object Tag)
        {
            if(busTypes == EventBusTypes.ON_ACTION_COMPLETE)
            {
                switch(Tag)
                {
                    // 广播源：GenralConfig
                    // 信息：Catalogue已完成添加
                    case "C_UPD":
                        CataloguesRefleshAll();
                        break;
                    case "C_UPD_ALB":
                        CataloguesRefleshAlbum();
                        break;
                    case "C_UPD_ART":
                        CataloguesRefleshArtist();
                        break;
                    case "C_UPD_USR":
                        CatalogueRefleshUser();
                        break;
                }
            }
            if(busTypes == EventBusTypes.ON_ACTION_START && Tag.Equals("END_SESSION"))
            {
                this.Close();
            }
        }

        #region Reflesh the list
        private void CataloguesRefleshAll()
        {
            Task.Factory.StartNew(() =>
            {
                mlp.CreateAlbumClasses();
                mlp.CreateArtistClasses();
                ByLocation = CPOOL.GetLocationClassified();
                ByAlbum = CPOOL.GetAlbumClassfied();
                ByArtist = CPOOL.GetArtistClassfied();
                ByUserDefined = CPOOL.GetUserDefined();
            });
        }

        private void CataloguesRefleshAlbum()
        {
            Task.Factory.StartNew(() =>
            {
                mlp.CreateAlbumClasses();
                ByAlbum = CPOOL.GetAlbumClassfied();
            });
        }

        private void CataloguesRefleshArtist()
        {
            Task.Factory.StartNew(() =>
            {
                mlp.CreateArtistClasses();
                ByArtist = CPOOL.GetArtistClassfied();
            });
        }

        private void CatalogueRefleshUser()
        {
            Task.Factory.StartNew(() =>
            {
                ByUserDefined = CPOOL.GetUserDefined();
                Dispatcher.Invoke(() =>
                {
                    if (CATALOGUES.TAG == CatalogueSections.USER_PLAYLISTS)
                    {
                        FPresentor.ShowContent(showcase, false, () => showcase.SetCatalogues(ByUserDefined));
                    }
                });
            });
        }
        #endregion

        public void Translate(II18NConvertor converter)
        {
            i18NConvertor = converter;
            CATALOGUES.Translate(converter);
            playlistGuard.Translate(converter);
            LinearMode = converter.ConvertTo(SupportedPages.CORE_FUNC, "CORE_MAINUI_MODE_LINEAR");
            SingleLoop = converter.ConvertTo(SupportedPages.CORE_FUNC, "CORE_MAINUI_MODE_SINGLELOOP");
            ShuffleMode = converter.ConvertTo(SupportedPages.CORE_FUNC, "CORE_MAINUI_MODE_SHUFFLE");
            NextSongHint = converter.ConvertTo(SupportedPages.CORE_FUNC, "CORE_MAINUI_TOAST_SONGHINT");
            SaveSettingContent = converter.ConvertTo(SupportedPages.CORE_FUNC, "CORE_SETTING_SAVE_SETTING_CONTENT");
            SaveSettingTitle = converter.ConvertTo(SupportedPages.CORE_FUNC, "CORE_SETTING_SAVE_SETTING_TITLE");
            update_ok_title = converter.ConvertTo(SupportedPages.CORE_FUNC, "CORE_UPDATE_AVAILABLE_TITLE");
            update_ok_content = converter.ConvertTo(SupportedPages.CORE_FUNC, "CORE_UPDATE_AVAILABLE_CTN");
        }

        /// <summary>
        /// 初始化Lunalipse运行时必需组件
        /// </summary>
        private void InitializeModules()
        {          
            CPOOL = CataloguePool.Instance;
            versionHelper = VersionHelper.Instance;
            themeManager = LThemeManager.Instance;
            Bus = EventBus.Instance;
            cacheSystem = CacheHub.Instance(BasePath);
            mlp = MusicListPool.Instance(mmdr = new MediaMetaDataReader());
            globalSettingHelper = GlobalSettingHelper<GLS>.Instance;
            desktopDisplay = new DesktopDisplay();
            playlistGuard = new PlaylistGuard();
            bitmapAnalyser = new BitmapAnalyser();

            core = LpsCore.Session("MAIN_AUDIO_SESSION", GlobalSetting.ImmerseMode, GlobalSetting.AudioLatency);
            core.OnMusicComplete += PlayFinished;
            core.OnMusicPrepared += MusicPerpeared;
            core.OnMusicProgressChanged += NotifyChanged;
            core.CurrentMusicVolume = 70;

            vManager = VisualizationManager.Instance;
            vManager.EnableSpectrum = GlobalSetting.FFTEnabled;
            vManager.AddStyleProvider("SPECTRUM_CLASSIC", typeof(LineSpectrum));
            vManager.ScalingStrategy = GlobalSetting.scalingStrategy;
            vManager.FftSize = GlobalSetting.fftSize;

            if (GlobalSetting.SpectrumDisplayers.Count > 0)
            {
                foreach(KeyValuePair<string,SpectrumDisplayCfg> pair in GlobalSetting.SpectrumDisplayers)
                {
                    vManager.RegisterDisplayer(pair.Key, null, pair.Value.Resolution, pair.Value.Style);
                }
            }

            ControlPanel.Value = 0;
            ControlPanel.OnProgressChanged += ControlPanel_OnProgressChanged;
            ControlPanel.OnVolumeChanged += ControlPanel_OnVolumeChanged;
            ControlPanel.OnTrigging += ControlPanel_OnTrigging;
            ControlPanel.OnProfilePictureClicked += ControlPanel_OnProfilePictureClicked;
            ControlPanel.OnModeChange += ControlPanel_OnModeChange;

            CATALOGUES.OnSelectionChange += CATALOGUES_OnSelectionChange;
            CATALOGUES.OnMenuButtonClicked += CATALOGUES_OnMenuButtonClicked;
            CATALOGUES.OnConfigClicked += CATALOGUES_OnConfigClicked;

            showcase = new CatalogueShowCase();
            showcase.CatalogueSelected += Showcase_CatalogueSelected;
            internetMusicPage = new InternetMusic();

            musicList = new MusicSelected();
            musicList.OnSelectedMusicChange += MusicList_OnSelectedMusicChange;

            musicDetailPage = new MusicDetail();

            this.OnMinimizClicked += MainWindow_OnMinimizClicked;

            bsManager = BehaviorScriptManager.Instance();
            bsManager.CurrentLoader.OnRuntimeErrorArised += CurrentLoader_OnRuntimeErrorArised;

            controllerManager = SequenceControllerManager.Instance;
            controllerManager.SetController(GlobalSetting.SelectedController);
        }

        private void RegisteringCommands()
        {
            this.CommandBindings.Add(new CommandBinding(MediaCommands.TogglePlayPause, (sender, evt_args) =>
            {
                ControlPanel.TriggerPlayPause();
            }));

            this.CommandBindings.Add(new CommandBinding(MediaCommands.NextTrack, (sender, evt_args) =>
            {
                ControlPanel_OnTrigging(AudioPanelTrigger.SkipNext, null);
            }));

            this.CommandBindings.Add(new CommandBinding(MediaCommands.PreviousTrack, (sender, evt_args) =>
            {
                ControlPanel_OnTrigging(AudioPanelTrigger.SkipPrev, null);
            }));
        }

        private void CurrentLoader_OnRuntimeErrorArised(Exception obj)
        {
            RuntimeException runtimeError = obj as RuntimeException;
            if (runtimeError == null)
            {
                return;
            }
            string body = i18NConvertor.ConvertTo(SupportedPages.CORE_FUNC, runtimeError.Message, runtimeError.Arguements);
            string caption = i18NConvertor.ConvertTo(SupportedPages.CORE_FUNC, "CORE_LBS_RT");
            new CommonDialog(caption, body, MessageBoxButton.OK).ShowDialog();
        }

        private void ControlPanel_OnModeChange(PlayMode mode, object append)
        {
            core.MusicPlayMode = mode;
            switch(mode)
            {
                case PlayMode.RepeatList:
                    DesktopDisplay.ShowToast(LinearMode, 5);
                    break;
                case PlayMode.RepeatOne:
                    DesktopDisplay.ShowToast(SingleLoop, 5);
                    break;
                case PlayMode.Shuffle:
                    DesktopDisplay.ShowToast(ShuffleMode, 5);
                    break;
            }
        }

        private void CATALOGUES_OnConfigClicked()
        {
            //TestPage tp = new TestPage();
            //tp.Show();
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void CATALOGUES_OnMenuButtonClicked()
        {
            if (CATALOGUES.Width < sliderSize)
            {
                CATALOGUES.BeginAnimation(WidthProperty, ExpandPanel);
            }
            else
            {
                CATALOGUES.BeginAnimation(WidthProperty, MinimizePanel);
            }
        }

        private void MainWindow_OnMinimizClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ControlPanel_OnProfilePictureClicked(bool isPanelOpen)
        {
            if(core.AudioOut.Playing && !isPanelOpen)
            {
                musicDetailPage.musicEntity = core.CurrentPlaying;
                musicDetailPage.source = ControlPanel.AlbumProfile;
                musicDetailPage.FlushChanges();
                FPresentor.ShowContent(musicDetailPage);
            }
            else if(isPanelOpen)
            {
                FPresentor.BackWard();
            }
        }

        private void MusicList_OnSelectedMusicChange(MusicEntity arg1, object arg2)
        {
            if (core.CurrentPlaying == arg1) return;
            core.SetCatalogue(musicList.SelectedCatalogue);
            core.PrepareMusic(arg1);
        }

        private void Showcase_CatalogueSelected(Catalogue obj)
        {
            if (obj == null) return;
            FPresentor.ShowContent(musicList,false, () => musicList.SetCatalogue(obj));
        }

        /// <summary>
        /// 侧边栏音乐归类选择列表选项变化回调事件
        /// </summary>
        /// <param name="selected">选择的归类</param>
        /// <param name="tag">附加参数</param>
        private void CATALOGUES_OnSelectionChange(CatalogueSections tag)
        {
            CatalogueSections TAG = tag;
            switch (TAG)
            {
                case CatalogueSections.BY_LOCATION:
                    FPresentor.ShowContent(showcase, false, () => showcase.SetCatalogues(ByLocation));
                    break;
                case CatalogueSections.USER_PLAYLISTS:
                    FPresentor.ShowContent(showcase, false, () => showcase.SetCatalogues(ByUserDefined));
                    break;
                case CatalogueSections.ALBUM_COLLECTIONS:
                    FPresentor.ShowContent(showcase, false, () => showcase.SetCatalogues(ByAlbum));
                    break;
                case CatalogueSections.ARTIST_COLLECTIONS:
                    FPresentor.ShowContent(showcase, false, () => showcase.SetCatalogues(ByArtist));
                    break;
                case CatalogueSections.INTERNET_MUSIC:
                    FPresentor.ShowContent(internetMusicPage);
                    break;
            }
        }

        /// <summary>
        /// 音量更改回调事件
        /// </summary>
        /// <param name="value">修改的音量</param>
        private void ControlPanel_OnVolumeChanged(double value)
        {
            core.CurrentMusicVolume = (float)value;
            GlobalSetting.Volume = core.CurrentMusicVolume;
        }

        /// <summary>
        /// 进度非自然更改回调事件
        /// </summary>
        /// <param name="value"></param>
        private void ControlPanel_OnProgressChanged(double value)
        {
            core.PositionMoveTo(value);
        }

        bool desktopEnable = true;
        private string update_ok_title;
        private string update_ok_content;

        /// <summary>
        /// 音乐控制面板开关选项发生状态改变触发事件
        /// </summary>
        /// <param name="identifier">改变的开关选项</param>
        /// <param name="args">附加参数</param>
        private void ControlPanel_OnTrigging(AudioPanelTrigger identifier, object args)
        {
            if (!core.AudioOut.isLoaded && identifier != AudioPanelTrigger.LBScript)
            {
                if (core.currentCatalogue == null && musicList.SelectedCatalogue != null)
                {
                    core.SetCatalogue(musicList.SelectedCatalogue);
                }
                else return;
            }
            switch (identifier)
            {
                case AudioPanelTrigger.PausePlay:
                    bool isPaused = (bool)args;
                    if(core.CurrentPlaying==null)
                    {
                        core.GetNext();
                    }
                    else
                    {
                        if (isPaused) core.Pause();
                        else core.Resume();
                    }
                    break;
                case AudioPanelTrigger.SkipNext:
                    core.GetNext();
                    break;
                case AudioPanelTrigger.SkipPrev:
                    core.GetPrevious();
                    break;
                case AudioPanelTrigger.Lyric:
                    Bus.Unicast(
                        GlobalSetting.LyricEnabled ?
                            EventBusTypes.ON_ACTION_REQ_DISABLE :
                            EventBusTypes.ON_ACTION_REQ_ENABLE,
                        typeof(DesktopDisplay), "lyric");
                    GlobalSetting.LyricEnabled = !GlobalSetting.LyricEnabled;
                    break;
                case AudioPanelTrigger.Spectrum:
                    Bus.Unicast(
                        desktopEnable ?
                            EventBusTypes.ON_ACTION_REQ_DISABLE :
                            EventBusTypes.ON_ACTION_REQ_ENABLE,
                        typeof(DesktopDisplay), "fft");
                    desktopEnable = !desktopEnable;
                    break;
                case AudioPanelTrigger.Equalizer:
                    MyEqualizer myEqualizer = new MyEqualizer();
                    myEqualizer.Show();
                    break;
                case AudioPanelTrigger.LBScript:
                    LpsScriptLoader scriptLoader = new LpsScriptLoader();
                    scriptLoader.ShowDialog();
                    break;
                case AudioPanelTrigger.Volume:
                    ControlPanel.Volume = core.CurrentMusicVolume;
                    break;
            }
        }

        /// <summary>
        /// 当音乐播放进度更改时（由<see cref="LpsAudio"/>.CountTimerDelegate定时触发）
        /// </summary>
        /// <param name="curPos"></param>
        private void NotifyChanged(TimeSpan curPos)
        {
            Dispatcher.Invoke(() =>
            {
                ControlPanel.Value = curPos.TotalSeconds;
                ControlPanel.Current = curPos;
                if(DesktopDisplayIndicator==null)
                {
                    desktopDisplay.Show();
                }
            });
        }

        /// <summary>
        /// 告知当音乐已经准备好播放（加载完成）
        /// </summary>
        /// <param name="Music">音乐实体</param>
        /// <param name="mTrack">音轨信息</param>
        private void MusicPerpeared(MusicEntity Music, Track mTrack)
        {
            Dispatcher.Invoke(() =>
            {
                if (GlobalSetting.ShowNextSongHint)
                    DesktopDisplay.ShowToast(NextSongHint.FormateEx(Music.MusicName), 4000);
                BitmapSource source;
                ControlPanel.AlbumProfile = (source = MediaMetaDataReader.GetPicture(Music)) == null ? null : new ImageBrush(source);
                if (GlobalSetting.ThemeColorFollowAlbum)
                {
                    if (source == null)
                    {
                        themeManager.Restore();
                    }
                    else
                    {
                        bitmapAnalyser.CalcHighestContrastColor(Graphic.BitmapSource2Bitmap(source));
                        ThemeTuple themeTuple = new ThemeTuple(
                            bitmapAnalyser.Foreground.ToBrush(),
                            bitmapAnalyser.Background.ToBrush(),
                            bitmapAnalyser.Intermediate.ToBrush()
                        );
                        themeManager.Reflush(themeTuple);
                    }
                }
                ControlPanel.StartPlaying();
                ControlPanel.MaxValue = mTrack.Duration.TotalSeconds;
                ControlPanel.Value = 0;
                if (musicList.SelectedCatalogue != null)
                {
                    musicList.PlayingIndex = musicList.SelectedCatalogue.CurrentIndex;
                }
                ControlPanel.CurrentMusic = Music.Artist[0] +" - "+ Music.MusicName;
                ControlPanel.TotalLength = mTrack.Duration;
                AudioDelegations.InvokeLyricUpdate(null);

                Bus.Unicast(EventBusTypes.ON_ACTION_UPDATE, typeof(MusicDetail), Music, ControlPanel.AlbumProfile);
            });
        }

        /// <summary>
        /// 歌曲目录选项更改事件，当用户人为选定歌曲时触发
        /// </summary>
        /// <param name="selected">选择的音乐实体</param>
        /// <param name="tag">附加信息</param>
        private void DipMusic_ItemSelectionChanged(MusicEntity selected, object tag)
        {
#region disposed
            //if (dia == null)
            //{
            //    dia = new Dialogue(new _3DVisualize(), "3D");
            //    dia.Show();
            //}
#endregion
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CATALOGUES.SelectedIndex = -1;
            LunalipseLogger.GetLogger().Info("Start Crossing Window");
            desktopDisplay.Show();
            LunalipseLogger.GetLogger().Info("Loaded complete, rendering UI");
#if BUILD
            this.SetVersion(versionHelper.getGenerationTypedVersion(LunalipseGeneration.Build));
#elif ALPHA
            this.SetVersion(versionHelper.getGenerationTypedVersion(LunalipseGeneration.Alpha));
#elif BETA
            this.SetVersion(versionHelper.getGenerationTypedVersion(LunalipseGeneration.Beta));
#elif RELEASE
            this.SetVersion(versionHelper.getGenerationTypedVersion(LunalipseGeneration.Release));
#endif
            RunVersionCheck();
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (GlobalSetting.UpdateArguments != string.Empty)
            {
                LunalipseLogger.GetLogger().Info("Initiating Upgrade installation");
                ProcessStartInfo info = new ProcessStartInfo("CeliUpdater.exe", GlobalSetting.UpdateArguments);
                info.WorkingDirectory = BasePath;
                Process.Start(info);
                GlobalSetting.UpdateArguments = string.Empty;
            }
            desktopDisplay?.Close();
            
            LunalipseLogger.GetLogger().Debug("Saving Playlist");
            playlistGuard.SavePlaylist();
            playlistGuard.SaveMusicCache();

            LunalipseLogger.GetLogger().Info("Terminating Lunalipse.....");
            core.Dispose();
            LunalipseLogger.GetLogger().Release();
            Environment.Exit(0);
        }

        private void LunalipseMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
            core.Pause();
        }

        void RunVersionCheck()
        {
            UpdateHelper updateHelper = new UpdateHelper();
            updateHelper.OnQueryCompleted += () =>
            {
                ReleaseInfo releaseInfo = updateHelper.UpdateAvailability();
                if (releaseInfo != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        CommonDialog update_ava = new CommonDialog(update_ok_title, update_ok_content.FormateEx(releaseInfo.Tag), MessageBoxButton.OK);
                        update_ava.ShowDialog();
                    });
                }
            };
            updateHelper.QueryLatestUpdate();
        }

        private void SaveSettings()
        {
            bool hasChange = false;
            foreach(KeyValuePair<string,SpectrumDisplayer> disp in vManager.Displayers)
            {
                hasChange = false;
                if (GlobalSetting.SpectrumDisplayers.ContainsKey(disp.Key))
                {
                    SpectrumDisplayCfg spectrumDisplayCfg = GlobalSetting.SpectrumDisplayers[disp.Key];
                    if (spectrumDisplayCfg.Resolution != disp.Value.DesireResolution)
                    {
                        hasChange = true;
                        spectrumDisplayCfg.Resolution = disp.Value.DesireResolution;
                    }
                    if (spectrumDisplayCfg.Style != disp.Value.currentStyle)
                    {
                        hasChange = true;
                        spectrumDisplayCfg.Style = disp.Value.currentStyle;
                    }
                    if(hasChange)
                    {
                        GlobalSetting.SpectrumDisplayers[disp.Key] = spectrumDisplayCfg;
                    }
                }
                else
                {
                    GlobalSetting.SpectrumDisplayers.Add(disp.Key, new SpectrumDisplayCfg()
                    {
                        Resolution = disp.Value.DesireResolution,
                        Style = disp.Value.currentStyle
                    });
                }
            }
            if (SettingHash != GLS.INSTANCE.ComputeHash())
            {
                CommonDialog UserShouldSave = new CommonDialog(SaveSettingTitle, SaveSettingContent, System.Windows.MessageBoxButton.OKCancel);
                if (UserShouldSave.ShowDialog().Value)
                {
                    globalSettingHelper.SaveSetting(GLS.INSTANCE);
                }
            }
        }

        protected override void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            base.ThemeManagerBase_OnThemeApplying(obj);
        }

        /// <summary>
        /// 播放完成时的回调方法
        /// </summary>
        private void PlayFinished()
        {
            //Dispatcher.Invoke(() => dipMusic.SelectedIndex = core.getCurrentPlayingIndex);
        }
    }
}
