using CSCore.DSP;
using Lunalipse.Common.Generic.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IVisualization
{
    public interface IV11nHelper
    {
        void UpdateFrequencyMapping();
        SpectrumPointData[] CalculateSpectrumPoints(float[] fftBuffer, double maxValue = 1);

        void UpdateSpectrumResolution(int resolution);
        void SetScalingStrategy(ScalingStrategy scalingStrategy);
        void SetFftSize(FftSize size);
        FftSize GetFftSize();
    }
    public struct SpectrumPointData
    {
        public int SpectrumPointIndex;
        public double Value;
    }
}
