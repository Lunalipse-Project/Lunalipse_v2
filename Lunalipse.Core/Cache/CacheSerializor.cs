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

namespace Lunalipse.Core.Cache
{
    public class CacheSerializor : ICache
    {
        UniversalSerializor<Cachable, ICachable> USerializor;
        public CacheSerializor()
        {
            USerializor = new UniversalSerializor<Cachable, ICachable>();
        }
        public string CacheTo<T>(T instance, WinterWrapUp cw, string fieldName = null) where T : ICachable
        {
            JObject final = new JObject();
            final["name"] = cw.markName;
            final["date"] = cw.createDate;
            final["clean"] = cw.deletable;
            final["ctx"] = fieldName == null ?
                    USerializor.WriteNested(instance).ToString() : 
                    USerializor.WriteSingleField(instance, fieldName).ToString();
            return final.ToString();
        }

        public T RestoreTo<T>(object ctx) where T : ICachable
        {
            return (T)USerializor.ReadNested(typeof(T), (JObject)ctx);
        }

        public object RestoreField<T>(T ancestor, object ctx, string fieldName)
        {
            return USerializor.ReadSingleField((JObject)ctx, fieldName, ancestor);
        }
    }
}
