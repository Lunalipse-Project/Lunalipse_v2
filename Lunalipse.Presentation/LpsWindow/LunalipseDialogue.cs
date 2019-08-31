using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.ITheme;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Lunalipse.Presentation.LpsWindow
{
    public class LunalipseDialogue : Window
    {
        const string UI_COMPONENT_THEME_UID = "PR_WND_LunalipseDialogue";
        const int BLUR_RADIUS =80;

        Border TITLE_BAR;

        Image imgControl = new Image();
        Grid imgControl_container = new Grid();
        public LunalipseDialogue()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Style = (Style)Application.Current.Resources["LunalipseDialogue"];
            Loaded += DialogueLoaded;

            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;

            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = BLUR_RADIUS;
            blurEffect.KernelType = KernelType.Gaussian;
            imgControl.Stretch = Stretch.UniformToFill;
            imgControl.VerticalAlignment = VerticalAlignment.Center;
            imgControl.HorizontalAlignment = HorizontalAlignment.Center;
            imgControl.Effect = blurEffect;
            imgControl.Opacity = 0.65;
            imgControl_container.Children.Add(imgControl);
            //imgControl_container.ClipToBounds = true;          
            imgControl_container.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x33, 0x33, 0x33));
        }

        public static readonly DependencyProperty ENABLE_FOCUSED =
            DependencyProperty.Register("LPSDIALWND_ENABLEFOCUSED",
                                        typeof(bool),
                                        typeof(LunalipseDialogue),
                                        new PropertyMetadata(false));

        protected const int HeightBias = 55;
        protected const int WidthBias = 25;


        protected virtual void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            if (!EnableFocused)
            {
                Background = obj.Primary;
            }
            Foreground = obj.Foreground;
            BorderBrush = obj.Primary;
        }

        public bool EnableFocused
        {
            get => (bool)GetValue(ENABLE_FOCUSED);
            set
            {
                SetValue(ENABLE_FOCUSED, value);
            }
        }

        protected virtual void DialogueLoaded(object sender, EventArgs args)
        {
            ControlTemplate ct = (ControlTemplate)Application.Current.Resources["LunalipseDialogueBaseTemplate"];
            (TITLE_BAR = ct.FindName("TitleBar", this) as Border).MouseDown += TitleBarMove;
            (ct.FindName("DialogueClose", this) as Button).Click += ClosePressed;
            this.HideWindowFromAltTab();
            Topmost = true;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            (imgControl_container.Children[0] as Image).Width = ActualWidth * 1.1;
            (imgControl_container.Children[0] as Image).Height = ActualHeight * 1.1;
            imgControl_container.Width = ActualWidth;
            imgControl_container.Height = ActualHeight;
        }

        protected void SetFocusedBackground(ImageSource image)
        {
            if (EnableFocused)
            {
                (imgControl_container.Children[0] as Image).Source = image;
                Background = new VisualBrush(imgControl_container);
            }
        }

        protected void TitleBarMove(object sender, EventArgs args)
        {
            if(Mouse.LeftButton== MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        protected void ClosePressed(object sender, EventArgs args)
        {
            this.Close();
        }
    }
}
