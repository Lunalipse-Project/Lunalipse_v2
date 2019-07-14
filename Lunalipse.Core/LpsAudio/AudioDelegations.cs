using Lunalipse.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // FFT Delegations
        public delegate float[] FFTDataAcquired();
        public delegate int FFTIndexAcquired(float freq);

        public static OnStatuesChanged StatuesChanged;
        public static OnPositionChanged PostionChanged;
        public static OnMusicLoaded MusicLoaded;
        public static OnMusicPlayingFinished PlayingFinished;
        public static event OnLyricUpdate LyricUpdated;
        public static OnLyricLoadStatus LyricLoadStatus;
        public static VolumeChangeInvoke ChangeVolume;

        public static FFTDataAcquired FftAcquired;
        public static FFTIndexAcquired FftInxAcquired;

        public static void InvokeLyricUpdate(LyricToken token)
        {
            LyricUpdated?.Invoke(token);
        }
    }
}
