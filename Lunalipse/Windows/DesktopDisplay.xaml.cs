using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.IVisualization;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Core.Visualization;
using Lunalipse.Utilities;
using Lunalipse.Utilities.Win32;

namespace Lunalipse.Windows
{
    /// <summary>
    /// DesktopDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopDisplay : Window
    {
        EventBus eventBus;
        static event Action<string,int> OnToastSet;

        DoubleAnimation PreFadeOut = new DoubleAnimation(1, 1, new Duration(TimeSpan.FromSeconds(5)));
        DoubleAnimation FadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(2)));

        GLS GlobalSetting;
        VisualizationManager vManager;

        bool EnableLyricFFT = true;

        public DesktopDisplay()
        {
            InitializeComponent();
            Width = SystemParameters.PrimaryScreenWidth;
            eventBus = EventBus.Instance;
            vManager = VisualizationManager.Instance;

            eventBus.AddUnicastReciever(GetType(), ReceiveAction);
            vManager.RegisterDisplayer(FFTDrawing.Tag as string, FFTDrawing.DisplayerDelegator, 40);

            FFTDrawing.VisualManager = vManager;
            FFTDrawing.SetV11NHelper(typeof(VisualizationHelper));

            LocateWindow();
            AudioDelegations.LyricUpdated += token =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (token != null)
                        LyricDisplayArea.Content = token.Statement;
                    else
                        LyricDisplayArea.Content = "";
                });
            };
            OnToastSet += DesktopDisplay_OnToastSet;
            PreFadeOut.Completed += (sender, eventArgs) => BorderDisplay.BeginAnimation(OpacityProperty, FadeOut);

            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            this.Topmost = true;

            GlobalSetting = GLS.INSTANCE;
            GlobalSetting.OnSettingUpdated += GlobalSetting_OnSettingUpdated;

            this.Closing += DesktopDisplay_Closing;
        }

        private void DesktopDisplay_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.DesktopDisplayIndicator -= MainWindow_DesktopDisplayIndicator;
            AudioDelegations.OnFftDataUpdate -= AudioDelegations_OnFftDataUpdate;
        }

        private void GlobalSetting_OnSettingUpdated(string obj)
        {
            switch(obj)
            {
                case "LyricFontFamily":
                    LyricDisplayArea.FontFamily = GlobalSetting.LyricFontFamilyInternal;
                    break;
                case "LyricFontSize":
                    LyricDisplayArea.FontSize = GlobalSetting.LyricFontSize;
                    break;
            }
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            Foreground = obj.Foreground;
            BorderDisplay.Background = obj.Primary;
            FFTDrawing.UpdateSpectrumColor(obj.Secondary);
        }

        private void DesktopDisplay_OnToastSet(string Content, int ElapseTime)
        {
            BorderDisplay.Opacity = 1;
            Toast.Content = Content;
            PreFadeOut.Duration = new Duration(TimeSpan.FromMilliseconds(ElapseTime));
            BorderDisplay.BeginAnimation(OpacityProperty, PreFadeOut);
        }

        public static void ShowToast(string Content, int ElapseTime = 2000) => OnToastSet?.Invoke(Content, ElapseTime);

        private void ReceiveAction(EventBusTypes eventBusTypes, object[] param)
        {
            string action = param[0] as string;
            switch (eventBusTypes)
            {
                case EventBusTypes.ON_ACTION_REQ_ENABLE:
                    switch(action.ToLower())
                    {
                        case "lyric":
                            LyricDisplayArea.Visibility = Visibility.Visible;
                            break;
                        case "fft":
                            EnableLyricFFT = true;
                            break;
                    }
                    break;
                case EventBusTypes.ON_ACTION_REQ_DISABLE:
                    switch (action.ToLower())
                    {
                        case "lyric":
                            LyricDisplayArea.Visibility = Visibility.Hidden;
                            break;
                        case "fft":
                            EnableLyricFFT = false;
                            FFTDrawing.Clear();
                            break;
                    }
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowInTaskbar = false;
            this.SetThroughableWindow();
            this.HideWindowFromAltTab();
            MainWindow.DesktopDisplayIndicator += MainWindow_DesktopDisplayIndicator;
            AudioDelegations.OnFftDataUpdate += AudioDelegations_OnFftDataUpdate;
        }

        private void AudioDelegations_OnFftDataUpdate(float[] fft)
        {
            if(EnableLyricFFT)
            {
                Dispatcher.Invoke(() => FFTDrawing.DrawSpectrum(fft));
            }
        }

        private void MainWindow_DesktopDisplayIndicator()
        {
            throw new NotImplementedException();
        }

        void LocateWindow()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            Taskbar tbar = new Taskbar();
            System.Drawing.Size TBarSize = tbar.Size;
            Left = 0;
            switch (tbar.Position)
            {
                case TaskbarPosition.Bottom:
                    Top = screenHeight - Height - TBarSize.Height;
                    break;
                case TaskbarPosition.Top:
                    Top = screenHeight - Height;
                    break;
                case TaskbarPosition.Right:
                    Top = screenHeight - Width;
                    Width = screenWidth - TBarSize.Width;
                    break;
                case TaskbarPosition.Left:
                    Width = screenWidth - TBarSize.Width;
                    Left = TBarSize.Width;
                    break;
            }
        }
    }
}
