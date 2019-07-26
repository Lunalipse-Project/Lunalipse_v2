using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LunalipseInstaller
{
    /// <summary>
    /// LpsProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class LpsProgressBar : UserControl
    {
        private double CURRENT_VALUE = 0d;
        private bool progress_waiting = false;

        Thread WaitingAnimation;
        public LpsProgressBar()
        {
            InitializeComponent();
            Loaded += LpsProgressBar_Loaded;
        }

        private void LpsProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBarBackground.CornerRadius = new CornerRadius(ActualHeight / 2);
            Track.CornerRadius = new CornerRadius(ActualHeight / 2);
        }

        public void Wait()
        {
            if(!progress_waiting)
            {
                progress_waiting = true;
                if (WaitingAnimation != null)
                {
                    if (WaitingAnimation.ThreadState == ThreadState.Running)
                    {
                        WaitingAnimation.Abort();
                    }
                }
                WaitingAnimation = new Thread(new ThreadStart(Waiting));
                WaitingAnimation.Start();
            }
        }

        public double MaximumValue { get; set; }
        public double CurrentValue
        {
            get
            {
                return CURRENT_VALUE;
            }
            set
            {
                CURRENT_VALUE = value;
                if(progress_waiting)
                {
                    progress_waiting = false;
                }
                if (CURRENT_VALUE >= 0 && CURRENT_VALUE <= MaximumValue)
                {
                    ProgressBar.Width = ActualWidth * (CURRENT_VALUE / MaximumValue);
                }
            }
        }

        public Brush TrackBackgroundBrush
        {
            get => Track.Background;
            set => Track.Background = value;
        }

        public Brush ProgressBackgroundBrush
        {
            get => ProgressBarBackground.Background;
            set => ProgressBarBackground.Background = value;
        }

        private void Waiting()
        {
            HorizontalAlignment currentAlignment = HorizontalAlignment.Left;
            Dispatcher.Invoke(new Action(() => ProgressBar.HorizontalAlignment = currentAlignment));
            Dispatcher.Invoke(new Action(() => ProgressBar.Width = 0));
            while (progress_waiting)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (ProgressBar.Width < this.ActualWidth && currentAlignment == HorizontalAlignment.Left)
                    {
                        ProgressBar.Width+=4;
                    }
                    else if (ProgressBar.Width >= this.ActualWidth && currentAlignment == HorizontalAlignment.Left)
                    {
                        currentAlignment = HorizontalAlignment.Right;
                        ProgressBar.HorizontalAlignment = currentAlignment;
                    }
                    else if (ProgressBar.Width <= 0 && currentAlignment == HorizontalAlignment.Right)
                    {
                        currentAlignment = HorizontalAlignment.Left;
                        ProgressBar.HorizontalAlignment = currentAlignment;
                    }
                    else
                    {
                        ProgressBar.Width-=4;
                    }
                }));
                Thread.Sleep(1000 / 64);
            }
            currentAlignment = HorizontalAlignment.Left;
            Dispatcher.Invoke(new Action(() => ProgressBar.HorizontalAlignment = currentAlignment));
            Dispatcher.Invoke(new Action(() => ProgressBar.Width = 0));
        }
    }
}
