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
            public object uid;
        }

        public struct SettingWrapUp
        {
            public string Version;
        }
    }
}
