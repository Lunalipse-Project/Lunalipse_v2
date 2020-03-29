using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Common.Interfaces.ISetting;
using Lunalipse.Core.Cache;
using Lunalipse.Core.Console;
using Lunalipse.Utilities;
using MinJSON.JSON;
using MinJSON.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lunalipse.Core.GlobalSetting
{
    public class GlobalSettingHelper<T> : IConsoleComponent, ISettingHelper<T> where T : IGlobalSetting
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

        string VERSION;
        public string OutputFile { get; set; }
        public bool UseLZ78Compress { get; set; }
        private GlobalSettingHelper()
        {
            VERSION = Assembly.GetEntryAssembly().GetName().Version.ToString();
            ConsoleAdapter.Instance.RegisterComponent("GlobalSettingHelper", this);
            OutputFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/config.lps";
        }

        public T ReadSetting(string path="")
        {
            if (path == "")
                path = OutputFile;
            if (!File.Exists(path)) return default(T);
            JsonObject jo = JsonObject.Parse(Encoding.UTF8.GetString(
                        Compression.Decompress(path, UseLZ78Compress)
                    ));
            SettingSaveFile<T> restored = JsonConversion.DeserializeJsonObject<SettingSaveFile<T>>(jo);
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
                            JsonConversion.SerializeJsonObject(saveFile).ToString()
                            ), OutputFile, UseLZ78Compress);
        }

        #region Command Handler
        public bool OnCommand(ILunaConsole console, params string[] args)
        {
            return false;
        }

        public void OnEnvironmentLoaded(ILunaConsole console)
        {
            throw new NotImplementedException();
        }

        public ICommandRegistry GetCommandRegistry()
        {
            throw new NotImplementedException();
        }

        public string GetContextDescription()
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    [JsonSerializable]
    public class SettingSaveFile<T>
    {
        public string version;
        public string hash;
        public T setting;
    }
}
