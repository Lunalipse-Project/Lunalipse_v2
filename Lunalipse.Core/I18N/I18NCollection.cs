using Lunalipse.Utilities;
using Lunalipse.Common.Interfaces.II18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.I18N
{
    /// <summary>
    /// 适用于国际化页面的字段值
    /// </summary>
    public class I18NCollection : II18NCollection
    {
        public Dictionary<string, string> Maps = new Dictionary<string, string>();
        public bool AddToCollection(string indexer, string context)
        {
            return Maps.AddNonRepeat(indexer, context);
        }

        public string getContext(string indexer)
        {

            if (Maps.ContainsKey(indexer)) return Maps[indexer];
            return indexer;
        }

        public bool RemoveFromCollection(string indexer)
        {
            return Maps.Remove(indexer);
        }

        public bool SetCollectionElement(string indexer, string NewContext)
        {
            if (Maps.ContainsKey(indexer))
            {
                Maps[indexer] = NewContext;
                return true;
            }
            return false;
        }
    }
}
