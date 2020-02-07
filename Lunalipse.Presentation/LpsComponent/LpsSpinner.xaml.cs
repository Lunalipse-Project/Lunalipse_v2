using System;
using System.Collections.Generic;
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

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// LpsSpinner.xaml 的交互逻辑
    /// </summary>
    public partial class LpsSpinner : UserControl
    {
        /// <summary>
        /// 重力加速度
        /// </summary>
        double g = .025;
        /// <summary>
        /// 半径
        /// </summary>
        double r = 90;
        /// <summary>
        /// 初速度
        /// </summary>
        double u = 0;

        double ratio = 0.0025;

        Vector center = new Vector();
        Thread thread;
        bool Run = true;

        public double SpinnerDotRadius { get; set; } = 10;

        double[] movingPointThetas = new double[5] { 0.28, 0.28, 0.28, 0.28, 0.28 };
        public LpsSpinner()
        {
            InitializeComponent();
            this.Loaded += LpsSpinner_Loaded;
        }

        public void StopSpinning()
        {
            Run = false;
        }

        public void InitiateSpinning()
        {
            ResetAll();
            if (thread==null || thread.ThreadState != ThreadState.Running)
            {
                thread = new Thread(new ThreadStart(BeginSpinningAnimation));
                thread.Start();
            }
        }

        private void LpsSpinner_Loaded(object sender, RoutedEventArgs e)
        {
            InitiateSpinning();
        }

        private void ResetAll()
        {
            center.X = ActualWidth;
            center.Y = ActualHeight;
            r = ActualWidth / 2;
            g = ratio * r;
            u = Math.Ceiling(2 * Math.Sqrt(r * g));
            u += u * 1e-3;
            Run = true;
            for (int i = 0; i < movingPointThetas.Length; i++)
            {
                movingPointThetas[i] = 0.28;
            }
            Draw(center, movingPointThetas.Length, 0);
        }

        void CalcTheta(int appi, int start)
        {
            for (int i = start; i < appi; i++)
            {
                GetDTheta(ref movingPointThetas[i]);
            }
        }

        void GetDTheta(ref double theta)
        {
            double invr = 1 / (r * r);
            theta += Math.Sqrt(invr * (u * u - 2 * g * r * (1 - Math.Cos(theta))));
        }
        void Draw(Vector center, int appi, int start)
        {
            Drawing.Children.Clear();
            for (int i = start; i < appi; i++)
            {
                var ellipse = new Ellipse();
                ellipse.Width = SpinnerDotRadius;
                ellipse.Height = SpinnerDotRadius;
                ellipse.Fill = Foreground;
                Canvas.SetLeft(ellipse, center.X - r * (1 + Math.Sin(movingPointThetas[i])));
                Canvas.SetTop(ellipse, center.Y - r * (1 - Math.Cos(movingPointThetas[i])));
                Drawing.Children.Add(ellipse);
            }
        }

        private void BeginSpinningAnimation()
        {
            int appendIndex = 0;
            int startIndex = 0;
            int counter = 0;
            int complete = 0;
            try
            {
                while (Run)
                {
                    for (int i = startIndex; i < appendIndex; i++)
                    {
                        if (movingPointThetas[i] > (complete > 4 ? 1.91 : 2) * Math.PI)
                        {
                            if (complete > 4)
                            {
                                movingPointThetas[i] = 0.28;
                                Dispatcher.Invoke(() => Drawing.Children.RemoveAt(i - startIndex));
                                startIndex++;
                            }
                            else
                            {
                                movingPointThetas[i] = 0;
                                complete++;
                            }
                        }
                    }
                    if (startIndex >= appendIndex)
                    {
                        startIndex = 0;
                        appendIndex = 0;
                        counter = 0;
                        complete = 0;
                        Thread.Sleep(500);
                    }
                    else
                    {
                        CalcTheta(appendIndex, startIndex);
                        Dispatcher.Invoke(() =>
                        {
                            Draw(center, appendIndex, startIndex);
                        });
                    }
                    if (appendIndex < 5 && counter % 8 == 0) appendIndex++;
                    if (appendIndex < 5)
                    {
                        counter++;
                    }
                    Thread.Sleep(16);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
