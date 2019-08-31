using Lunalipse.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.ILyric
{
    public interface ILyricTokenizer
    {
        List<LyricToken> CreateTokens(string lyrics);
        List<LyricToken> CreateTokensFromFile(string path);
        List<LyricToken> CreateTokensFromWeb(string uri);

    }
}
