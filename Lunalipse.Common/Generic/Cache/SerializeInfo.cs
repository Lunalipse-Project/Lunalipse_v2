using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Generic.Cache
{
    public class SerializeInfo
    {
        public const string CACHE_FILE_EXT = ".lce";

        /// <summary>
        /// The snow of the winter.
        /// We wrap 'em up together.
        /// For the clean of summer.
        /// And none upon the flower
        /// </summary>
        public struct WinterWrapUp
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string comment;
            public uint createDate;
            public byte reserved;
            public int dataSize;
        }

        public struct CacheFileInfo
        {
            public CacheType cacheType;
            public string id;
        }

    }
}
