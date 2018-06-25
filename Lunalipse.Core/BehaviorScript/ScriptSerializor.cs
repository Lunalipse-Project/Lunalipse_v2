using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Core.Cache;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript
{
    public class ScriptSerializor : ICacheOperator
    {
        const string OP_UID = "lbS";
        public string SavedPath { get; set; }

        public bool UseLZ78Compress { get; set; }

        UniversalSerializor<Cachable, ICachable> USerializor;

        public ScriptSerializor()
        {
            USerializor = new UniversalSerializor<Cachable, ICachable>();
        }

        public object InvokeOperator(CacheResponseType crt, params object[] args)
        {
            switch (crt)
            {
                case CacheResponseType.SINGLE_CACHE:
                    return USerializor.WriteSingleField(args[0], args[1] as string);
                case CacheResponseType.SINGLE_RESTORE:
                    return USerializor.ReadSingleField(args[0] as JObject, args[1] as string, args[2]);
                default:
                    return null;
            }
        }

        public string OperatorUID()
        {
            return OP_UID;
        }

        public bool SaveTo(string fileName)
        {
            throw new NotImplementedException();
        }

        public void SetCacheDir(string BaseDir)
        {
            SavedPath = BaseDir;
        }
    }
}
