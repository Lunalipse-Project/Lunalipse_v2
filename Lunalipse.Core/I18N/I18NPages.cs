using Lunalipse.Utilities;
using Lunalipse.Common.Interfaces.II18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunalipse.Common.Data;

namespace Lunalipse.Core.I18N
{
    /// <summary>
    /// 适用于国际化的页面集合。
    /// </summary>
    public class I18NPages : II18NPages
    {
        public I18NPages() {}
        private Dictionary<string, II18NCollection> Pages = new Dictionary<string, II18NCollection>();
        public bool AddPage(string name, II18NCollection pageCollection)
        {
            return Pages.AddNonRepeat(name, pageCollection);
        }

        public bool DropPage(string name)
        {
            return Pages.Remove(name);
        }

        public void Clear()
        {
            Pages.Clear();
        }

        public II18NCollection GetPage(SupportedPages pageName)
        {
            if (Pages.ContainsKey(pageName.ToString())) return Pages[pageName.ToString()];
            return null;
        }
    }
}
