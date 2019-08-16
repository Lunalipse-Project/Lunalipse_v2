using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Lunalipse.Common.Interfaces.IVisualization
{
    public interface ISpectrumStyle<T>
    {
        IEnumerable<T> OnDrawSpectrum(float[] fftData);
        void SetV11NHelper(IV11nHelper v11NHelper);
        void SetMainColor(Brush brush);
        Brush GetMainColor();
        void SetDrawingSize(Size size);
        void SetCustomizedArguments(params object[] args);
        void SetResolution(int resolution);
    }
}
