using Lunalipse.Common.Interfaces.ICache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Lunalipse.Common.Data.Attribute;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;
using System.Collections;
using System.Runtime.Serialization;
using Lunalipse.Common.Generic.Cache;

namespace Lunalipse.Core.Cache
{
    public class CacheSerializor : ICache
    {
        UniversalSerializor<Cachable, ICachable> USerializor;
        public CacheSerializor()
        {
            USerializor = new UniversalSerializor<Cachable, ICachable>();
        }
        public string CacheTo<T>(T instance, WinterWrapUp wwu, string fieldName = null) where T : ICachable
        {
            JObject final = new JObject();
            final["name"] = wwu.markName;
            final["date"] = wwu.createDate;
            final["clean"] = wwu.deletable;
            final["ctx"] = fieldName == null ?
                    USerializor.WriteNested(instance).ToString() : 
                    USerializor.WriteSingleField(instance, fieldName).ToString();
            return final.ToString();
        }

        public byte[] CacheToBin<T>(T instance, WinterWrapUp wwu)
        {
            CacheFile<T> cache = new CacheFile<T>()
            {
                Content = instance,
                CreateDate = wwu.createDate,
                Deletable = wwu.deletable,
                MarkName = wwu.markName
            };
            return UniversalObjectSerializor<CacheFile<T>>.Serialize(cache);
        }

        public T RestoreTo<T>(object ctx) where T : ICachable
        {
            return (T)USerializor.ReadNested(typeof(T), (JObject)ctx);
        }



        public object RestoreField<T>(T ancestor, object ctx, string fieldName)
        {
            return USerializor.ReadSingleField((JObject)ctx, fieldName, ancestor);
        }

        public T BinRestoreTo<T>(byte[] bytes)
        {
            CacheFile<T> restored = UniversalObjectSerializor<CacheFile<T>>.Deserialize(bytes);
            return restored.Content;
        }
    }
}
