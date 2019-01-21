using Lunalipse.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.II18N
{
    public interface II18NConvertor
    {
        string ConvertTo(SupportedPages page, string key);
        string ConvertTo(SupportedPages page, string key, params object[] replace);
    }
}
