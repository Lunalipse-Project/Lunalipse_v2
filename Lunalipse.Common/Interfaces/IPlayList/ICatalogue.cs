using Lunalipse.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Lunalipse.Common.Interfaces.IPlayList
{
    public interface ICatalogue
    {
        string Uid();
        string Name();
        bool AddMusic(MusicEntity ME);
        bool DeleteMusic(MusicEntity ME);
        bool DeleteMusic(string name);
        bool DeleteMusic(int index);
        bool DeleteMusic(int start, int count);
        bool IsUserDefined();
        int GetCount();
        MusicEntity getMusic(int index);
        MusicEntity getMusic(string name);
        MusicEntity getNext();
        MusicEntity getCurrent();
        List<MusicEntity> SearchMusic(string name);
        void SortByYear();
        void SortByAlbum();
        void SortByName();
        BitmapSource GetCatalogueCover();
        IEnumerable<MusicEntity> GetAll();

        
    }
}
