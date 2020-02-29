using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IPlayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IAudio
{
    public interface IAudioContext
    {
        void SetCatalogue(ICatalogue catalogue);
        void PrepareMusic(MusicEntity entity);
        void PrepareMusicBytesRepresentation(MusicEntity representative, byte[] audio_data);
        ICatalogue GetCurrentCatalogue();
        void Pause();
        void Resume();
        void SetVolume(double volume);
        void ApplyEqualizerSetting(double[] equalizer_values);
    }
}
