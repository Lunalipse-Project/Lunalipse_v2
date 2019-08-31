using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Data.Errors;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Common.Interfaces.ISetting;
using Lunalipse.Core.Cache;
using Lunalipse.Core.Console;
using Lunalipse.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Lunalipse.Core.GlobalSetting
{
    public class GlobalSettingHelper<T> : ComponentHandler, ISettingHelper<T> where T : IGlobalSetting
    {
        static volatile GlobalSettingHelper<T> GSH_INSTANCE;
        static readonly object GSH_LOCK = new object();
        public static GlobalSettingHelper<T> Instance
        {
            get
            {
                if (GSH_INSTANCE == null)
                    lock (GSH_LOCK)
                        GSH_INSTANCE = GSH_INSTANCE ?? new GlobalSettingHelper<T>();
                return GSH_INSTANCE;
            }
        }

        UniversalSerializor<NonConfigField, IGlobalSetting> USerializor;
        string VERSION;
        public string OutputFile { get; set; }
        public bool UseLZ78Compress { get; set; }
        private GlobalSettingHelper()
        {
            VERSION = Assembly.GetEntryAssembly().GetName().Version.ToString();
            ConsoleAdapter.Instance.RegisterComponent("lpsseting", this);
            //Set default value
            OutputFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/config.lps";
            //USerializor = new UniversalSerializor<NonConfigField, IGlobalSetting>();
        }

        public T ReadSetting(string path="")
        {
            if (path == "")
                path = OutputFile;
            if (!File.Exists(path)) return default(T);
            SettingSaveFile<T> restored = JsonConvert.DeserializeObject<SettingSaveFile<T>>(
                Encoding.UTF8.GetString(
                    Compression.Decompress(path, UseLZ78Compress)
                    )
                );
            if(VERSION != restored.version)
            {
                // TODO Add warning dialog
            }
            //return (T)USerializor.ReadNested(typeof(T), jo["ctx"] as JObject);
            return restored.setting;
        }

        public bool SaveSetting(T instance)
        {
            SettingSaveFile<T> saveFile = new SettingSaveFile<T>();
            saveFile.version = VERSION;
            saveFile.hash = instance.ComputeHash();
            saveFile.setting = instance;
            return Compression.Compress(
                        Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(saveFile)
                            ), OutputFile, UseLZ78Compress);
        }

        #region Command Handler
        public override bool OnCommand(params string[] args)
        {
            return true;
        }
        #endregion
    }

    [Serializable]
    public class SettingSaveFile<T>
    {
        public string version;
        public string hash;
        public T setting;
    }
}
