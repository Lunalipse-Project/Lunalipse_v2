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
    public class ScriptLoader : IScriptLoader
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

        public event Action<Exception> OnErrorArised;
        public event Action OnScriptCompleted;

        public IExecutor ScriptExecutor { get=> executor; }

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
                OnErrorArised?.Invoke(se);
                isScriptLoaded = false;
            }
        }

        public void GoNext()
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
                OnErrorArised?.Invoke(se);
                isScriptLoaded = true;
                executor.forcedStepping();
            }
        }
    }
}
