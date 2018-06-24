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
        string UID();
        bool AddMusic(MusicEntity ME);
        bool DeleteMusic(MusicEntity ME);
        bool DeleteMusic(string name);
        bool DeleteMusic(int index);
        bool DeleteMusic(int start, int count);
        int GetCount();
        MusicEntity getMusic(int index);
        MusicEntity getMusic(string name);
        MusicEntity getNext();
        List<MusicEntity> SearchMusic(string name);
        void SortByYear();
        void SortByAlbum();
        void SortByName();
        BitmapSource GetCatalogueCover();
        IEnumerable<MusicEntity> GetAll();
    }
}
