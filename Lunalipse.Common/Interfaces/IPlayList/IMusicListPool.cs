using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.ICommunicator;
using Lunalipse.Common.Interfaces.IMetadata;
using Lunalipse.Core.PlayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IPlayList
{
    public interface IMusicListPool
    {
        void LoadAllMusics();
        string AddToPool(string path, IProgressIndicator indicator, bool indicatorManuallyClose = false);
        List<string> AddToPool(string[] pathes);
        void DeleteMusic(MusicEntity entity, bool complete);
        bool AddFileToPool(string MediaPath);
        List<MusicEntity> GetMusics(string any, MusicEntityType metyn);
        MusicEntity GetMusic(int index);
        ICatalogue ToCatalogue();
    }
}
