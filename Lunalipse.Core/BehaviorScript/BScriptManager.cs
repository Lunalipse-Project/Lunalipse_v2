using Lunalipse.Common.Data;
using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Core.BehaviorScript.ScriptV1;
using Lunalipse.Core.BehaviorScript.ScriptV2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript
{
    public class BScriptManager
    {
        static volatile BScriptManager bsManagerInstance = null;
        static readonly object InstanceLock = new object();

        public static BScriptManager Instance(string scriptPath = "")
        {
            if(bsManagerInstance==null)
            {
                lock(InstanceLock)
                {
                    bsManagerInstance = bsManagerInstance ?? new BScriptManager(scriptPath);
                }
            }
            return bsManagerInstance;
        }

        public List<BScriptLocation> ScriptCollection { get; private set; } = new List<BScriptLocation>();
        public IScriptLoader CurrentLoader { get; private set; }
        public BScriptLocation LoadedScript { get; private set; }

        string scriptPath;

        private BScriptManager(string scriptPath)
        {
            if (scriptPath != "")
            {
                this.scriptPath = scriptPath;
                if (!Directory.Exists(scriptPath))
                {
                    Directory.CreateDirectory(scriptPath);
                }
                //interpreter = Interpreter.INSTANCE(scriptPath);
                CurrentLoader = ScriptLoader.Instance;
                foreach (string script in Directory.GetFiles(scriptPath).Where(x => x.EndsWith(".lbs")))
                {
                    ScriptCollection.Add(new BScriptLocation(Path.GetFileNameWithoutExtension(script), script));
                }
            }
        }

        public void DeleteScript(string scriptName)
        {
            ScriptCollection.RemoveAt(ScriptCollection.FindIndex(x => x.ScriptName.Equals(scriptName)));
        }

        public void AddScript(string scriptPath)
        {
            ScriptCollection.Add(new BScriptLocation(Path.GetFileNameWithoutExtension(scriptPath), scriptPath));
        }

        public void LoadScript(string Name)
        {
            BScriptLocation bScript = ScriptCollection.Find(x => x.ScriptName.Equals(Name));
            LoadedScript = bScript;
            CurrentLoader.LoadScript(bScript);
            LpsAudio.AudioDelegations.PlayingFinished?.Invoke();
        }

        public MusicEntity StepToNext()
        {
            CurrentLoader.GoNext();
            return CurrentLoader.ScriptExecutor.CurrentMusicEntity;
        }

        public ICatalogue UsingCatalogue
        {
            get => CurrentLoader.ScriptExecutor.CurrentCatalogue;
        }
    }
}
