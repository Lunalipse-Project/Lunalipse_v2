using Lunalipse.Common.Generic.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.ICache
{
    public interface ICacheOperator
    {
        object InvokeOperator(CacheResponseType crt,object data_to_cache, params object[] args);
        void SetCacheDir(string BaseDir);
    }
}
