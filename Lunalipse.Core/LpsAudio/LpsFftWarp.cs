using System;
using CSCore;
using CSCore.Streams;
using CSCore.DSP;
using Lunalipse.Common.Interfaces.IAudio;

namespace Lunalipse.Core.LpsAudio
{
    public class LpsFftWarp : ILpsFftWarp, IDisposable
    {
        public volatile static LpsFftWarp LFW_instance;
        public readonly static object LFW_LOCK = new object();

        public static LpsFftWarp INSTANCE
        {
            get
            {
                if (LFW_instance == null)
                {
                    lock(LFW_LOCK)
                    {
                        LFW_instance = LFW_instance ?? new LpsFftWarp();
                    }
                }
                return LFW_instance;
            }
        }

        LpsFFTProvider provider;
        SingleBlockNotificationStream notify;
        FftSize size = FftSize.Fft4096;

        FftSize FFTBufferSize
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        public IWaveSource Initialize(ISampleSource OrgWave)
        {
            ISampleSource iss = OrgWave;
            provider = new LpsFFTProvider(iss.WaveFormat.Channels, iss.WaveFormat.SampleRate, size);
            if(notify==null)
            {
                notify = new SingleBlockNotificationStream(iss);
            }
            else notify.BaseSource = iss;
            notify.SingleBlockRead += (s, e) => provider.Add(e.Left, e.Right);
            return notify.ToWaveSource(16);
        }

        public LpsFftWarp()
        {
            AudioDelegations.FftAcquired = GetFFTDat;
            AudioDelegations.FftInxAcquired = GetFftBandIndex;
        }

        public float[] GetFFTDat()
        {
            float[] buffer = new float[(int)size];
            if (provider.GetFftData(buffer, this))
                return buffer;
            else
                return null;
        }

        public int GetFftBandIndex(float freq)
        {
            return provider.GetFftBandIndex(freq);
        }

        public void Dispose()
        {
            notify.Dispose();
            LFW_instance = null;
            AudioDelegations.FftAcquired = null;
        }
    }
}
