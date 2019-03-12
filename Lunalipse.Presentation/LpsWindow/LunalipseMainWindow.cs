using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.ITheme;
using Lunalipse.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Lunalipse.Presentation.LpsWindow
{
    public class LunalipseMainWindow : Window
    {
        const string UI_COMPONENT_THEME_UID = "PR_WND_LunalipseMainWindow";

        public event RoutedEventHandler OnSettingClicked;
        public event RoutedEventHandler OnMinimizClicked;

        private Label VersionNumber;

        public static readonly DependencyProperty ENABLE_BLUR =
            DependencyProperty.Register("LPSMAINWND_ENABLEBLUR",
                                        typeof(bool),
                                        typeof(LunalipseMainWindow),
                                        new PropertyMetadata(false));
        public LunalipseMainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Style = (Style)Application.Current.Resources["LunalipseMainWindow"];
            Loaded += MainWindowLoaded;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Foreground = obj.Foreground;
            Background = obj.Primary.SetOpacity(0.85);
        }

        public bool EnableBlur
        {
            get => (bool)GetValue(ENABLE_BLUR);
            set
            {
                SetValue(ENABLE_BLUR, value);
            }
        }

        protected virtual void MainWindowLoaded(object sender, EventArgs args)
        {
            ControlTemplate ct = (ControlTemplate)Application.Current.Resources["LunalipseMainWindowTemplate"];
            (ct.FindName("TitleBar", this) as Border).MouseDown += TitleBarMove;
            (ct.FindName("BtnCloseWn", this) as Button).Click += CloseWnd ;
            (ct.FindName("BtnSetting", this) as Button).Click += (a, b) => OnSettingClicked?.Invoke(a, b);
            (ct.FindName("BtnMinimiz", this) as Button).Click += (a, b) => OnMinimizClicked?.Invoke(a, b);
            VersionNumber = ct.FindName("VersionNumber", this) as Label;
            if (EnableBlur)
                this.EnableBlur();
        }

        private void CloseWnd(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected void TitleBarMove(object sender, EventArgs args)
        {
            this.DragMove();
        }

        public void SetVersion(string version)
        {
            VersionNumber.Content = version;
        }
    }
}
