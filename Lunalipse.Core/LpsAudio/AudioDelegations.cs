using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IVisualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Lunalipse.Core.LpsAudio
{
    public class AudioDelegations
    {
        // Controler Delegations
        //------- [Out] -------
        public delegate void OnStatuesChanged(bool isPlaying);
        public delegate void OnPositionChanged(TimeSpan curPos);
        public delegate void OnMusicLoaded(MusicEntity Music,Track mTrack);
        public delegate void OnMusicPlayingFinished();
        public delegate void OnLyricUpdate(LyricToken Token);
        public delegate void OnLyricLoadStatus(bool isSucess);
        //------- [In] --------
        public delegate void VolumeChangeInvoke(float volume);


        public static OnStatuesChanged StatuesChanged;
        public static OnPositionChanged PostionChanged;
        public static OnMusicLoaded MusicLoaded;
        public static OnMusicPlayingFinished PlayingFinished;
        public static event OnLyricUpdate LyricUpdated;
        public static OnLyricLoadStatus LyricLoadStatus;
        public static VolumeChangeInvoke ChangeVolume;

        public static event Action<float[]> OnFftDataUpdate;

        public static Func<float[]> FftAcquired;
        /// <summary>
        /// Arg: Frequency
        /// Return: Index
        /// </summary>
        public static Func<float, int> FftInxAcquired;

        public static void InvokeLyricUpdate(LyricToken token)
        {
            LyricUpdated?.Invoke(token);
        }

        public static void UpdateFftData(float[] Lines)
        {
            OnFftDataUpdate?.Invoke(Lines);
        }

        public static bool HasSubscribersSpectrum()
        {
            if (OnFftDataUpdate == null) return false;
            return OnFftDataUpdate.GetInvocationList().Length > 0;
        }
    }
}
