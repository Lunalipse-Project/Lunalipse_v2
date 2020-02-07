using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.AudioControlPanel;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Core.Lyric;
using Lunalipse.Core.Metadata;
using Lunalipse.Core.PlayList;
using Lunalipse.Utilities.Misc;

namespace Lunalipse.Core
{
    public class LpsCore: IDisposable
    {

        public volatile static Dictionary<string, LpsCore> lpsCoresSessions = new Dictionary<string, LpsCore>();

        private BScriptManager bsManager;
        private SequenceControllerManager controllerManager;
        private UnrepeatedRandom random;
        //private MusicEntity CurrentPlaying = null;

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
            bsManager = BScriptManager.Instance(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)+"/Scripts");
            controllerManager = SequenceControllerManager.Instance;
            AudioOut.LyricTokenzier = LyricTokenizer.INSTANCE;
            random = new UnrepeatedRandom();

            AudioDelegations.MusicLoaded = mLoaded;
            AudioDelegations.PlayingFinished = mComplete;
            AudioDelegations.PostionChanged = time => OnMusicProgressChanged?.Invoke(time);

            controllerManager.AddController("GENERAL", GeneralSeqController);
            controllerManager.AddController("BSCRIPT", ScriptSeqController, true, true);
            controllerManager.SetController("GENERAL");
        }

        private void mComplete()
        {
            CurrentPlaying?.DisposePicture();
            GetNext();
            OnMusicComplete?.Invoke();
        }


        public void GeneralSeqController(Action<MusicEntity> prepareFunc, ICatalogue catalogue, PlayMode playmode, bool isNext)
        {
            if (catalogue == null) return;
            if(isNext)
            {
                switch (MusicPlayMode)
                {
                    case PlayMode.RepeatOne:
                        PrepareMusic(currentCatalogue.getCurrent());
                        break;
                    case PlayMode.RepeatList:
                        PrepareMusic(currentCatalogue.getNext());
                        break;
                    case PlayMode.Shuffle:
                        int i = random.Next();
                        PrepareMusic(currentCatalogue.getMusic(i));
                        break;
                }
            }
            else
            {
                switch (MusicPlayMode)
                {
                    case PlayMode.RepeatOne:
                    case PlayMode.RepeatList:
                        PrepareMusic(currentCatalogue.getPrevious());
                        break;
                    case PlayMode.Shuffle:
                        int i = random.Previous();
                        PrepareMusic(currentCatalogue.getMusic(i));
                        break;
                }
            }
        }

        public void ScriptSeqController(Action<MusicEntity> prepareFunc, ICatalogue catalogue, PlayMode playmode, bool isNext)
        {
            if (isNext)
            {
                prepareFunc(bsManager.StepToNext());
            }
        }

        public void GetNext()
        {
            controllerManager.CurrentController.controllerDelegation?.Invoke(PrepareMusic, currentCatalogue, MusicPlayMode, true);
        }

        public void GetPrevious()
        {
            controllerManager.CurrentController.controllerDelegation?.Invoke(PrepareMusic, currentCatalogue, MusicPlayMode, false);
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

        public void PrepareMusic(MusicEntity entity)
        {
            if (entity == null) return;
            CurrentPlaying?.DisposePicture();
            if (AudioOut.Playing || AudioOut.isLoaded) AudioOut.Stop();
            if (!bsManager.CurrentLoader.isScriptLoaded)
            {
                currentCatalogue?.SetMusic(entity);
            }
            CurrentPlaying = entity;
            MediaMetaDataReader.RetrievePictureFromCache(CurrentPlaying);
            AudioOut.Load(entity);
            AudioOut.Play();
        }

        public void PrepareMusicBytesRepresentation(MusicEntity representative, byte[] audio_data)
        {
            CurrentPlaying?.DisposePicture();
            if (AudioOut.Playing || AudioOut.isLoaded) AudioOut.Stop();
            if (!bsManager.CurrentLoader.isScriptLoaded)
            {
                currentCatalogue?.SetMusic(representative);
            }
            CurrentPlaying = representative;
            MediaMetaDataReader.RetrievePictureFromCache(CurrentPlaying);
            AudioOut.Load(representative,audio_data);
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
