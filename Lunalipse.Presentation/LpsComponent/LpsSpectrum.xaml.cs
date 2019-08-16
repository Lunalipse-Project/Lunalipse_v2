using Lunalipse.Common.Interfaces.IVisualization;
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
    /// Interaction logic for LpsSpectrum.xaml
    /// </summary>
    public partial class LpsSpectrum : UserControl
    {
        IV11nHelper vHelper;
        ISpectrumStyle<UIElement> spectrumStyle;
        
        bool isStyleReset = false;
        public LpsSpectrum()
        {
            InitializeComponent();
            spectrumDrawing.SizeChanged += SpectrumDrawing_SizeChanged;
        }

        private void SpectrumDrawing_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            spectrumStyle?.SetDrawingSize(spectrumDrawing.RenderSize);
        }

        public string SpectrumStyle { get; set; }
        public int SpectrumResolution { get; set; }
        public VisualizationManagerBase VisualManager { get; set; }

        private void VisualizationManagerBase_OnScalingChanged(Common.Generic.Audio.ScalingStrategy obj)
        {
            vHelper?.SetScalingStrategy(obj);
        }
        private void VisualizationManagerBase_OnEnableFFTChanged(bool obj)
        {
            if(!obj)
            {
                spectrumDrawing.Children.Clear();
            }
        }

        public bool SetV11NHelper(Type helper)
        {
            if (helper.GetInterface("IV11nHelper") == null) return false;
            vHelper = Activator.CreateInstance(helper) as IV11nHelper;
            vHelper.SetScalingStrategy(VisualManager.AquireScalingStrategy());
            return true;
        }

        public bool ReloadSpectrumStyle(bool full_reload)
        {
            SpectrumUnit spectrumUnit = VisualManager.GetSpectrumUnit(Tag as string);
            if (full_reload || spectrumStyle == null)
            {
                Type style = spectrumUnit.SpectrumStyle;
                if (style == null) return false;
                spectrumStyle = Activator.CreateInstance(style) as ISpectrumStyle<UIElement>;
                spectrumStyle.SetV11NHelper(vHelper);
                spectrumStyle.SetMainColor(Foreground);
                spectrumStyle.SetDrawingSize(spectrumDrawing.RenderSize);
            }
            spectrumStyle.SetResolution(spectrumUnit.resolution);
            isStyleReset = true;
            return true;
        }

        public void UpdateSpectrumColor(Brush spectrumColor)
        {
            this.Foreground = spectrumColor;
            spectrumStyle?.SetMainColor(Foreground);
        }

        public void DrawSpectrum(float[] fftData)
        {
            if (fftData != null)
            {
                if (isStyleReset)
                {
                    vHelper.UpdateFrequencyMapping();
                    isStyleReset = false;
                }
                spectrumDrawing.Children.Clear();
                foreach (UIElement element in spectrumStyle.OnDrawSpectrum(fftData))
                {
                    spectrumDrawing.Children.Add(element);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            VisualizationManagerBase.OnScalingChanged += VisualizationManagerBase_OnScalingChanged;
            VisualizationManagerBase.OnEnableFFTChanged += VisualizationManagerBase_OnEnableFFTChanged;
            ReloadSpectrumStyle(true);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            VisualizationManagerBase.OnScalingChanged -= VisualizationManagerBase_OnScalingChanged;
            VisualizationManagerBase.OnEnableFFTChanged -= VisualizationManagerBase_OnEnableFFTChanged;
        }

        public void DisplayerDelegator(bool full_reload)
        {
            ReloadSpectrumStyle(full_reload);
        }
    }
}
