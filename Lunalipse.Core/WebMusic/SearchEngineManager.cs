using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting;
using Lunalipse.Common.Interfaces.IWebMusic;
using Lunalipse.Utilities;

namespace Lunalipse.Core.WebMusic
{
    public class SearchEngineManager
    {
        static volatile SearchEngineManager engines_instance;
        static readonly object engines_lock = new object();

        public static SearchEngineManager Instance
        {
            get
            {
                if (engines_instance == null)
                {
                    lock(engines_lock)
                    {
                        engines_instance = engines_instance ?? new SearchEngineManager();
                    }
                }
                return engines_instance;
            }
        }

        Dictionary<string, IWebMusicsSearchEngine> Engines = new Dictionary<string, IWebMusicsSearchEngine>();
        //AppDomain EngineDomain;
        
        public IWebMusicsSearchEngine CurrentSelected { get; private set; }
        public event Action<EngineActionType> OnRequesting;
        public event Action<EngineActionType> OnRespond;
        public event Action<Exception> OnError;

        public bool hasError { get; private set; }
        LunalipseLogger logger;

        public string ApplicationBaseDir { get; set; }

        private SearchEngineManager()
        {
            logger = LunalipseLogger.GetLogger();
        }

        //public void InitializeEngineDomain()
        //{
        //    EngineDomain = AppDomain.CreateDomain("EngineEnv", null, new AppDomainSetup
        //    {
        //        ApplicationBase = $"{ApplicationBaseDir}\\SearchEngines"
        //    });
        //    EngineDomain.Load(typeof(Lunalipse.Common.AppConst).Assembly);
        //    EngineDomain.Load(typeof(LunaNetCore.LNetC).Assembly.FullName);
        //    EngineDomain.Load(typeof(Newtonsoft.Json.JsonConvert).Assembly.FullName);
        //}
        //public void LoadEngines()
        //{
        //    string directory = $"{ApplicationBaseDir}\\SearchEngines";
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //        return;
        //    }
        //    foreach (string file in Directory.GetFiles(directory, "*.wse", SearchOption.TopDirectoryOnly))
        //    {
        //        ObjectHandle objectHandle = EngineDomain.CreateInstanceFrom(file, $"{Path.GetFileNameWithoutExtension(file)}.MainEngine");
        //        IWebMusicsSearchEngine webMusicsSearchEngine = objectHandle.Unwrap() as IWebMusicsSearchEngine;
        //        EngineEntryPoint ep = null;
        //        if((ep = webMusicsSearchEngine.GetType().GetCustomAttribute(typeof(EngineEntryPoint)) as EngineEntryPoint) != null)
        //        {
        //            RegisterEngine(ep.EngineName, webMusicsSearchEngine);
        //        }
        //    }
        //}

        public void RegisterEngine(string engineID, IWebMusicsSearchEngine engineInstance)
        {
            Engines.AddNonRepeat(engineID, engineInstance);
        }

        public void SelectEngine(string engineID)
        {
            if (Engines.ContainsKey(engineID))
            {
                CurrentSelected?.OnUnload();
                CurrentSelected = Engines[engineID];
                CurrentSelected.OnLoad();

                UpdateHandlers();
                LunalipseLogger.GetLogger().Info($"Selected engine: {engineID}");
            }
        }

        public void UpdateHandlers()
        {
            CurrentSelected.SetOnQueryRequesting(InvokeRequesting);
            CurrentSelected.SetOnQueryReturned(InvokeRespond);
            CurrentSelected.SetExceptionRaised(InvokeErrorRaised);
        }

        public void UpdateProxySetting(IWebProxy proxy)
        {
            Engines.ForEach((key, engine) => engine.SetProxy(proxy));
        }

        public string[] getAllLoadedEngineIDs()
        {
            return Engines.Keys.ToArray();
        }

        //public void UnloadEngines()
        //{
        //    Engines.Clear();
        //    AppDomain.Unload(EngineDomain);
        //}

        private void InvokeRequesting(EngineActionType actionType)
        {
            hasError = false;
            OnRequesting?.Invoke(actionType);
        }
        private void InvokeRespond(EngineActionType actionType)
        {
            hasError = false;
            OnRespond?.Invoke(actionType);
        }
        private void InvokeErrorRaised(Exception e)
        {
            hasError = true;
            logger.Error($"At current selected search engine. An exception was thrown: \"{e.Message}\"");
            OnError?.Invoke(e);
        }
    }
}
