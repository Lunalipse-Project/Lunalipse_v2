using Lunalipse.Common.Generic.Themes;
using Lunalipse.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// ToggleSwitch.xaml 的交互逻辑
    /// </summary>
    public partial class ToggleSwitch : UserControl
    {
        readonly Duration elapseTime = new Duration(TimeSpan.FromMilliseconds(90));
        ColorAnimation ThumbToAct, ThumbToDeact;
        DoubleAnimation TrackAct, TrackDeact;

        const int DEACTIVE_TRACK_LEN = 7;
        const int ACTIVE_TRACK_LEN = 25;
        const string UI_COMPONENT_THEME_UID = "PR_COMP_ToggleSwitch";

        public Color ThumbActive { get; protected set; } = Colors.PaleGreen;
        public Color ThumbDeactive { get; protected set; } = Colors.White;
        public SolidColorBrush TrackActive { get; protected set; } = Colors.LightGreen.ToBrush();
        public SolidColorBrush TrackDeactive { get; protected set; } = Colors.LightGray.ToBrush();

        public event Action OnSwitchTurnOn;
        public event Action OnSwitchTurnOff;
        public event Action<object,bool> OnSwitchStatusChanged;

        private bool initialState = false;  //OFF
        public ToggleSwitch()
        {
            InitializeComponent();
            ThumbToAct = new ColorAnimation(ThumbDeactive, ThumbActive, elapseTime);
            ThumbToDeact = new ColorAnimation(ThumbActive, ThumbDeactive, elapseTime);
            TrackAct = new DoubleAnimation(DEACTIVE_TRACK_LEN, ACTIVE_TRACK_LEN, elapseTime);
            TrackDeact = new DoubleAnimation(ACTIVE_TRACK_LEN, DEACTIVE_TRACK_LEN, elapseTime);

            this.MouseDown += OnThumbPressed;

            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());

            Thumb.Fill = new SolidColorBrush(ThumbDeactive);
            Track_Active.Background = TrackActive;
            Track_Deactive.Background = TrackDeactive;
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            ThumbActive = obj.Secondary.GetColor();
            ThumbDeactive = obj.Foreground.GetColor().ToLuna();
            TrackActive = obj.Secondary.GetColor().ToCelestia(0.3f).ToBrush();
            TrackDeactive = obj.Foreground.GetColor().ToLuna(0.3f).ToBrush();
            ResetColor();
        }

        private void ResetColor()
        {
            ThumbToAct.From = ThumbToDeact.To = ThumbDeactive;
            ThumbToAct.To = ThumbToDeact.From = ThumbActive;
            Track_Active.Background = TrackActive;
            Track_Deactive.Background = TrackDeactive;

        }

        private void OnThumbPressed(object sender, EventArgs args)
        {
            Toggle(!initialState);
        }

        public void Toggle(bool state)
        {
            if (!state)
            {
                Track_Active.BeginAnimation(WidthProperty, TrackDeact);
                Thumb.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ThumbToDeact);
                initialState = false;
                OnSwitchTurnOff?.Invoke();
            }
            else
            {
                Track_Active.BeginAnimation(WidthProperty, TrackAct);
                Thumb.Fill.BeginAnimation(SolidColorBrush.ColorProperty, ThumbToAct);
                initialState = true;
                OnSwitchTurnOn?.Invoke();
            }
            OnSwitchStatusChanged?.Invoke(this, initialState);
        }
    }
}
