using System;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.AudioControlPanel;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Core.PlayList;
using Lunalipse.Utilities.Misc;

namespace Lunalipse.Core
{
    public class LpsCore: IDisposable
    {
        private Interpreter executor;
        private UnrepeatedRandom random;

        public event Action OnMusicComplete;
        public event Action<MusicEntity, Track> OnMusicPrepared;
        public event Action<TimeSpan> OnMusicProgressChanged;

        public static LpsCore Session()
        {
            return new LpsCore();
        }

        protected LpsCore()
        {
            AudioOut = LpsAudio.LpsAudio.INSTANCE();
            executor = Interpreter.INSTANCE(Environment.CurrentDirectory);
            AudioDelegations.MusicLoaded += mLoaded;
            AudioDelegations.PlayingFinished += mComplete;
            AudioDelegations.PostionChanged += time => OnMusicProgressChanged?.Invoke(time);
            random = new UnrepeatedRandom();
        }

        private void mComplete()
        {
            if (!executor.LBSLoaded)
            {
                switch (MusicPlayMode)
                {
                    case PlayMode.RepeatList:
                        PerpareMusic(currentCatalogue.getNext());
                        break;
                    case PlayMode.RepeatOne:
                        PerpareMusic(CurrentPlaying);
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
            OnMusicComplete?.Invoke();
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
