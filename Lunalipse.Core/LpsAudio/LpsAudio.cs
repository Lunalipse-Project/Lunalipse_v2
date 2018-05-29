﻿using Lunalipse.Common.Data;
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

namespace Lunalipse.Core.LpsAudio
{
    public class LpsAudio : ComponentHandler, ILpsAudio, IDisposable
    {
        public volatile static LpsAudio LA_instance;
        public readonly static object LA_lock = new object();

        public static LpsAudio INSTANCE(bool immersed = false)
        {
            if (LA_instance == null)
            {
                lock (LA_lock)
                {
                    LA_instance = LA_instance ?? new LpsAudio(immersed);
                }
            }
            return LA_instance;
        }

        ISoundOut wasapiOut;
        IWaveSource iws;
        Equalizer mEqualizer;
        LpsFftWarp lfw;
        Thread Counter;
        LyricEnumerator lEnum;
        bool isPlaying = false;
        float _vol = 0.7f;

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
        public LpsAudio(bool immersed=false)
        {
            wasapiOut = WasapiOut.IsSupportedOnCurrentPlatform ? GetWasapiSoundOut(immersed) : GetDirectSoundOut();
            lfw = LpsFftWarp.INSTANCE;
            lEnum = new LyricEnumerator();
            ConsoleAdapter.INSTANCE.RegisterComponent("lpsa", this);
            
            wasapiOut.Stopped += (s, e) =>
            {
                //Counter?.Abort();
                //AudioDelegations.PlayingFinished?.Invoke();
            };
            AudioDelegations.ChangeVolume += vol =>
            {
                wasapiOut.Volume = vol;
            };
            isLoaded = false;
            //Counter = new Thread(new ThreadStart(CountTimerDelegate));
        }

        //Interface implements
        public void Load(MusicEntity music)
        {
            AudioDelegations.LyricLoadStatus?.Invoke(lEnum.AcquireLyric(music));
            initializeSoundSource(music);
            isLoaded = true;
            wasapiOut.Volume = _vol / 100;
            AudioDelegations.MusicLoaded?.Invoke(music,iws.ToTrack());
        }

        public void MoveTo(double secs)
        {
            if (!isLoaded) return;
            if (secs < iws.Length)
            {
                iws.SetPosition(TimeSpan.FromSeconds(secs));
            }
        }

        public void Pause()
        {
            isPlaying = false;
            wasapiOut.Pause();
            AudioDelegations.StatuesChanged?.Invoke(isPlaying);
        }

        public void Play()
        {
            isPlaying = true;
            Counter = new Thread(new ThreadStart(CountTimerDelegate));
            Counter?.Start();
            wasapiOut.Play();
            AudioDelegations.StatuesChanged?.Invoke(isPlaying);
        }

        public void Resume()
        {
            isPlaying = true;
            wasapiOut.Resume();
            AudioDelegations.StatuesChanged?.Invoke(isPlaying);
        }

        public void Stop()
        {
            Counter?.Abort();
            isPlaying = false;
            wasapiOut.Stop();
        }

        public void SetEqualizer(double[] data)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if (!SetEqualizerIndex(i, data[i])) break;
            }
        }

        public bool SetEqualizerIndex(int inx, double data)
        {
            if (inx >= mEqualizer.SampleFilters.Count) return false;
            mEqualizer.SampleFilters[inx].AverageGainDB = data * 20;
            return true;
        }


        // Private Methods
        private IWaveSource GetCodec(string type, string file)
        {
            switch (type)
            {
                case SupportFormat.MP3:
                    if (file != null) return new DmoMp3Decoder(file);
                    break;
                case SupportFormat.FLAC:
                    if (file != null) return new FlacFile(file);
                    break;
                case SupportFormat.WAV:
                    if (file != null) return new WaveFileReader(file);
                    break;
                case SupportFormat.ACC:
                    if (file != null) return new AacDecoder(file);
                    break;
                case SupportFormat.AIFF:
                    if (file != null) return new AiffReader(file);
                    break;
            }
            return null;
        }

        private ISoundOut GetWasapiSoundOut(bool immersed = false, int latency = 100)
        {
            return new WasapiOut(true, immersed ? AudioClientShareMode.Exclusive : AudioClientShareMode.Shared, latency);
        }

        private ISoundOut GetDirectSoundOut(int latency = 100)
        {
            return new DirectSoundOut(latency);
        }

        private void initializeSoundSource(MusicEntity music)
        {
            iws?.Dispose();
            iws = GetCodec(music.Extension, music.Path);
            iws = lfw.Initialize(iws.ToSampleSource()
                .ChangeSampleRate(32000)
                .AppendSource(Equalizer.Create10BandEqualizer, out mEqualizer));
            wasapiOut.Initialize(iws);
            
        }
        
        private void CountTimerDelegate()
        {
            double totalMS = iws.GetLength().TotalMilliseconds;
            while(iws.GetPosition().TotalMilliseconds < totalMS)
            {
                if (isPlaying)
                {
                    AudioDelegations.PostionChanged?.Invoke(iws.GetPosition());
                    LyricToken lt;
                    if((lt = lEnum.Enumerating(iws.GetPosition()))!= null)
                    {
                        AudioDelegations.LyricUpdated?.Invoke(lt);
                    }
                }
                Thread.Sleep(1000);
            }
            isPlaying = false;
            AudioDelegations.PlayingFinished?.Invoke();
        }

        #region Command Handler
        public override bool OnCommand(params string[] args)
        {
            return true;
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
