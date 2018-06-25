using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using CSCore.DSP;
//using HelixToolkit.Wpf;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Core.Visualization;

namespace Lunalipse
{
    /// <summary>
    /// _3DVisualize.xaml 的交互逻辑
    /// </summary>
    public partial class _3DVisualize : Page
    {
//        LpsAudio lps;
//        PointsVisual3D pv3;
//        Wave w;
//        public _3DVisualize()
//        {
//            InitializeComponent();
//            lps = LpsAudio.INSTANCE();
//            View.Camera.Reset();
//            View.Camera.Position = new Point3D(26.1378213208776, -57.5322885647617, 6.85004889041239);
//            View.Camera.LookDirection = new Vector3D(-31.1378213208776, 53.9322885647617, -3.85004889041239);
//            View.ShowViewCube = false;
//            View.IsRotationEnabled = true;
//            View.IsPanEnabled = true;
//            View.IsMoveEnabled = true;
//            w = new Wave(FftSize.Fft4096) {
//                UseAverage = true,
//                XMax = 100,
//                YMax = 100,
//                IsXLogScale = true,
//                MaxOffset = 8,
//                MaxHeight = 4
//            };
//            __gen();
//        }

//        void __gen()
//        {
//            pv3 = new PointsVisual3D() { Color = Colors.White, Size = 2 };
//            Point3DCollection p3d = null;
//            DispatcherTimer timer = new DispatcherTimer();
//#if !DEBUG            
//            int counter = 0;
//#endif
//            timer.Tick += (a, b) =>
//            {
//#if DEBUG
//                p3d = new Point3DCollection(w.CreateWave());
//                View.Children.Remove(pv3);
//                pv3.Points = p3d;
//                View.Children.Add(pv3);
//#else
//                if (lps.Playing)
//                {
//                    if (counter < 5 && counter != 0)
//                    {
//                        p3d = new Point3DCollection(Desend(pv3.Points.GetEnumerator()));
//                        View.Children.Remove(pv3);
//                        pv3.Points = p3d;
//                        View.Children.Add(pv3);
//                    }
//                    else if (counter >= 5)
//                    {
//                        counter = 0;
//                    }
//                    if (counter == 0)
//                    {
//                        p3d = new Point3DCollection(w.CreateWave());
//                        View.Children.Remove(pv3);
//                        pv3.Points = p3d;
//                        View.Children.Add(pv3);
//                    }
//                    counter++;
//                }
//#endif
//            };
//            timer.Interval = TimeSpan.FromMilliseconds(35d);
//            timer.Start();
//        }

//        void Filp(ref Point3DCollection p3dc)
//        {
//            for (int i = 1; i < p3dc.Count; i++)
//            {
//                Point3D p = p3dc[i];
//                p.X = p.X * -1;
//                p3dc[i] = p;
//            }
//        }

//        IEnumerable<Point3D> Desend(Point3DCollection.Enumerator ps)
//        {
//            while(ps.MoveNext())
//            {
//                Point3D p = ps.Current;
//                int pz = (int)Math.Round(p.Z);
//                if (pz != 0)
//                {
//                    if (pz > 0) pz--;
//                    else if (pz < 0) pz++;
//                    p.Z = pz;
//                }
//                yield return p;
//            }
//        }
    }
}
