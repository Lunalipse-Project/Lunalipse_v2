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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lunalipse.Presentation.LpsComponent
{
    public delegate void ProgressChange(double value);
    /// <summary>
    /// MusicProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class MusicProgressBar : UserControl
    {
        bool isDown=false;
        public MusicProgressBar()
        {
            InitializeComponent();
            this.AddHandler(PreviewMouseUpEvent, new RoutedEventHandler(UserControl_MouseUp), true);
            this.AddHandler(PreviewMouseMoveEvent, new RoutedEventHandler(UserControl_MouseMove), true);
            this.AddHandler(PreviewMouseDownEvent, new RoutedEventHandler(CurrentProgress_MouseDown), true);
            Thumb.AddHandler(MouseDownEvent, new RoutedEventHandler(CurrentProgress_MouseDown), true);
            Container.MouseLeave += Container_MouseLeave;
            MaxValue = -1;
        }

        private void Container_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                UserControl_MouseUp(sender, e);
            }
        }

        public event ProgressChange OnProgressChanged;

        //public static readonly DependencyProperty PRG_BG =
        //    DependencyProperty.Register("PROGRESS_BACKGROUND",
        //                                typeof(Brush),
        //                                typeof(MusicProgressBar),
        //                                new PropertyMetadata(Brushes.Red));
        //public static readonly DependencyProperty TRACK_BG =
        //    DependencyProperty.Register("PROGRESS_TRACK_BACKGROUND",
        //                                typeof(Brush),
        //                                typeof(MusicProgressBar),
        //                                new PropertyMetadata(null));
        public static readonly DependencyProperty _value =
            DependencyProperty.Register("PROGRESS_VALUE",
                                        typeof(double),
                                        typeof(MusicProgressBar),
                                        new PropertyMetadata(0.0d));
        public static readonly DependencyProperty _maxValue =
            DependencyProperty.Register("PROGRESS_MAXVALUE",
                                        typeof(double),
                                        typeof(MusicProgressBar),
                                        new PropertyMetadata(100d));
        public Brush BarColor
        {
            get => CurrentProgress.Background;
            set => ThumbColor = CurrentProgress.Background = value;
        }
        public Brush ThumbColor
        {
            get => Thumb.Background;
            set => Thumb.Background = value;
        }
        public Brush TrackColor
        {
            get => BAR_TRACK.Background;
            set => BAR_TRACK.Background = value;
        }

        public double MaxValue
        {
            get => (double)GetValue(_maxValue);
            set
            {
                if (value == -1)
                {
                    IsEnabled = false;
                    return;
                }
                IsEnabled = true;
                SetValue(_maxValue, value);
            }
        }
        public double Value
        {
            get => (double)GetValue(_value);
            set
            {
                if (value > MaxValue||isDown) return;
                SetValue(_value, value);
                UpdateLength(false);
            }
        }
        private double ValueInner
        {
            get => (double)GetValue(_value);
            set
            {
                if (value > MaxValue) return;
                SetValue(_value, value);
                UpdateLength(true);
            }
        }

        private void UpdateLength(bool isNotify)
        {
            double v = (Value / MaxValue) * (this.ActualWidth - 4);
            CurrentProgress.Width = v < 0 ? 0 : v;
            if (isNotify)
            {
                OnProgressChanged(Value);
            }
        }
        private void UserControl_MouseMove(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Moved");
            if (isDown)
            {
                Point p = Mouse.GetPosition(this);
                CurrentProgress.Width = (p.X - 4) < 0 ? 0 : p.X - 4;
            }
            e.Handled = true;
        }

        private void UserControl_MouseUp(object sender, RoutedEventArgs e)
        {
            if (isDown)
            {
                Point p = Mouse.GetPosition(this);
                double newVal = (p.X / ActualWidth) * MaxValue;
                ValueInner = newVal;
                isDown = false;
            }
            e.Handled = true;
        }

        private void CurrentProgress_MouseDown(object sender, RoutedEventArgs e)
        {
            isDown = true;
            e.Handled = true;
        }
    }
}
