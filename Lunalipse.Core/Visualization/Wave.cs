using CSCore.DSP;
using Lunalipse.Common.Interfaces.IVisualization;
using Lunalipse.Core.LpsAudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Lunalipse.Core.Visualization
{
    public class Wave
    {
        /*
        private int _XMax, _YMax = 80;

        public delegate void GetFFTData(float[] fft);
        public Wave(FftSize fftSize)
        {
            FftSize = fftSize;
            UpdateFrequencyMapping();
        }

        public int XMax
        {
            get { return _XMax; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                _XMax = value;
                SpectrumResolution = value;
            }
        }

        public int YMax
        {
            get { return _YMax; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                _YMax = value;
            }
        }

        public double MaxOffset { get; set; }
        public double MaxHeight { get; set; }
        /*
        public IEnumerable<Point3D> CreateWave()
        {
            //UpdateFrequencyMapping();
            float[] fftBuffer = new float[(int)FftSize];
            fftBuffer = AudioDelegations.FftAcquired();
            //get the fft result from the spectrum provider
            SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(MaxOffset, fftBuffer);
            return GeneratePoints(spectrumPoints, MaxHeight);
            
        }

        protected override void UpdateFrequencyMapping()
        {
            base.UpdateFrequencyMapping();
        }

        private IEnumerable<Point3D> GeneratePoints(SpectrumPointData[] n,double h)
        {
            int offset = (int)Math.Ceiling(_XMax / 2d) * -1;
            for (int i = 0; i < _XMax; i++)
            {
                Point3D p = new Point3D();
                p.X = (i + offset) * 1.5;
                double v = (double)Decimal.Round(new decimal(n[i].Value), 3);
                if (v < Math.Round(MaxOffset / 2))
                    v = -v;
                else if (v == Math.Round(MaxOffset / 2))
                    v = 0;
                for (int j = 0; j < _YMax; j++)
                {
                    double rad = Math.PI * 0.1;
                    p.Y = (j - 40) * 1.5;
                    p.Z = h * 0.05 * v * (Math.Sin(rad * p.X + v) + Math.Cos(rad * p.Y + v));
                    yield return p;
                }
            }
        }*/
    }
}
