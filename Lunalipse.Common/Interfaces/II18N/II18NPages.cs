using Lunalipse.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.II18N
{
    public interface II18NPages
    {
        II18NCollection GetPage(SupportedPages pageName);
        bool AddPage(string name, II18NCollection pageCollection);
        bool DropPage(string name);
    }
}
