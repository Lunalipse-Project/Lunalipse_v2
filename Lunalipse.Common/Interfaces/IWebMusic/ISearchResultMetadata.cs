using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IWebMusic
{
    public interface ISearchResultMetadata
    {
        List<IWebMusicDetail> GetMusics();
    }
}
