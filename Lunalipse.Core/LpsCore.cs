using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.AudioControlPanel;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Core.Lyric;
using Lunalipse.Core.PlayList;
using Lunalipse.Utilities.Misc;

namespace Lunalipse.Core
{
    public class LpsCore: IDisposable
    {

        public volatile static Dictionary<string, LpsCore> lpsCoresSessions = new Dictionary<string, LpsCore>();

        private Interpreter executor;
        private UnrepeatedRandom random;

        public event Action OnMusicComplete;
        public event Action<MusicEntity, Track> OnMusicPrepared;
        public event Action<TimeSpan> OnMusicProgressChanged;

        public static LpsCore Session(string name,bool immersed = false, int latency = 100)
        {
            LpsCore tempCore = new LpsCore(immersed, latency);
            lpsCoresSessions.Add(name, tempCore);
            return tempCore;
        }

        public static LpsCore GetSession(string name)
        {
            if (!lpsCoresSessions.ContainsKey(name)) return null;
            return lpsCoresSessions[name];
        }

        protected LpsCore(bool immersed, int latency)
        {
            AudioOut = LpsAudio.LpsAudio.Instance(immersed,latency);
            executor = Interpreter.INSTANCE(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            AudioDelegations.MusicLoaded += mLoaded;
            AudioDelegations.PlayingFinished += mComplete;
            AudioDelegations.PostionChanged += time => OnMusicProgressChanged?.Invoke(time);
            random = new UnrepeatedRandom();

            AudioOut.LyricTokenzier = LyricTokenizer.INSTANCE;
        }

        private void mComplete()
        {
            GetNext();
            OnMusicComplete?.Invoke();
        }

        public void GetNext()
        {
            if (!executor.LBSLoaded)
            {
                switch (MusicPlayMode)
                {
                    case PlayMode.RepeatOne:
                    case PlayMode.RepeatList:
                        PerpareMusic(currentCatalogue.getNext());
                        break;
                    case PlayMode.Shuffle:
                        int i = random.Next();
                        PerpareMusic(currentCatalogue.getMusic(i));
                        break;
                }
            }
            else
            {
                PerpareMusic(executor.Stepping());
            }
        }

        public void GetPrevious()
        {
            if (!executor.LBSLoaded)
            {
                switch (MusicPlayMode)
                {
                    case PlayMode.RepeatOne:
                    case PlayMode.RepeatList:
                        PerpareMusic(currentCatalogue.getPrevious());
                        break;
                    case PlayMode.Shuffle:
                        int i = random.Previous();
                        PerpareMusic(currentCatalogue.getMusic(i));
                        break;
                }
            }
            else
            {
                PerpareMusic(executor.Stepping());
            }
        }

        private void mLoaded(MusicEntity Music, Track mTrack)
        {

            OnMusicPrepared?.Invoke(Music, mTrack);
        }

        public float CurrentMusicVolume
        {
            get
            {
                return AudioOut.Volume;
            }
            set
            {
                AudioOut.Volume = value;
            }
        }

        public void PositionMoveTo(double sec) => AudioOut.MoveTo(sec);

        public LpsAudio.LpsAudio AudioOut { get; }

        public PlayMode MusicPlayMode { get; set; }

        public int getCurrentPlayingIndex { get => currentCatalogue.CurrentIndex; }

        public MusicEntity CurrentPlaying { get; private set; }

        public Catalogue currentCatalogue { get; private set; }

        public void PerpareMusic(MusicEntity entity)
        {
            if (AudioOut.Playing || AudioOut.isLoaded) AudioOut.Stop();
            currentCatalogue.SetMusic(entity);
            CurrentPlaying = entity;
            AudioOut.Load(entity);
            AudioOut.Play();
        }

        /// <summary>
        /// Set playing catalogue
        /// </summary>
        /// <param name="catalogue"></param>
        public void SetCatalogue(Catalogue catalogue)
        {
            if (currentCatalogue != catalogue)
            {
                currentCatalogue = catalogue;
                random.Update(0, catalogue.GetCount());
            }
        }

        /// <summary>
        /// Pause the stream
        /// </summary>
        public void Pause()
        {
            if (AudioOut.Playing)
                AudioOut.Pause();
        }

        /// <summary>
        /// Resume the stream
        /// </summary>
        public void Resume()
        {
            if (!AudioOut.Playing)
                AudioOut.Resume();
        }

        

        public void Dispose()
        {
            //if (audio.Playing) 
            //    audio.Pause();
            if (AudioOut.isLoaded)
                AudioOut.Stop();
            AudioOut.Dispose();
        }


        //TODO 增加LPScript加载方法
    }
}
