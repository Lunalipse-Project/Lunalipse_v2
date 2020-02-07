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
        /// Tis snow of the winter.
        /// We wrap them up together.
        /// For the clean of summer.
        /// And none upon the flower
        /// </summary>
        public struct WinterWrapUp
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string comment;
            public uint createDate;
            public byte num_of_sealed;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] offsets;
        }

        public struct CacheFileInfo
        {
            public CacheType cacheType;
            public string id;
        }

    }
}
