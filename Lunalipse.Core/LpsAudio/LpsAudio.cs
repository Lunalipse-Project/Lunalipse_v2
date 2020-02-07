using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IAudio;
using CSCore.SoundOut;
using CSCore;
using CSCore.Codecs.MP3;
using CSCore.Codecs.AIFF;
using CSCore.Codecs.AAC;
using CSCore.Codecs.WAV;
using CSCore.Codecs.FLAC;
using CSCore.CoreAudioAPI;
using CSCore.Streams.Effects;
using Lunalipse.Common.Generic.Audio;
using System.Threading;
using System;
using Lunalipse.Core.Lyric;
using Lunalipse.Common.Interfaces.ILyric;
using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Core.Console;
using System.Windows;
using Lunalipse.Core.Visualization;
using Lunalipse.Common.Interfaces.IVisualization;
using CSCore.DSP;
using System.IO;

namespace Lunalipse.Core.LpsAudio
{
    public class LpsAudio : IConsoleComponent, ILpsAudio, IDisposable
    {
        volatile static LpsAudio LA_instance;
        readonly static object LA_lock = new object();

        public static LpsAudio Instance(bool immersed = false,int latency = 100)
        {
            if (LA_instance == null)
            {
                lock (LA_lock)
                {
                    LA_instance = LA_instance ?? new LpsAudio(immersed, latency);
                }
            }
            return LA_instance;
        }

        ISoundOut wasapiOut;
        IWaveSource iws;
        public static Equalizer mEqualizer;
        LpsFftWarp lfw;
        Thread Counter, SpectrumUpdater;
        LyricEnumerator lEnum;
        bool isPlaying = false;
        bool isSoundThreadFinished = false;
        float _vol = 0.7f;

        double[] equalizerSetting_temp = new double[10];

        VisualizationManager vManager;

        public float Volume
        {
            get => _vol;
            set
            {
                if (isLoaded)
                    wasapiOut.Volume = (_vol = value) / 100;
                else
                    _vol = value;
            }
        }
        public bool Playing
        {
            get { return isPlaying; }
        }


        public bool wasapiSupport
        {
            get
            {
                return WasapiOut.IsSupportedOnCurrentPlatform;
            }
        }

        
        

        public Equalizer LpsEqualizer
        {
            get
            {
                return mEqualizer;
            }
            set
            {
                mEqualizer = value;
            }
        }
        public ILyricTokenizer LyricTokenzier
        {
            get
            {
                return lEnum.Tokenizer;
            }
            set
            {
                lEnum.Tokenizer = value;
            }
        }


        public bool isLoaded { get; private set; }

        // Constructor
        private LpsAudio(bool immersed, int latency)
        {
            wasapiOut = WasapiOut.IsSupportedOnCurrentPlatform ? GetWasapiSoundOut(immersed, latency) : GetDirectSoundOut(latency);
            lfw = LpsFftWarp.Instance;
            vManager = VisualizationManager.Instance;
            lEnum = new LyricEnumerator();

            lfw.FFTBufferSize = vManager.AquireFftSize();
            VisualizationManagerBase.OnSizeChanged += VisualizationManagerBase_OnSizeChanged;
            lEnum.LyricDefaultDir = "Lyrics";
            ConsoleAdapter.Instance.RegisterComponent(GetType().Name, this);

            AudioDelegations.ChangeEqualizerSetting = SetEqualizer;


            wasapiOut.Stopped += (s, e) =>
            {
                //Counter?.Abort();
                //AudioDelegations.PlayingFinished?.Invoke();
            };
            AudioDelegations.ChangeVolume += vol =>
            {
                Volume = vol;
            };
            isLoaded = false;
            //Counter = new Thread(new ThreadStart(CountTimerDelegate));
        }

        private void VisualizationManagerBase_OnSizeChanged(FftSize obj)
        {
            lfw.FFTBufferSize = obj;
        }

        //Interface implements
        public void Load(MusicEntity music)
        {
            bool LyrciPerpared = lEnum.AcquireLyric(music);
            AudioDelegations.LyricLoadStatus?.Invoke(LyrciPerpared);
            initializeSoundSource(music);
        }

        public void Load(MusicEntity music, byte[] audio_data)
        {
            bool LyrciPerpared = lEnum.AcquireLyric(music);
            AudioDelegations.LyricLoadStatus?.Invoke(LyrciPerpared);
            initializeSoundSource(music, audio_data);
        }

        [AttrConsoleSupportable]
        public void MoveTo(double secs)
        {
            if (!isLoaded) return;
            if (secs < iws.Length)
            {
                iws.SetPosition(TimeSpan.FromSeconds(secs));
            }
        }

        [AttrConsoleSupportable]
        public void Pause()
        {
            isPlaying = false;
            wasapiOut.Pause();
            AudioDelegations.StatuesChanged?.Invoke(isPlaying);
        }

        [AttrConsoleSupportable]
        public void Play()
        {
            isPlaying = true;
            Counter = new Thread(new ThreadStart(CountTimerDelegate));
            SpectrumUpdater = new Thread(new ThreadStart(FFTSpectrumUpdateDelegate));
            isSoundThreadFinished = false;
            wasapiOut.Play();
            Counter?.Start();
            SpectrumUpdater.Start();

            AudioDelegations.StatuesChanged?.Invoke(isPlaying);
        }

        [AttrConsoleSupportable]
        public void Resume()
        {
            isPlaying = true;
            wasapiOut.Resume();
            AudioDelegations.StatuesChanged?.Invoke(isPlaying);
        }

        [AttrConsoleSupportable]
        public void Stop()
        {
            isPlaying = false;
            isLoaded = false;
            Counter?.Abort();
            wasapiOut.Stop();
        }

        
        public void SetEqualizer(double[] data)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if (!SetEqualizerIndex(i, data[i])) break;
            }
        }

        [AttrConsoleSupportable]
        public bool SetEqualizerIndex(int inx, double data)
        {
            if (inx >= mEqualizer.SampleFilters.Count) return false;
            mEqualizer.SampleFilters[inx].AverageGainDB = data;
            equalizerSetting_temp[inx] = data;
            return true;
        }


        // Private Methods
        private IWaveSource GetCodec(string type, string file, byte[] audioData = null)
        {
            Stream audioStream = null;
            if (audioData != null)
            {
                audioStream = new MemoryStream(audioData);
            }
            else
            {
                if (string.IsNullOrEmpty(file)) return null;
                audioStream = File.OpenRead(file);
            }
            switch (type)
            {
                case SupportFormat.MP3:
                    return new DmoMp3Decoder(audioStream);
                case SupportFormat.FLAC:
                    return new FlacFile(audioStream);
                case SupportFormat.WAV:
                    return new WaveFileReader(audioStream);
                case SupportFormat.ACC:
                    return new AacDecoder(audioStream);
                case SupportFormat.AIFF:
                    return new AiffReader(audioStream);
                case SupportFormat.OGG:
                    return new NVorbisOggSource(audioStream).ToWaveSource();
                default:
                    return null;
            }
        }

        private IWaveSource GetCodecWebStream(string type, string Address)
        {
            switch (type)
            {
                case SupportFormat.MP3:
                    return new Mp3WebStream(Address,false);
                default:
                    return null;
            }
            
        }

        private ISoundOut GetWasapiSoundOut(bool immersed = false, int latency = 100)
        {
            return new WasapiOut(true, immersed ? AudioClientShareMode.Exclusive : AudioClientShareMode.Shared, latency);
        }

        private ISoundOut GetDirectSoundOut(int latency = 100)
        {
            return new DirectSoundOut(latency);
        }

        private void initializeSoundSource(MusicEntity music, byte[] audioData = null)
        {
            iws?.Dispose();
            if(music.IsInternetLocation)
            {
                iws = GetCodec(music.Extension, music.Path, audioData);
            }
            else
            {
                iws = GetCodec(music.Extension, music.Path);
            }
            prepareSoundOut(music);
            lfw.FFTBufferSize = vManager.FftSize;
        }

        private void prepareSoundOut(MusicEntity music)
        {
            iws = lfw.Initialize(
                iws.ToSampleSource()
                    .ChangeSampleRate(32000)
                    .AppendSource(Equalizer.Create10BandEqualizer, out mEqualizer));
            wasapiOut.Initialize(iws);
            vManager.NotifySizeChanged(vManager.FftSize);
            for (int i = 0; i < 10; i++)
            {
                SetEqualizerIndex(i, equalizerSetting_temp[i]);
            }
            isLoaded = true;
            wasapiOut.Volume = _vol / 100;
            AudioDelegations.MusicLoaded?.Invoke(music, iws.ToTrack());
        }

        private void CountTimerDelegate()
        {          
            double totalMS = iws.GetLength().TotalMilliseconds;
            TimeSpan position;
            LyricToken p_lt = null;
            while (wasapiOut.PlaybackState != PlaybackState.Stopped)
            {
                if (isPlaying)
                {
                    position = iws.GetPosition();
                    AudioDelegations.PostionChanged?.Invoke(position);
                    LyricToken lt = null;
                    if((lt = lEnum.Enumerating(position,lt))!= null)
                    {
                        if (p_lt != lt)
                        {
                            AudioDelegations.InvokeLyricUpdate(lt);
                            p_lt = lt;
                        }
                    }
                }
                Thread.Sleep(1000);
            }
            Application.Current.Dispatcher.Invoke(() => AudioDelegations.PlayingFinished?.Invoke());
            isPlaying = false;
            isSoundThreadFinished = true;
        }

        private void FFTSpectrumUpdateDelegate()
        {
            while(!isSoundThreadFinished)
            {
                if (isPlaying && vManager.EnableSpectrum && AudioDelegations.HasSubscribersSpectrum())
                {
                    AudioDelegations.UpdateFftData(AudioDelegations.FftAcquired?.Invoke());
                }
                Thread.Sleep(1000 / vManager.SpectrumUpdatePreSecond);
            }
        }

        #region Command Handler
        public bool OnCommand(ILunaConsole console, params string[] args)
        {
            return false;
        }

        public void OnEnvironmentLoaded(ILunaConsole console)
        {
            throw new NotImplementedException();
        }

        public ICommandRegistry GetCommandRegistry()
        {
            throw new NotImplementedException();
        }

        public string GetContextDescription()
        {
            return "Manage all audio playbacks and audio file decoding in Lunalipse.";
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (iws!=null && wasapiOut!=null)
                    {
                        Counter?.Abort();
                        wasapiOut.Stop();
                        wasapiOut.Dispose();
                        iws.Dispose();
                    }
                }
                disposedValue = true;
                LA_instance = null;
            }
        }

        // ~LpsAudio() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // GC.SuppressFinalize(this);
        }

        
        #endregion
    }
}
