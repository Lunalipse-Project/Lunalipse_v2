using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Core.Lyric;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Lunalipse.Pages
{
    /// <summary>
    /// MusicDetail.xaml 的交互逻辑
    /// </summary>
    public partial class MusicDetail : Page
    {
        II18NConvertor converter;

        public MusicEntity musicEntity;
        public Brush source;

        string Artist = "",Album = "";
        string LyricNotFoundHint = "";

        EventBus eventBus;

        ObservableCollection<LyricToken> lyricTokens = new ObservableCollection<LyricToken>();
        ThicknessAnimation thicknessAnimation = new ThicknessAnimation();
        Thickness thickness = new Thickness(0,0,0,0);
        
        
        Duration elapse = new Duration(TimeSpan.FromMilliseconds(500));
        Brush background;

        public MusicDetail()
        {
            InitializeComponent();
            eventBus = EventBus.Instance;
            converter = TranslationManagerBase.AquireConverter();
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            

            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());

            eventBus.AddUnicastReciever(this.GetType(), MusicDetail_UnicastReciever);

            LyricScrollWall.ItemsSource = lyricTokens;

            thicknessAnimation.Duration = elapse;
            thicknessAnimation.EasingFunction = new SineEase();

            LyricEnumerator.OnLyricPrepared += LyricEnumerator_OnLyricPrepared;

            LyricEnumerator_OnLyricPrepared(LyricEnumerator.TryGetLyric());

            Loaded += MusicDetail_Loaded;
            Unloaded += MusicDetail_Unloaded;
        }

        private void MusicDetail_Unloaded(object sender, RoutedEventArgs e)
        {
            AudioDelegations.LyricUpdated -= AudioDelegations_LyricUpdated;
        }

        public MusicDetail(MusicEntity musicEntity, Brush source) : this()
        {
            this.musicEntity = musicEntity;
            this.source = source;
            FlushChanges();
        }

        public void FlushChanges()
        {
            if(musicEntity!=null)
            {
                AlbumPicture.Background = this.source ?? background;
                NoPictureFound.Visibility = source == null ? Visibility.Visible : Visibility.Hidden;
                Artist = musicEntity.Artist.Length > 0 ? musicEntity.ArtistFrist : musicEntity.DefaultArtist;
                Album = string.IsNullOrEmpty(musicEntity.Album) ? musicEntity.DefaultAlbum : musicEntity.Album;
                MusicName.Text = musicEntity.MusicName;
            }
        }

        private void MusicDetail_Loaded(object sender, RoutedEventArgs e)
        {
            AudioDelegations.LyricUpdated += AudioDelegations_LyricUpdated;
            thicknessAnimation.Duration = elapse;
        }

        bool isReady = false;
        private void LyricEnumerator_OnLyricPrepared(List<LyricToken> obj)
        {
            isReady = false;
            lyricTokens.Clear();
            thicknessAnimation.From = thickness;
            thickness.Top = 150;
            thicknessAnimation.To = thickness;
            LyricScrollWall.BeginAnimation(MarginProperty, thicknessAnimation);
            SumOfAllHeight = 0;
            lastIndex = 0;
            if (obj == null || obj.Count == 0)
            {
                lyricTokens.Add(new LyricToken(LyricNotFoundHint, TimeSpan.MaxValue));
            }
            else
            {
                foreach (LyricToken lyricToken in obj)
                {
                    lyricTokens.Add(lyricToken);
                }
                isReady = true;
                lyricTokens[0].LyricOpacityUI = 1;
            }
            LyricScrollWall.UpdateLayout();
        }

        int lastIndex = 0;
        double SumOfAllHeight = 0d;

        private void AudioDelegations_LyricUpdated(LyricToken Token)
        {
            if (isReady && Token != null)
            {
                int index = lyricTokens.IndexOf(Token);
                Dispatcher.Invoke(() =>
                {
                    OffsetHeight(lastIndex, index, ref SumOfAllHeight);
                    thicknessAnimation.From = thickness;
                    thickness.Top = 150 - SumOfAllHeight;
                    thicknessAnimation.To = thickness;
                    LyricScrollWall.BeginAnimation(MarginProperty, thicknessAnimation);
                    lyricTokens[lastIndex].LyricOpacityUI = 0.7d;
                    lyricTokens[index].LyricOpacityUI = 1;
                    lastIndex = index;
                });
            }
        }

        private void OffsetHeight(int lastIndex,int index,ref double sum)
        {
            if(lastIndex-index>0)
            {
                for(int i = lastIndex; i > index; i--)
                {
                    sum -= ((UIElement)LyricScrollWall
                                .ItemContainerGenerator
                                    .ContainerFromIndex(i))
                                        .RenderSize.Height;
                }
            }
            else
            {
                for (int i = lastIndex; i < index; i++)
                {
                    sum += ((UIElement)LyricScrollWall
                                .ItemContainerGenerator
                                    .ContainerFromIndex(i))
                                        .RenderSize.Height;
                }
            }
        }

        private void MusicDetail_UnicastReciever(EventBusTypes eventBusTypes,object[] parList)
        {
            switch (eventBusTypes)
            {
                case EventBusTypes.ON_ACTION_UPDATE:
                    musicEntity = parList[0] as MusicEntity;
                    source = parList[1] as Brush;
                    FlushChanges();
                    TranslationManagerBase_OnI18NEnvironmentChanged(converter);
                    break;
            }
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            background = obj.Primary;
            Foreground = obj.Foreground;
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(II18NConvertor obj)
        {
            MusicArtist.Text = obj.ConvertTo(SupportedPages.CORE_FUNC, Artist);
            MusicAlbum.Text = obj.ConvertTo(SupportedPages.CORE_FUNC, Album);
            LyricNotFoundHint = obj.ConvertTo(SupportedPages.CORE_MUSIC_DETAIL, "CORE_MUSIC_DETAIL_NOLYRIC");
        }

        private void LyrciScrolling_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {

        }

        ~MusicDetail()
        {
            LyricEnumerator.OnLyricPrepared -= LyricEnumerator_OnLyricPrepared;
            TranslationManagerBase.OnI18NEnvironmentChanged -= TranslationManagerBase_OnI18NEnvironmentChanged;
            ThemeManagerBase.OnThemeApplying -= ThemeManagerBase_OnThemeApplying;
            eventBus.RemoveUnicastReciever(typeof(MusicDetail));
        }
    }
}
