using Lunalipse.Common.Generic.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Common.Interfaces.ICache
{
    public interface ICacheHub
    {
        bool CacheObject<T>(T obj, CacheType type, params object[] aux_data);
        bool CacheObjects<T>(List<T> obj, CacheType type, params object[] aux_data);
        bool CacheField<T>(T ancestor, CacheType type, string FieldName);

        T RestoreObject<T>(string id, CacheType type);
        object RestoreField(Func<WinterWrapUp, bool> Conditions, CacheType type, string FieldName);
        IEnumerable<T> RestoreObjects<T>(CacheType type);

        void DeleteCaches(CacheType cacheType);
        void DeleteCache(CacheType cacheType, string id);

    }
}
