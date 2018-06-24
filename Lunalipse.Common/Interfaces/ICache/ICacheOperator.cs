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
        object InvokeOperator(CacheResponseType crt, params object[] args);
        void SetCacheDir(string BaseDir);
        string OperatorUID();
    }
}
