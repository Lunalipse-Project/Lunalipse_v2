using Lunalipse.Common.Data;
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

        public static BScriptManager Instance(string scriptPath)
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
        Interpreter interpreter;

        private BScriptManager(string scriptPath)
        {
            interpreter = Interpreter.INSTANCE(scriptPath);
            foreach (string script in Directory.GetFiles(scriptPath).Where(x => x.EndsWith(".lbs")))
            {
                ScriptCollection.Add(new BScriptLocation(Path.GetFileNameWithoutExtension(script), script));
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
            interpreter.LoadPath(bScript.ScriptLocation);
        }

        public MusicEntity StepToNext()
        {
            return interpreter.LBSLoaded ? interpreter.Stepping() : null;
        }
    }

    public class BScriptLocation
    {
        string _name, _location;
        public string ScriptName { get => _name; }
        public string ScriptLocation { get => _location; }
        public BScriptLocation(string name, string location)
        {
            _name = name;
            _location = location;
        }
    }
}
