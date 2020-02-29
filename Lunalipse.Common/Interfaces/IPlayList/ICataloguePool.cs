using Lunalipse.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IPlayList
{
    public interface ICataloguePool<T> where T: ICatalogue
    {
        void AddCatalogue(T catalogue);
        bool RemoveCatalogue(T catalogue);
        void RemoveCatalogueRange(string containName);
        void RemoveCatalogue(string ID);
        List<T> SearchCatalogue(string Name);
        T GetCatalogue(string uuid);
        T GetCatalogue(int index);
        T GetCatalogueFirst(string Name, bool includeMotherCatalogue = false);
        void AddMusic(string uuid, MusicEntity music);
        void RemoveMusic(string uuid, MusicEntity music);
        void RemoveMusic(string uuid, string MusicUUID);
        void RemoveMusicRange(string uuid, string Name);
        bool Exists(Func<T, bool> condition);
    }
}
