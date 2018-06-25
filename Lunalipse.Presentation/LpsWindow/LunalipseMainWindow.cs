using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.Themes;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Lunalipse.Presentation.LpsWindow
{
    public class LunalipseMainWindow : Window , IThemeCustomizable
    {
        const string UI_COMPONENT_THEME_UID = "UICOMP_LPS_MAINWINDOW";

        public event RoutedEventHandler OnSettingClicked;
        public event RoutedEventHandler OnMinimizClicked;

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

        public virtual void ThemeOverriding(ThemeTuple themeTuple)
        {
            
        }

        public string ComponentUID()
        {
            return UI_COMPONENT_THEME_UID;
        }
    }
}
