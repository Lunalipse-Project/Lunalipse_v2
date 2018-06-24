using System;
using System.Windows;
using System.Windows.Input;
using Lunalipse.Core.PlayList;
using Lunalipse.Core.Metadata;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Common.Data;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Presentation.LpsWindow;
using System.Windows.Threading;
using System.Collections.Generic;
using Lunalipse.Common.Generic.AudioControlPanel;
using System.Windows.Media;
using Lunalipse.I18N;
using Lunalipse.Core.I18N;
using System.Windows.Media.Imaging;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Common.Generic.Catalogue;
using Lunalipse.Presentation.Utils;
using Lunalipse.Core.Cache;
using Lunalipse.Common.Generic.Cache;

namespace Lunalipse
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// （测试界面）
    /// </summary>
    public partial class MainWindow : LunalipseMainWindow
    {
        MusicListPool mlp;
        CataloguePool CPOOL;
        MediaMetaDataReader mmdr;
        I18NConvertor converter;
        LpsAudio laudio;
        Interpreter intp;
        Dialogue dia;
        CacheHub cacheSystem;
        public MainWindow() : base()
        {
            InitializeComponent();
            InitializeModules();
        }

        private void DoTranslate()
        {
            CATALOGUES.Translate(converter);
            dipMusic.Translate(converter);
        }

        /// <summary>
        /// 初始化Lunalipse运行时必需组件
        /// </summary>
        private void InitializeModules()
        {
            mlp = MusicListPool.INSATNCE;
            CPOOL = CataloguePool.INSATNCE;
            laudio = LpsAudio.INSTANCE();
            cacheSystem = CacheHub.INSTANCE(Environment.CurrentDirectory);
            converter = I18NConvertor.INSTANCE(I18NPages.INSTANCE);
            //mmdr = new MediaMetaDataReader(converter);
            //mlp.AddToPool("F:/M2", mmdr);

            //intp = Interpreter.INSTANCE(@"F:\Lunalipse\TestUnit\bin\Debug");
            //if (intp.Load("prg2"))
            //{
            //    PlayFinished();
            //}
            //alb.Source = mlp.ToCatalogue().GetCatalogueCover();
            AudioDelegations.PlayingFinished += PlayFinished;
            AudioDelegations.MusicLoaded += MusicPerpeared;
            dipMusic.ItemSelectionChanged += DipMusic_ItemSelectionChanged;
            ControlPanel.OnTrigging += ControlPanel_OnTrigging;
            AudioDelegations.PostionChanged += NotifyChanged;
            ControlPanel.Value = 0;
            laudio.Volume = (float)ControlPanel.Value;
            ControlPanel.OnProgressChanged += ControlPanel_OnProgressChanged;
            ControlPanel.OnVolumeChanged += ControlPanel_OnVolumeChanged;
            CATALOGUES.OnSelectionChange += CATALOGUES_OnSelectionChange;
            CATALOGUES.TheMainCatalogue = mlp.ToCatalogue();
        }

        /// <summary>
        /// 侧边栏音乐归类选择列表选项变化回调事件
        /// </summary>
        /// <param name="selected">选择的归类</param>
        /// <param name="tag">附加参数</param>
        private void CATALOGUES_OnSelectionChange(ICatalogue selected, object tag)
        {
            Catalogue cat = selected as Catalogue;
            CatalogueSections TAG = (CatalogueSections)tag;
            switch (TAG)
            {
                case CatalogueSections.ALL_MUSIC:
                    dipMusic.AsyncExecute(() =>
                    {
                        dipMusic.Catalogue = cat;
                    });
                    break;
                case CatalogueSections.INDIVIDUAL:
                    dipMusic.Clear();
                    dipMusic.AsyncExecute(() =>
                    {
                        dipMusic.Catalogue = cat;
                    });
                    break;
                case CatalogueSections.USER_PLAYLISTS:
                    CATALOGUES.EmptyContent();
                    break;
                case CatalogueSections.ALBUM_COLLECTIONS:
                    CATALOGUES.Reset();
                    List<Catalogue> catas = CPOOL.GetAlbumClassfied();
                    if (catas.Count == 0)
                    {
                        CATALOGUES.EmptyContent();
                    }
                    else
                    {
                        foreach (Catalogue c in catas)
                            CATALOGUES.Add(c);
                    }
                    break;
                case CatalogueSections.ARTIST_COLLECTIONS:
                    CATALOGUES.Reset();
                    List<Catalogue> art_catas = CPOOL.GetArtistClassfied();
                    if (art_catas.Count == 0)
                    {
                        CATALOGUES.EmptyContent();
                    }
                    else
                    {
                        foreach (Catalogue c in art_catas)
                            CATALOGUES.Add(c);
                    }
                    break;
            }
        }

        /// <summary>
        /// 音量更改回调事件
        /// </summary>
        /// <param name="value">修改的音量</param>
        private void ControlPanel_OnVolumeChanged(double value)
        {
            laudio.Volume = (float)value;
        }

        /// <summary>
        /// 进度非自然更改回调事件
        /// </summary>
        /// <param name="value"></param>
        private void ControlPanel_OnProgressChanged(double value)
        {
            laudio.MoveTo(value);
        }

        /// <summary>
        /// 音乐控制面板开关选项发生状态改变触发事件
        /// </summary>
        /// <param name="identifier">改变的开关选项</param>
        /// <param name="args">附加参数</param>
        private void ControlPanel_OnTrigging(AudioPanelTrigger identifier, object args)
        {
            switch (identifier)
            {
                case AudioPanelTrigger.PausePlay:
                    bool isPaused = (bool)args;
                    if (laudio.isLoaded)
                    {
                        if (isPaused) laudio.Pause();
                        else laudio.Resume();
                    }
                    break;
            }
        }

        /// <summary>
        /// 当音乐播放进度更改时（由<see cref="LpsAudio"/>的CountTimerDelegate方法定时触发
        /// </summary>
        /// <param name="curPos"></param>
        private void NotifyChanged(TimeSpan curPos)
        {
            Dispatcher.Invoke(()=>
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
                ControlPanel.MaxValue = mTrack.Duration.TotalSeconds;
                ControlPanel.Value = 0;
            });
        }

        /// <summary>
        /// 歌曲目录选项更改事件，当用户人为选定歌曲时触发
        /// </summary>
        /// <param name="selected">选择的音乐实体</param>
        /// <param name="tag">附加信息</param>
        private void DipMusic_ItemSelectionChanged(MusicEntity selected, object tag)
        {
            if (laudio.Playing) laudio.Stop();
            BitmapSource source;
            ControlPanel.AlbumProfile = (source = MediaMetaDataReader.GetPicture(selected.Path)) == null ? null : new ImageBrush(source);
            laudio.Load(selected);
            ControlPanel.StartPlaying();
            laudio.Play();
            //if (dia == null)
            //{
            //    dia = new Dialogue(new _3DVisualize(), "3D");
            //    dia.Show();
            //}
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.EnableBlur();
            DoTranslate();
            CATALOGUES.SelectedIndex = -1;
            //cacheSystem.CacheObject(CPOOL, CacheType.MUSIC_CATALOGUE_CACHE);
            dipMusic.AsyncExecute(() =>
            {
                mlp.CreateAlbumClasses();
                mlp.CreateArtistClasses();
                dipMusic.Catalogue = mlp.ToCatalogue();
            });
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            laudio.Dispose();
        }

        /// <summary>
        /// 播放完成时的回调方法
        /// </summary>
        private void PlayFinished()
        {
            MusicEntity MEnt = null;
            if (intp.LBSLoaded)
                MEnt = intp.Stepping();
            else
            {
                Next();
            }
        }

        /// <summary>
        /// 列表自动递增
        /// </summary>
        private void Next()
        {
            Dispatcher.Invoke(() => dipMusic.SelectedIndex++);
        }

        private void EventTrigger_MouseEnter(object sender, MouseEventArgs e)
        {

        }
    }
}
