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
using Lunalipse.Utilities;
using System.Runtime.InteropServices;

namespace Lunalipse.Core.Cache
{
    public class CacheSerializor : ICache
    {
        public CacheSerializor()
        {

        }

        public byte[] CacheToBin<T>(T instance, WinterWrapUp wwu)
        {
            byte[] content = UniversalObjectSerializor<T>.Serialize(instance);
            wwu.dataSize = content.Length;
            byte[] header = wwu.ToBytes();
            byte[] file = new byte[content.Length + header.Length];
            Array.Copy(header, 0, file, 0, header.Length);
            Array.Copy(content, 0, file, header.Length, content.Length);
            return file;
        }

        public T BinRestoreTo<T>(byte[] bytes)
        {
            int headerSize = Marshal.SizeOf(typeof(WinterWrapUp));
            //Since we do not need any information in header, so just skip it
            byte[] content = new byte[bytes.Length - headerSize];
            Array.Copy(bytes, headerSize, content, 0, content.Length);
            return UniversalObjectSerializor<T>.Deserialize(content);
        }


    }
}
