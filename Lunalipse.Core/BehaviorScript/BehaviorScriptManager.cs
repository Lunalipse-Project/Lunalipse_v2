using Lunalipse.Common;
using Lunalipse.Common.Data;
using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Common.Interfaces.IAudio;
using Lunalipse.Common.Interfaces.IBehaviorScript;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Core.BehaviorScript.ScriptV3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript
{
    public class BehaviorScriptManager
    {
        static volatile BehaviorScriptManager bsManagerInstance = null;
        static readonly object InstanceLock = new object();

        public static BehaviorScriptManager Instance(IAudioContext audioCore = null)
        {
            if(bsManagerInstance==null)
            {
                lock(InstanceLock)
                {
                    if (bsManagerInstance == null)
                    {
                        if(audioCore == null)
                        {
                            throw new ArgumentNullException("Script Manager must be initialized with an instance of LpsCore.");
                        }
                        bsManagerInstance = new BehaviorScriptManager(audioCore);
                    }
                }
            }
            return bsManagerInstance;
        }

        public List<BScriptLocation> ScriptCollection { get; private set; } = new List<BScriptLocation>();
        public IScriptEngine CurrentLoader { get; private set; }
        public BScriptLocation LoadedScript { get; private set; }

        string scriptPath;

        IAudioContext audioCore;

        private BehaviorScriptManager(IAudioContext core)
        {
            if (scriptPath != "")
            {
                this.scriptPath = AppConst.APP_EXE_DIRECTORY + "/Scripts/";
                audioCore = core;
                if (!Directory.Exists(scriptPath))
                {
                    Directory.CreateDirectory(scriptPath);
                }
                CurrentLoader = new LetterEngineV3(audioCore);
                foreach (string script in Directory.GetFiles(scriptPath).Where(x => x.EndsWith(".letter")))
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

        public void ResumeExecution()
        {
            CurrentLoader.Resume();
        }
    }
}
