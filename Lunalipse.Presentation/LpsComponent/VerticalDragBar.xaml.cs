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
    /// <summary>
    /// VerticalDragBar.xaml 的交互逻辑
    /// </summary>
    public partial class VerticalDragBar : UserControl
    {
        public event ProgressChange OnValueChanged;
        bool isDown = false;
        public VerticalDragBar()
        {
            InitializeComponent();
            AddHandler(PreviewMouseUpEvent, new RoutedEventHandler(SetToUnDraged), true);
            AddHandler(PreviewMouseMoveEvent, new RoutedEventHandler(ThumbDrag), true);
            AddHandler(PreviewMouseDownEvent, new RoutedEventHandler(StartDragThumb), true);
            AddHandler(MouseLeaveEvent, new RoutedEventHandler(SetToUnDraged), true);
            Thumb.AddHandler(MouseDownEvent, new RoutedEventHandler(StartDragThumb), true);
            // Refreshing the UI, GETTING THE HEIGHT
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        public static readonly DependencyProperty PRG_BG =
            DependencyProperty.Register("VDRAGBAR_BACKGROUND",
                                        typeof(Brush),
                                        typeof(MusicProgressBar),
                                        new PropertyMetadata(Brushes.Red));
        public static readonly DependencyProperty TRACK_BG =
            DependencyProperty.Register("VDRAGBAR_TRACK_BACKGROUND",
                                        typeof(Brush),
                                        typeof(MusicProgressBar),
                                        new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty _value =
            DependencyProperty.Register("VDRAGBAR_VALUE",
                                        typeof(double),
                                        typeof(MusicProgressBar),
                                        new PropertyMetadata(0.0d));
        public static readonly DependencyProperty _maxValue =
            DependencyProperty.Register("VDRAGBAR_MAXVALUE",
                                        typeof(double),
                                        typeof(MusicProgressBar),
                                        new PropertyMetadata(100d));
        public SolidColorBrush BarColor
        {
            get => (SolidColorBrush)GetValue(PRG_BG);
            set => SetValue(PRG_BG, value);
        }
        public SolidColorBrush TrackColor
        {
            get => (SolidColorBrush)GetValue(TRACK_BG);
            set => SetValue(TRACK_BG, value);
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
        public bool IsHold { get => isDown; }
        public double Value
        {
            get => (double)GetValue(_value);
            set
            {
                if (value > MaxValue || isDown) return;
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
            if (ActualHeight != 0)
            {
                CurrentVal.Height = (Value / MaxValue) * (ActualHeight - 4);
            }
            else
                CurrentVal.Height = (Value / MaxValue) * (DesiredSize.Height - 4);
            if (isNotify)
            {
                OnValueChanged?.Invoke(Value);
            }
        }
        private void ThumbDrag(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("Moved");
            //Point p = Mouse.GetPosition(this);
            //Console.WriteLine(p.Y);
            if (isDown)
            {
                Point p = Mouse.GetPosition(this);
                double H = ActualHeight - (p.Y);
                //Console.WriteLine(H);
                if (H <= ActualHeight && H>=0)
                {
                    CurrentVal.Height = H;
                    double newVal = (CurrentVal.Height / ActualHeight) * MaxValue;
                    ValueInner = newVal;
                }
            }
        }
        private void SetToUnDraged(object sender, RoutedEventArgs e)
        {
            if (isDown)
            {
                //double newVal = (CurrentVal.Height / ActualHeight) * MaxValue;
                //ValueInner = newVal;
                isDown = false;
            }
        }
        private void StartDragThumb(object sender, RoutedEventArgs e)
        {
            isDown = true;
        }
    }
}
