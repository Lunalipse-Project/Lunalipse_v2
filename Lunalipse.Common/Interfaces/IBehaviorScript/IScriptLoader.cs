using Lunalipse.Common.Data.BehaviorScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IBehaviorScript
{
    public interface IScriptLoader
    {
        event Action<Exception> OnErrorArised;
        event Action OnScriptCompleted;
        bool isScriptLoaded { get; }
        IExecutor ScriptExecutor { get; }

        void LoadScript(BScriptLocation bScriptLocation);
        void GoNext();
    }
}
