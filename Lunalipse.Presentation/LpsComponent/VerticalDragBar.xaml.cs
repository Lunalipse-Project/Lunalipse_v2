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
        public event Action<object,double> OnValueChanged;
        double maxval, val;
        bool isDown = false;
        Brush track, bar;
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
            this.DataContext = this;
        }

        public Brush BarColor
        {
            get
            {
                return bar;
            }
            set
            {
                bar = value;
                CurrentVal.Background = bar;
                BarShadowEffect.Color = (bar as SolidColorBrush).Color;
                Thumb.Background = bar;
            }
        }
        public Brush TrackColor
        {
            get
            {
                return track;
            }
            set
            {
                track = value;
                Track.Background = track ;
            }
        }

        public double MaxValue
        {
            get => maxval;
            set
            {
                if (value == -1)
                {
                    IsEnabled = false;
                    return;
                }
                IsEnabled = true;
                maxval = value;
            }
        }
        public bool IsHold { get => isDown; }
        public double Value
        {
            get => val;
            set
            {
                if (value > MaxValue || isDown) return;
                val = value;
                UpdateLength(false);
            }
        }
        private double ValueInner
        {
            get => val;
            set
            {
                if (value > MaxValue) return;
                val = value;
                UpdateLength(true);
            }
        }

        private void UpdateLength(bool isNotify)
        {
            if (ActualHeight > 0)
            {
                CurrentVal.Height = (Value / MaxValue) * (ActualHeight - 4);
            }
            else
                CurrentVal.Height = (Value / MaxValue) * (DesiredSize.Height - 4);
            if (isNotify)
            {
                OnValueChanged?.Invoke(this,Value);
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
            e.Handled = true;
        }
        private void StartDragThumb(object sender, RoutedEventArgs e)
        {
            isDown = true;
            e.Handled = true;
        }
    }
}
