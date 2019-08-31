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

        public static LpsFftWarp Instance
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

        public FftSize FFTBufferSize { get; set; } = FftSize.Fft4096;

        public IWaveSource Initialize(ISampleSource OrgWave)
        {
            ISampleSource iss = OrgWave;
            provider = new LpsFFTProvider(iss.WaveFormat.Channels, iss.WaveFormat.SampleRate, FFTBufferSize);
            if (notify != null)
            {
                notify.SingleBlockRead -= Notify_SingleBlockRead;
                notify.Dispose();
            }
            notify = new SingleBlockNotificationStream(iss);
            notify.SingleBlockRead += Notify_SingleBlockRead;
            return notify.ToWaveSource(16);
        }

        private void Notify_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
        {
            provider.Add(e.Left, e.Right);
        }

        public LpsFftWarp()
        {
            AudioDelegations.FftAcquired = GetFFTData;
            AudioDelegations.FftInxAcquired = GetFftBandIndex;
        }

        public float[] GetFFTData()
        {
            float[] buffer = new float[(int)FFTBufferSize];
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
