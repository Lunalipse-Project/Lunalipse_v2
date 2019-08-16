using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// Interaction logic for LpsEqualizer.xaml
    /// </summary>
    public partial class LpsEqualizer : UserControl
    {
        public event Action<int, double> OnEqualizerValueChanged;
        public LpsEqualizer()
        {
            InitializeComponent();
        }

        private void VerticalDragBar_OnValueChanged(object sender, double value)
        {
            VerticalDragBar dragBar = sender as VerticalDragBar;
            if(dragBar!=null)
            {
                OnEqualizerValueChanged?.Invoke(int.Parse((string)dragBar.Tag), value);
            }
        }

        public void ApplyEqualizerValue(double[] value,double maxVal = 24)
        {
            int i = 0;
            double middleVal = maxVal / 2d;
            BandForEach((dragBar) =>
            {
                dragBar.MaxValue = maxVal;
                dragBar.Value = value[i++] * 0.6d + middleVal;
            });
        }

        public void SetDragBarTheme(Brush track, Brush bar)
        {
            BandForEach((dragBar) =>
            {
                dragBar.TrackColor = track;
                dragBar.BarColor = bar;
            });
        }

        void BandForEach(Action<VerticalDragBar> action)
        {
            for (int i = 1; i <= 10; i++)
            {
                VerticalDragBar dragBar = FindName("p" + i) as VerticalDragBar;
                action.Invoke(dragBar);
            }
        }
    }
}
