using System;
using System.Collections.Generic;
using System.Linq;
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
            public string markName;
            public string createDate;
            public bool deletable;
            public string HashCode;
            public bool LZ78Enc;
            /// <summary>
            /// B : Binary Serialized;
            /// S : Json Serialized
            /// </summary>
            public string FileType;
        }

        public struct SettingWrapUp
        {
            public string Version;
        }
    }
}
