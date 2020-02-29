using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Common.Interfaces.IAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IBehaviorScript
{
    public interface IScriptEngine
    {
        event Action<Exception> OnRuntimeErrorArised;
        event Action<Exception> OnSyntaxErrorArised;
        event Action<Exception> OnSemanticErrorArised;
        event Action OnScriptCompleted;
        bool isScriptLoaded { get; }
        IInterpreter ScriptInterpreter { get; }
        void SetAudioContext(IAudioContext AudioCoreContext);
        void LoadScript(BScriptLocation bScriptLocation);
        void Resume();
        void UnloadScript();
    }
}
