using CSCore.DSP;
using Lunalipse.Common.Interfaces.IVisualization;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lunalipse.Core.Visualization
{
    public class LineSpectrum : ISpectrumStyle<UIElement>
    {
        private int Resolution = 20;
        private double BarWidth;
        private double BarSpacing = 3;
        private IV11nHelper helper;

        const double RATIO = 2.618;

        private Size cur_RegionSize;
        private Brush MainColor;

        private List<Line> SpectrumLine = new List<Line>();

        public LineSpectrum()
        {
            SetBarSpacing(2);
        }

        public void SetBarSpacing(double Spacing)
        {
            BarSpacing = Spacing;
            BarWidth = Math.Max(((cur_RegionSize.Width - (BarSpacing * (Resolution + 1))) / Resolution), 0.00001);
        }

        public void SetDrawingSize(Size size)
        {
            if (cur_RegionSize != size)
            {
                cur_RegionSize = size;
            }
        }

        public IEnumerable<UIElement> OnDrawSpectrum(float[] fftData)
        {
            double height = cur_RegionSize.Height;
            //prepare the fft result for rendering 
            SpectrumPointData[] spectrumPoints = helper.CalculateSpectrumPoints(fftData, height);
            double xCoord;
            int barIndex;
            SpectrumPointData p;
            //connect the calculated points with lines
            for (int i = 0; i < spectrumPoints.Length; i++)
            {
                p = spectrumPoints[i];
                barIndex = p.SpectrumPointIndex;
                xCoord = BarSpacing * (barIndex + 1) + (BarWidth * barIndex) + BarWidth / 2;
                Line line = SpectrumLine[i];
                line.Stroke = MainColor;
                line.StrokeThickness = BarWidth;
                line.X1 = xCoord;
                line.X2 = xCoord;
                line.Y1 = height;
                line.Y2 = height - p.Value - 1d;
                yield return line;
            }
        }

        public void SetV11NHelper(IV11nHelper v11NHelper)
        {
            helper = v11NHelper;
        }

        public void SetMainColor(Brush brush)
        {
            MainColor = brush;
        }

        public Brush GetMainColor()
        {
            return MainColor;
        }

        public void SetCustomizedArguments(params object[] args)
        {
            
        }

        public void SetResolution(int resolution)
        {
            Resolution = resolution;
            helper.UpdateSpectrumResolution(resolution);
            if (SpectrumLine != null)
            {
                if (SpectrumLine.Count != resolution)
                {
                    BarSpacing = Math.Floor(cur_RegionSize.Width / (RATIO * resolution));
                    BarWidth = Math.Max(((cur_RegionSize.Width - (BarSpacing * (resolution + 1))) / resolution), 0.00001);
                    SpectrumLine.Clear();
                    for (int i = 0; i < resolution; i++)
                    {
                        SpectrumLine.Add(new Line());
                        SpectrumLine[i].SnapsToDevicePixels = true;
                        SpectrumLine[i].SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                    }
                }
            }
        }
    }
}
