using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IPlayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IBehaviorScript
{
    public interface IExecutor
    {
        event Action OnMovingNext;
        MusicEntity CurrentMusicEntity { get; }
        ICatalogue CurrentCatalogue { get; }
        string CurrentCode { get; }
        string CurrentCodeParsed { get; }

        int currentPointer { get; }
        int currentInnerPointer { get; }
    }
}
