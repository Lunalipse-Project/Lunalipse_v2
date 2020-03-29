using Lunalipse.Common.Interfaces.ICache;
using System;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;
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
            return CachePureByteArray(content, wwu);
        }

        public byte[] CachePureByteArray(byte[] content, WinterWrapUp winterWrapUp)
        {
            byte[] header = winterWrapUp.ToBytes();
            byte[] file = new byte[content.Length + header.Length];
            Array.Copy(header, 0, file, 0, header.Length);
            Array.Copy(content, 0, file, header.Length, content.Length);
            return file;
        }

        public T BinRestoreTo<T>(byte[] bytes, out WinterWrapUp winterWrapUp)
        {
            return UniversalObjectSerializor<T>.Deserialize(GetByteArrayContent(bytes, out winterWrapUp));
        }

        public byte[] GetByteArrayContent(byte[] raw, out WinterWrapUp winterWrapUp)
        {
            int headerSize = Marshal.SizeOf(typeof(WinterWrapUp));
            byte[] wwu = new byte[headerSize];
            byte[] content = new byte[raw.Length - headerSize];

            // Retrive WinterWrapUp header
            Array.Copy(raw, 0, wwu, 0, headerSize);
            winterWrapUp = wwu.ToStruct<WinterWrapUp>();

            Array.Copy(raw, headerSize, content, 0, content.Length);
            return content;
        }

    }
}
