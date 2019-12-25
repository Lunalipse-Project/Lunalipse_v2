using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Generic.Cache
{
    public enum CacheType
    {
        OTHER = 0,
        ALBUM_PIC = 1,
        WebAlbumPic = 2,
        PlayList = 3,
        MusicList = 4
    }

    public enum CacheResponseType
    {
        SINGLE_CACHE,
        BULK_CACHE,
        SINGLE_RESTORE,
        BULK_RESTORE,
        CACHE_EXIST,
        DELETE_CACHE,
        DELETE_ALL_CACHE
    }
}
