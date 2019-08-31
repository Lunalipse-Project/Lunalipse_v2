using CSCore.DSP;
using Lunalipse.Common.Generic.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IVisualization
{
    public struct SpectrumUnit
    {
        public Type SpectrumStyle;
        public int resolution;
    }
    public struct SpectrumDisplayer
    {
        public Action<bool> Delegator;
        public string currentStyle;
        public int DesireResolution;
    }
    public class VisualizationManagerBase
    {
        public static event Action<ScalingStrategy> OnScalingChanged;
        protected static Func<ScalingStrategy> OnScalingAquired;
        protected static Func<FftSize> OnSizeAquired;
        public static event Action<FftSize> OnSizeChanged;
        public static event Action<bool> OnEnableFFTChanged;

        public virtual SpectrumUnit GetSpectrumUnit(string DisplayerTag)
        {
            return new SpectrumUnit();
        }

        public virtual void RegisterDisplayer(string displayerTag, Action<bool> ConfigChangeDelegator, int DesireResolution, string Style = "SPECTRUM_CLASSIC")
        {

        }

        public virtual ScalingStrategy AquireScalingStrategy()
        {
            return OnScalingAquired == null ? ScalingStrategy.Decibel : OnScalingAquired.Invoke();
        }

        public virtual FftSize AquireFftSize()
        {
            return OnSizeAquired == null ? FftSize.Fft4096 : OnSizeAquired.Invoke();
        }

        protected void NotifyFFTEnable(bool isenable)
        {
            OnEnableFFTChanged?.Invoke(isenable);
        }

        protected void NotifyScalingChanged(ScalingStrategy scalingStrategy)
        {
            OnScalingChanged?.Invoke(scalingStrategy);
        }
        public void NotifySizeChanged(FftSize size)
        {
            OnSizeChanged?.Invoke(size);
        }
    }
}
