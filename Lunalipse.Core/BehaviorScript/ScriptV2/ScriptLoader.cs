using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using Lunalipse.Core.PlayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV2
{
    public class ScriptLoader
    {
        static volatile ScriptLoader loader = null;
        static readonly object lock_object = new object();
        public static ScriptLoader Instance
        {
            get
            {
                if (loader == null)
                {
                    lock (lock_object)
                    {
                        loader = loader ?? new ScriptLoader();
                    }
                }
                return loader;
            }
        }
        
        LexicalAnalyzer lexicalAnalyzer;
        SemanticAnalyzer semanticAnalyzer;
        private Executor executor;

        public event Action<Exception> OnRuntimeErrorArised;
        public event Action OnScriptCompleted;

        public IInterpreter ScriptInterpreter { get=> executor; }

        public bool isScriptLoaded { get; private set; } = false;

        public ScriptLoader()
        {
            lexicalAnalyzer = new LexicalAnalyzer();
            semanticAnalyzer = new SemanticAnalyzer();
            executor = new Executor();
            FunctionProc.CataloguePool = CataloguePool.Instance;
            executor.CataloguePool = CataloguePool.Instance;
        }

        public void LoadScript(BScriptLocation bScriptLocation)
        {
            try
            {
                List<CodeBlock> codeBlocks = lexicalAnalyzer.ParseScript(bScriptLocation.ScriptLocation);
                semanticAnalyzer.CheckSemanticAvailability(ref codeBlocks);
                executor.CodeBlocks = codeBlocks;
                isScriptLoaded = true;
                executor.ResetExecutor();
            }
            catch(ScriptException se)
            {
                OnRuntimeErrorArised?.Invoke(se);
                isScriptLoaded = false;
            }
        }

        public void Resume()
        {
            try
            {
                if (isScriptLoaded)
                {
                    isScriptLoaded = !executor.ExecuteNextBlock();
                    if (!isScriptLoaded)
                    {
                        OnScriptCompleted?.Invoke();
                    }
                }
            }
            catch (ScriptException se)
            {
                OnRuntimeErrorArised?.Invoke(se);
                isScriptLoaded = true;
                executor.forcedStepping();
            }
        }
    }
}
