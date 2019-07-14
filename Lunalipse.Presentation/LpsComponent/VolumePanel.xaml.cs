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
    /// VolumePanel.xaml 的交互逻辑
    /// </summary>
    public partial class VolumePanel : UserControl
    {
        public event ProgressChange OnValueChanged;
        public VolumePanel()
        {
            InitializeComponent();
            VolumBar.OnValueChanged += x =>
            {
                ValueDisp.Content = Math.Round(x);
                OnValueChanged?.Invoke(x);
            };
        }

        public double MaxValue
        {
            get => VolumBar.MaxValue;
            set => VolumBar.MaxValue = value;
        }

        public Brush BarColor
        {
            get => VolumBar.BarColor;
            set => VolumBar.BarColor = value;
        }
        public Brush BarTrackColor
        {
            get => VolumBar.TrackColor;
            set => VolumBar.TrackColor = value;
        }
        public Brush FontColor
        {
            get => (SolidColorBrush)ValueDisp.Foreground;
            set => ValueDisp.Foreground = value;
        }



        public double Value
        {
            get => VolumBar.Value;
            set {
                VolumBar.Value = value;
                ValueDisp.Content = value;
            }
        }
        public bool IsHold
        {
            get => VolumBar.IsHold;
        }
    }
}
