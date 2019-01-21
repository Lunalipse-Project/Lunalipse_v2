using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Generic.Cache
{
    [Serializable]
    public class CacheFile<T>
    {
        public string MarkName;
        public string CreateDate;
        public bool Deletable;
        public T Content;
    }
}
