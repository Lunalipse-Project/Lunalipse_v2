using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Generic.Cache
{
    public enum CacheType
    {
        MUSIC_CATALOGUE_CACHE,
        LPS_SCRIPT_CACHE
    }

    public enum CacheResponseType
    {
        SINGLE_CACHE,
        BULK_CACHE,
        SINGLE_RESTORE,
        BULK_RESTORE,
        FIELD_CACHE,
        FIELD_RESTORE
    }
}
