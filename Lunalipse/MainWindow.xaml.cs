using System;
using System.Windows;
using System.Windows.Input;
using Lunalipse.Core.PlayList;
using Lunalipse.Core.Metadata;
using Lunalipse.Common.Data;
using Lunalipse.Presentation.LpsWindow;
using System.Windows.Threading;
using System.Collections.Generic;
using Lunalipse.Common.Generic.AudioControlPanel;
using System.Windows.Media;
using Lunalipse.I18N;
using Lunalipse.Core.I18N;
using System.Windows.Media.Imaging;
using Lunalipse.Common.Generic.Catalogue;
using Lunalipse.Core.Cache;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Core;
using Lunalipse.Pages;
using System.Windows.Media.Animation;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Lunalipse.Windows;
using Lunalipse.Common.Interfaces.II18N;

namespace Lunalipse
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// （测试界面）
    /// </summary>
    public partial class MainWindow : LunalipseMainWindow, ITranslatable
    {
        const int sliderSize = 200;

        MusicListPool mlp;
        CataloguePool CPOOL;
        MediaMetaDataReader mmdr;
        Dialogue dia;
        CacheHub cacheSystem;

        LpsCore core;

        CatalogueShowCase showcase;
        MusicSelected musicList;

        Duration elapseTime = new Duration(TimeSpan.FromMilliseconds(250));
        DoubleAnimation ExpandPanel;
        DoubleAnimation MinimizePanel;

        List<Catalogue> ByLocation, ByArtist, ByAlbum;

        public MainWindow() : base()
        {
            InitializeComponent();
            InitializeModules();
            ExpandPanel = new DoubleAnimation(48, sliderSize, elapseTime);
            MinimizePanel = new DoubleAnimation(sliderSize, 48, elapseTime);

            Task.Factory.StartNew(() =>
            {
                ByLocation = CPOOL.GetLocationClassified();
                ByAlbum = CPOOL.GetAlbumClassfied();
                ByArtist = CPOOL.GetArtistClassfied();
            });
            TranslationManager.OnI18NEnvironmentChanged += Translate;
            Translate(TranslationManager.AquireConverter());
        }

        public void Translate(II18NConvertor converter)
        {
            CATALOGUES.Translate(converter);
        }

        /// <summary>
        /// 初始化Lunalipse运行时必需组件
        /// </summary>
        private void InitializeModules()
        {          
            CPOOL = CataloguePool.INSATNCE;
            core = LpsCore.Session();
            cacheSystem = CacheHub.INSTANCE(Environment.CurrentDirectory);
            mlp = MusicListPool.INSATNCE(mmdr = new MediaMetaDataReader());

            mlp.AddToPool(@"F:/M2");
            mlp.CreateAlbumClasses();
            mlp.CreateArtistClasses();

            #region duplicated
            //mmdr = new MediaMetaDataReader(converter);
            //mlp.AddToPool("F:/M2", mmdr);

            //intp = Interpreter.INSTANCE(@"F:\Lunalipse\TestUnit\bin\Debug");
            //if (intp.Load("prg2"))
            //{
            //    PlayFinished();
            //}
            //alb.Source = mlp.ToCatalogue().GetCatalogueCover();
            #endregion

            core.OnMusicComplete += PlayFinished;
            core.OnMusicPrepared += MusicPerpeared;
            core.OnMusicProgressChanged += NotifyChanged;

            ControlPanel.Value = 0;
            core.CurrentMusicVolume = 70;

            ControlPanel.OnProgressChanged += ControlPanel_OnProgressChanged;
            ControlPanel.OnVolumeChanged += ControlPanel_OnVolumeChanged;
            ControlPanel.OnTrigging += ControlPanel_OnTrigging;
            ControlPanel.OnProfilePictureClicked += ControlPanel_OnProfilePictureClicked;
            ControlPanel.OnModeChange += (mode, @object) => core.MusicPlayMode = mode;

            CATALOGUES.OnSelectionChange += CATALOGUES_OnSelectionChange;
            CATALOGUES.OnMenuButtonClicked += CATALOGUES_OnMenuButtonClicked;
            CATALOGUES.OnConfigClicked += CATALOGUES_OnConfigClicked;

            showcase = new CatalogueShowCase();
            showcase.CatalogueSelected += Showcase_CatalogueSelected;

            musicList = new MusicSelected();
            musicList.OnSelectedMusicChange += MusicList_OnSelectedMusicChange;

            this.OnMinimizClicked += MainWindow_OnMinimizClicked;
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

        private void ControlPanel_OnProfilePictureClicked()
        {
            if (musicList.SelectedCatalogue != null)
            {
                musicList.SetCatalogue(core.currentCatalogue, true);
                FPresentor.ShowContent(musicList);
            }
        }

        private void MusicList_OnSelectedMusicChange(MusicEntity arg1, object arg2)
        {
            if (core.CurrentPlaying == arg1) return;
            core.SetCatalogue(musicList.SelectedCatalogue);
            core.PerpareMusic(arg1);
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
                    FPresentor.ShowContent(showcase);
                    break;
                case CatalogueSections.ALBUM_COLLECTIONS:
                    FPresentor.ShowContent(showcase, false, () => showcase.SetCatalogues(ByAlbum));
                    break;
                case CatalogueSections.ARTIST_COLLECTIONS:
                    FPresentor.ShowContent(showcase, false, () => showcase.SetCatalogues(ByArtist));
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
        }

        /// <summary>
        /// 进度非自然更改回调事件
        /// </summary>
        /// <param name="value"></param>
        private void ControlPanel_OnProgressChanged(double value)
        {
            core.PositionMoveTo(value);
        }

        /// <summary>
        /// 音乐控制面板开关选项发生状态改变触发事件
        /// </summary>
        /// <param name="identifier">改变的开关选项</param>
        /// <param name="args">附加参数</param>
        private void ControlPanel_OnTrigging(AudioPanelTrigger identifier, object args)
        {
            if (!core.AudioOut.isLoaded) return;
            switch (identifier)
            {
                case AudioPanelTrigger.PausePlay:
                    bool isPaused = (bool)args;
                    if (isPaused) core.Pause();
                    else core.Resume();
                    break;
                case AudioPanelTrigger.SkipNext:
                    core.PerpareMusic(core.currentCatalogue.getNext());
                    break;
                case AudioPanelTrigger.SkipPrev:
                    core.PerpareMusic(core.currentCatalogue.getPrevious());
                    break;
            }
        }

        /// <summary>
        /// 当音乐播放进度更改时（由<see cref="LpsAudio"/>的CountTimerDelegate方法定时触发
        /// </summary>
        /// <param name="curPos"></param>
        private void NotifyChanged(TimeSpan curPos)
        {
            Dispatcher.Invoke(() =>
            {
                ControlPanel.Value = curPos.TotalSeconds;
                ControlPanel.Current = curPos;
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
                BitmapSource source;
                ControlPanel.AlbumProfile = (source = MediaMetaDataReader.GetPicture(Music.Path)) == null ? null : new ImageBrush(source);
                ControlPanel.StartPlaying();
                ControlPanel.MaxValue = mTrack.Duration.TotalSeconds;
                ControlPanel.Value = 0;
                musicList.PlayingIndex = musicList.SelectedCatalogue.CurrentIndex;
                ControlPanel.CurrentMusic = Music.Artist[0] +" - "+ Music.Name;
                ControlPanel.TotalLength = mTrack.Duration;
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
            LunalipseLogger.GetLogger().Info("Loaded complete, rendering UI");
            //cacheSystem.CacheObject(CPOOL, CacheType.MUSIC_CATALOGUE_CACHE);
            //dipMusic.AsyncExecute(() =>
            //{
            //    
            //    dipMusic.Catalogue = mlp.ToCatalogue();
            //});
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void Window_Closed(object sender, EventArgs e)
        {
            LunalipseLogger.GetLogger().Info("Terminating Lunalipse.....");
            core.Dispose();
            LunalipseLogger.GetLogger().Release();
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
