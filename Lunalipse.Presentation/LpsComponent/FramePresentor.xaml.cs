using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// FramePresentor.xaml 的交互逻辑
    /// </summary>
    public partial class FramePresentor : UserControl
    {
        private Duration elapseTime = new Duration(TimeSpan.FromMilliseconds(500));
        private DoubleAnimation AnimFadeIn;
        private DoubleAnimation AnimFadeOut;

        private object _temp_content = null;
        private Action _intermedianStep = null;

        public object currentContentKey { get; private set; }

        public event Action<object> OnFrameLoadedComplete;

        public FramePresentor()
        {
            InitializeComponent();
            AnimFadeIn = new DoubleAnimation(0, 1, elapseTime);
            AnimFadeOut = new DoubleAnimation(1, 0, elapseTime);
            AnimFadeOut.Completed += (a, b) =>
            {
                _intermedianStep?.Invoke();
                Presentor.Content = _temp_content;
                Presentor.BeginAnimation(OpacityProperty, AnimFadeIn);
            };
            AnimFadeIn.Completed += (a, b) =>
            {
                OnFrameLoadedComplete?.Invoke(Presentor.Content);
            };
            this.SizeChanged += FramePresentor_SizeChanged;
        }

        private void FramePresentor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement ui = Presentor.Content as FrameworkElement;
            if (ui == null) return;
            ui.Height = this.ActualHeight;
            ui.Width = this.ActualWidth;
        }

        public void ShowContent(object content, bool ContentFirst = false, Action step = null)
        {
            FrameworkElement ui= content as FrameworkElement;
            if (ui == null) return;
            if (!ContentFirst)
                ui.Height = this.ActualHeight;
            else
                Height = ui.Height;
            ui.Width = this.ActualWidth;
            AnimatedSwitch(ui,step);
        }

        public void BackWard()
        {
            Presentor.GoBack();
        }

        public void Forward()
        {
            Presentor.GoForward();
        }

        private void AnimatedSwitch(object content, Action step)
        {
            _temp_content = content;
            _intermedianStep = step;
            Presentor.BeginAnimation(OpacityProperty, AnimFadeOut);
        }
    }
}
