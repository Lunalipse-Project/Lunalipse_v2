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
        protected static event Func<ScalingStrategy> OnScalingAquired;
        public static event Action<bool> OnEnableFFTChanged;

        public virtual SpectrumUnit GetSpectrumUnit(string DisplayerTag)
        {
            return new SpectrumUnit();
        }

        public virtual void RegisterDisplayer(string displayerTag, Action<bool> ConfigChangeDelegator, int DesireResolution, string Style = "SPECTRUM_CLASSIC")
        {

        }

        public ScalingStrategy AquireScalingStrategy()
        {
            return OnScalingAquired == null ? ScalingStrategy.Decibel : OnScalingAquired.Invoke();
        }

        protected void NotifyFFTEnable(bool isenable)
        {
            OnEnableFFTChanged?.Invoke(isenable);
        }

        protected void NotifyScalingChanged(ScalingStrategy scalingStrategy)
        {
            OnScalingChanged?.Invoke(scalingStrategy);
        }
    }
}
