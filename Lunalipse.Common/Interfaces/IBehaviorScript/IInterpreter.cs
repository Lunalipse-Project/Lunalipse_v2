using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.IPlayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IBehaviorScript
{
    public interface IInterpreter
    {
        event Action OnInstructionFinished;
        int ExecutionStackDepth { get; }
        int CurrentStackPointer { get; }
        string CurrentContextIdentifier { get; }
        InterpreterStatus GetInterpreterStatus { get; }
    }
}
