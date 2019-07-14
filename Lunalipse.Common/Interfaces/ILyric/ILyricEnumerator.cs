using Lunalipse.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.ILyric
{
    public interface ILyricEnumerator
    {
        bool AcquireLyric(MusicEntity Music);
        LyricToken Enumerating(TimeSpan current, LyricToken currentToken);
    }
}
