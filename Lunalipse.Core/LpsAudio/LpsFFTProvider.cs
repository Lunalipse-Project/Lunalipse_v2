using CSCore.DSP;
using Lunalipse.Common.Interfaces.IAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.LpsAudio
{
    public class LpsFFTProvider : FftProvider, ILpsFFTProvider
    {
        private readonly int _sampleRate;
        private readonly object locker = new object();
        private readonly List<object> _contexts = new List<object>();

        public LpsFFTProvider(int channels, int sampleRate, FftSize fftSize = FftSize.Fft4096)
            : base(channels, fftSize)
        {
            if (sampleRate <= 0)
                throw new ArgumentOutOfRangeException("sampleRate");
            _sampleRate = sampleRate;
            
        }

        public int GetFftBandIndex(float frequency) 
        {
            int fftSize = (int)FftSize;
            double f = _sampleRate / 2.0;
            return (int)((frequency / f) * (fftSize / 2));
        }

        public bool GetFftData(float[] fftResultBuffer, object context)
        {
            if (_contexts.Contains(context))
                return false;

            lock(locker)
            {
                _contexts.Add(context);
            }
            GetFftData(fftResultBuffer);
            return true;
        }


        public override void Add(float[] samples, int count)
        {
            base.Add(samples, count);
            if (count > 0)
            {
                lock(locker)
                {
                    _contexts.Clear();
                }
            }
        }

        public override void Add(float left, float right)
        {
            base.Add(left, right);
            lock (locker)
            {
                _contexts.Clear();
            }
        }
    }
}
