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
            Loaded += LpsEqualizer_Loaded;
        }

        private void LpsEqualizer_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            for (int i = 1; i <= 10; i++)
            {
                VerticalDragBar dragBar = FindName("p" + i) as VerticalDragBar;
                dragBar.MaxValue = 24;
                dragBar.Value = 12;
            }
        }

        private void VerticalDragBar_OnValueChanged(object sender, double value)
        {
            VerticalDragBar dragBar = sender as VerticalDragBar;
            if(dragBar!=null)
            {
                OnEqualizerValueChanged?.Invoke(int.Parse((string)dragBar.Tag), value);
            }
        }

        

        public void SetDragBarTheme(Brush track, Brush bar)
        {
            for(int i=1;i<=10;i++)
            {
                VerticalDragBar dragBar = FindName("p" + i) as VerticalDragBar;
                dragBar.TrackColor = track;
                dragBar.BarColor = bar;
            }
            Brush b = p1.BarColor;
        }
    }
}
