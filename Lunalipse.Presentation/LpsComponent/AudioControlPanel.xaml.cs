using Lunalipse.Common.Generic.AudioControlPanel;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Presentation.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// AudioControlPanel.xaml 的交互逻辑
    /// </summary>
    public partial class AudioControlPanel : UserControl
    {
        bool isPaused = false;
        bool isFullscreen = false;
        bool LyricEnabled = true;
        bool SpectrumEnabled = true;
        PlayMode Mode = PlayMode.RepeatList;
        Duration elapseTime = new Duration(TimeSpan.FromMilliseconds(100));

        /// <summary>
        /// 开关类事件触发，对于<see cref="AudioPanelTrigger.PausePlay"/>事件，<see cref="object"/>为<see cref="bool"/>，表示是否已暂停。
        /// </summary>
        public event Action<AudioPanelTrigger, object> OnTrigging;
        public event Action<PlayMode, object> OnModeChange;
        public event ProgressChange OnProgressChanged;
        public event ProgressChange OnVolumeChanged;
        public event Action OnProfilePictureClicked;
        private DoubleAnimation FadeIn,FadeOut;

        private const string UI_COMPONENT_THEME_UID = "PR_COMP_AudioControlPanel";

        /* TODO 新的架构调整：
         * 音乐播放顺序现在由该类掌管。
         * 在专辑页面中选中一个专辑，相应的catalogue类将会传递到此类的ICatalogue字段，用于进行播放。
         * 当专辑改变时，catalogue类改变，播放新的catalogue。
         * 如用户使用LPScript时，通过Script组建一个catalogue。
         */
        //private ICatalogue SelectedCatalogue = null;
        //private bool isUsingScript = false;
        //// object是MusicEntity
        //private Func<object> ScriptSteppingFunc;

        public AudioControlPanel()
        {
            InitializeComponent();

            FadeIn = new DoubleAnimation(0, 1, elapseTime);
            FadeOut = new DoubleAnimation(1, 0, elapseTime);
            FadeOut.Completed += (a, b) => VolumePlanePopup.IsOpen = false;

            MusicProgress.OnProgressChanged += x => OnProgressChanged?.Invoke(x);
            VolumeBar.OnValueChanged += x =>
            {
                if (x >= 25 && x < 75)
                {
                    VolumeAdj.Content = FindResource("Volume_025");
                }
                else if (x >= 75)
                {
                    VolumeAdj.Content = FindResource("Volume_075");
                }
                else if (x >= 1)
                {
                    VolumeAdj.Content = FindResource("Volume_0");
                }
                else
                {
                    VolumeAdj.Content = FindResource("Volume_off");
                }
                OnVolumeChanged?.Invoke(x);
            };
            VolumeAdj.Click += VolumeAdj_Click;
            VolumePlanePopup.MouseLeave += VolumePlanePopup_MouseLeave;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void VolumePlanePopup_MouseLeave(object sender, MouseEventArgs e)
        {
            VolumeBar.BeginAnimation(OpacityProperty, FadeOut);
        }

        private void VolumeAdj_Click(object sender, RoutedEventArgs e)
        {
            if (VolumePlanePopup.IsOpen)
            {
                VolumeBar.BeginAnimation(OpacityProperty, FadeOut);
            }
            else
            {
                VolumePlanePopup.IsOpen = true;
                VolumeBar.BeginAnimation(OpacityProperty, FadeIn);
            }
        }

        public Brush AlbumProfile
        {
            get => AlbProfile.Background;
            set
            {
                AlbProfile.Background = value;
                if (value == null)
                {
                    FallBackPic.Visibility = Visibility.Visible;
                }
                else
                {
                    FallBackPic.Visibility = Visibility.Hidden;
                }
            }
        }

        public double MaxValue
        {
            get => MusicProgress.MaxValue;
            set => MusicProgress.MaxValue = value;
        }
        public double Value
        {
            get => MusicProgress.Value;
            set => MusicProgress.Value = value;
        }

        public TimeSpan Current
        {
            set
            {
                Time.Content = value.ToString(@"hh\:mm\:ss");
            }
        }

        public TimeSpan TotalLength
        {
            set
            {
                TotalTime.Content = value.ToString(@"hh\:mm\:ss");
            }
        }

        public string CurrentMusic
        {
            get
            {
                return CurrentPlaying.Text;
            }
            set
            {
                CurrentPlaying.Text = value;
            }
        }

        private void SkipToPrevious(object sender, RoutedEventArgs e)
        {
            OnTrigging?.Invoke(AudioPanelTrigger.SkipPrev, null);
        }

        public void StartPlaying()
        {
            Play.Content = FindResource("Pause");
            isPaused = false;
        }

        private void PlayOrPause(object sender, RoutedEventArgs e)
        {
            Button bsender = sender as Button;
            if (isPaused)
            {
                bsender.Content = FindResource("Pause");
                isPaused = false;
            }
            else
            {
                bsender.Content = FindResource("Play");
                isPaused = true;
            }
            OnTrigging?.Invoke(AudioPanelTrigger.PausePlay, isPaused);
        }

        private void SkipToNext(object sender, RoutedEventArgs e)
        {
            OnTrigging?.Invoke(AudioPanelTrigger.SkipNext, null);
        }

        private void ChangePlayMode(object sender, RoutedEventArgs e)
        {
            Button bsender = sender as Button;
            switch (Mode)
            {
                case PlayMode.RepeatList:
                    Mode = PlayMode.RepeatOne;
                    bsender.Content = FindResource("RepeatOne");
                    break;
                case PlayMode.RepeatOne:
                    Mode = PlayMode.Shuffle;
                    bsender.Content = FindResource("Shuffle");
                    break;
                case PlayMode.Shuffle:
                    Mode = PlayMode.RepeatList;
                    bsender.Content = FindResource("RepeatList");
                    break;
            }
            OnModeChange?.Invoke(Mode, null);
        }

        private void LBScriptEnable(object sender, RoutedEventArgs e)
        {
            OnTrigging?.Invoke(AudioPanelTrigger.LBScript, null);
        }
        private void OpenEqualizer(object sender, RoutedEventArgs e)
        {
            OnTrigging?.Invoke(AudioPanelTrigger.Equalizer, null);
        }

        private void TriggerSpectrum(object sender, RoutedEventArgs e)
        {
            if (SpectrumEnabled)
            {
                SpectrumDisable.Visibility = Visibility.Visible;
                SpectrumEnabled = false;
            }
            else
            {
                SpectrumDisable.Visibility = Visibility.Hidden;
                SpectrumEnabled = true;
            }
            OnTrigging?.Invoke(AudioPanelTrigger.Spectrum, null);
        }
        private void TriggerLyric(object sender, RoutedEventArgs e)
        {
            if (LyricEnabled)
            {
                LyricDisable.Visibility = Visibility.Visible;
                LyricEnabled = false;
            }
            else
            {
                LyricDisable.Visibility = Visibility.Hidden;
                LyricEnabled = true;
            }
            OnTrigging?.Invoke(AudioPanelTrigger.Lyric, null);
        }
        private void TriggerFullScreen(object sender, RoutedEventArgs e)
        {
            Button bsender = sender as Button;
            if (isFullscreen)
            {
                bsender.Content = FindResource("FullScreen");
                isFullscreen = false;
            }
            else
            {
                bsender.Content = FindResource("ExitFullScreen");
                isFullscreen = true;
            }
            OnTrigging?.Invoke(AudioPanelTrigger.FullScreen, null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            VolumeBar.MaxValue = 100;
            VolumeBar.Value = 70;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnProfilePictureClicked?.Invoke();
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            this.Foreground = obj.Foreground;
            MusicProgress.BarColor = obj.Secondary;
            MusicProgress.TrackColor = obj.Primary.SetOpacity(0.7).ToLuna();

            VolumeBar.BarTrackColor = obj.Primary.SetOpacity(0.7).ToLuna();
            VolumeBar.BarColor = obj.Secondary;
            VolumeBar.FontColor = obj.Foreground;
            VolumeBar.Background = obj.Primary.ToLuna();
        }
    }
}
