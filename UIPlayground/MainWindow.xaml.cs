using Lunalipse.Core.Markdown;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace UIPlayground
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 重力加速度
        /// </summary>
        double g = .025;
        /// <summary>
        /// 半径
        /// </summary>
        double r = 30;
        /// <summary>
        /// 初速度
        /// </summary>
        double u = 0;

        double ratio = 0.0025;

        double dim = 10;

        double[] movingPointThetas = new double[5] { 0.28, 0.28, 0.28, 0.28, 0.28 };

        public MainWindow()
        {
            InitializeComponent();
            ProgressBar.TrackBackgroundBrush = Brushes.LightGray;
            ProgressBar.ProgressBackgroundBrush = Brushes.Blue;
            ProgressBar.Wait();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
