using Lunalipse.Common.Generic.Audio;
using Lunalipse.Common.Interfaces.IVisualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Lunalipse.Core.Visualization
{
    public class VisualizationManager : VisualizationManagerBase
    {
        static volatile VisualizationManager vManagerInstance = null;
        static readonly object InstanceLock = new object();

        public static VisualizationManager Instance
        {
            get
            {
                if(vManagerInstance == null)
                {
                    lock(InstanceLock)
                    {
                        vManagerInstance = vManagerInstance ?? new VisualizationManager();
                    }
                }
                return vManagerInstance;
            }
        }


        bool _enableFFT = true;
        ScalingStrategy _scaling = ScalingStrategy.Decibel;
        public bool EnableSpectrum
        {
            get => _enableFFT;
            set
            {
                _enableFFT = value;
                NotifyFFTEnable(_enableFFT);
            }
        }

        public ScalingStrategy ScalingStrategy
        {
            get=> _scaling;
            set
            {
                _scaling = value;
                NotifyScalingChanged(_scaling);
            }
        }


        Dictionary<string, Type> StyleProviders = new Dictionary<string, Type>();
        Dictionary<string, SpectrumDisplayer> SpectrumDisplayers = new Dictionary<string, SpectrumDisplayer>();

        public string[] GetSpectrumProvidersName
        {
            get
            {
                return StyleProviders.Keys.ToArray();
            }
        }

        public string[] GetDisplayersNames
        {
            get
            {
                return SpectrumDisplayers.Keys.ToArray();
            }
        }

        public Dictionary<string, SpectrumDisplayer> Displayers
        {
            get => SpectrumDisplayers;
        }

        public int SpectrumUpdatePreSecond { get; set; } = 50;

        public override SpectrumUnit GetSpectrumUnit(string DisplayerTag)
        {
            SpectrumUnit unit = new SpectrumUnit();
            if(SpectrumDisplayers.ContainsKey(DisplayerTag))
            {
                SpectrumDisplayer displayer = SpectrumDisplayers[DisplayerTag];
                if (StyleProviders.ContainsKey(displayer.currentStyle))
                {
                    unit.resolution = displayer.DesireResolution;
                    if(!StyleProviders.ContainsKey(displayer.currentStyle))
                    {
                        unit.SpectrumStyle = StyleProviders.First().Value;
                        displayer.currentStyle = StyleProviders.First().Key;
                        SpectrumDisplayers[DisplayerTag] = displayer;
                    }
                    else
                    {
                        unit.SpectrumStyle = StyleProviders[displayer.currentStyle];
                    }
                }
            }
            return unit;
        }

        public void AddStyleProvider(string styleID, Type styleProvider)
        {
            if(!StyleProviders.ContainsKey(styleID))
            {
                StyleProviders.Add(styleID, styleProvider);
            }
        }

        public override void RegisterDisplayer(string displayerTag, Action<bool> ConfigChangeDelegator, int DesireResolution, string Style = "SPECTRUM_CLASSIC")
        {
            if(!SpectrumDisplayers.ContainsKey(displayerTag))
            {
                SpectrumDisplayers.Add(displayerTag, new SpectrumDisplayer()
                {
                    currentStyle = Style,
                    Delegator = ConfigChangeDelegator,
                    DesireResolution = DesireResolution > 0 ? DesireResolution : 30
                });
            }
            else
            {
                SpectrumDisplayer spectrumDisplayer = SpectrumDisplayers[displayerTag];
                spectrumDisplayer.Delegator = ConfigChangeDelegator;
                SpectrumDisplayers[displayerTag] = spectrumDisplayer;
            }
        }

        public void UpdateDisplayerConfig(string displayerTag, string styleID, int Resolution = -1)
        {
            if(SpectrumDisplayers.ContainsKey(displayerTag))
            {
                bool full_reload = false;
                SpectrumDisplayer displayer = SpectrumDisplayers[displayerTag];
                if (displayer.currentStyle != styleID && styleID != null)
                {
                    displayer.currentStyle = styleID;
                    full_reload = true;
                }
                if (Resolution != -1 && displayer.DesireResolution != Resolution)
                    displayer.DesireResolution = Resolution;
                SpectrumDisplayers[displayerTag] = displayer;
                displayer.Delegator?.Invoke(full_reload);
            }
        }

        public SpectrumDisplayer GetDisplayer(string displayerTag)
        {
            if (!SpectrumDisplayers.ContainsKey(displayerTag)) return new SpectrumDisplayer();
            return SpectrumDisplayers[displayerTag];
        }
    }
}
