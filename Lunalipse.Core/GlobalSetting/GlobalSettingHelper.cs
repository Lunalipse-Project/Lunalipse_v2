using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Data.Errors;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Common.Interfaces.ISetting;
using Lunalipse.Core.Cache;
using Lunalipse.Core.Console;
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
        public static GlobalSettingHelper<T> INSTANCE
        {
            get
            {
                if (GSH_INSTANCE == null)
                    lock (GSH_LOCK)
                        GSH_INSTANCE = GSH_INSTANCE ?? new GlobalSettingHelper<T>();
                return GSH_INSTANCE;
            }
        }

        UniversalSerializor<ConfigField, IGlobalSetting> USerializor;
        string VERSION;
        public string OutputFile { get; set; }
        public bool UseLZ78Compress { get; set; }
        private GlobalSettingHelper()
        {
            VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ConsoleAdapter.INSTANCE.RegisterComponent("lpsseting", this);
            //Set default value
            OutputFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/config.lps";
            UseLZ78Compress = true;
            USerializor = new UniversalSerializor<ConfigField, IGlobalSetting>();
        }

        public T ReadSetting(string path="")
        {
            if (path == "")
                path = OutputFile;
            if (!File.Exists(path)) return default(T);
            JObject jo = JObject.Parse(Encoding.UTF8.GetString(Compression.Decompress(path, UseLZ78Compress)));

            return (T)USerializor.ReadNested(typeof(T), jo["ctx"] as JObject);
        }

        public bool SaveSetting(T instance)
        {
            JObject jo = new JObject();
            jo["version"] = VERSION;
            jo["ctx"] = USerializor.WriteNested(instance);
            return Compression.Compress(Encoding.UTF8.GetBytes(jo.ToString()), OutputFile, UseLZ78Compress);
        }

        #region Command Handler
        public override bool OnCommand(params string[] args)
        {
            return true;
        }
        #endregion
    }
}
