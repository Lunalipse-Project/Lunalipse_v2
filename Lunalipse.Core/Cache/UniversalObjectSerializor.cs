using Lunalipse.Common.Generic.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.Cache
{
    public class UniversalObjectSerializor<T>
    {
        public static byte[] Serialize(T ObjectSerialize)
        {
            byte[] binaryArr;
            BinaryFormatter binFormatter = new BinaryFormatter();
            using (MemoryStream MemStream = new MemoryStream())
            {
                binFormatter.Serialize(MemStream, ObjectSerialize);
                binaryArr = MemStream.ToArray();
            }
            return binaryArr;
        }

        public static T Deserialize(byte[] content)
        {
            T result;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream MemStream = new MemoryStream(content))
            {
                result = (T)binaryFormatter.Deserialize(MemStream);
            }
            return result;
        }
    }
}
