using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Core;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Core.Cache;
using Lunalipse.Core.I18N;
using Lunalipse.Core.PlayList;
using Lunalipse.Core.Theme;
using Lunalipse.I18N;
using System;
using System.Reflection;
using System.Windows;

namespace Lunalipse
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        I18NTokenizer I18T;
        CacheHub cacheSystem;
        CataloguePool cp;
        ResourcesHandler resourcesHandler;
        string currentFolder = Environment.CurrentDirectory;
        LunalipseLogger Log;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            I18T = new I18NTokenizer();
            cp = CataloguePool.INSATNCE;
            cacheSystem = CacheHub.INSTANCE(Environment.CurrentDirectory);
            Log = LunalipseLogger.GetLogger();
            resourcesHandler = new ResourcesHandler(Assembly.GetEntryAssembly().GetName().Version);

            CheckResources();
            InitializeI18NEnvironemnt();
            RegisterOperators();

            RestoringCaches();
            PerpearThemeColor();
        }

        void RegisterOperators()
        {
            Log.Info("Registering cache operators..");
            cacheSystem.RegisterOperator(CacheType.MUSIC_CATALOGUE_CACHE, new MusicCacheIndexer()
            {
                UseLZ78Compress = true
            });
            cacheSystem.RegisterOperator(CacheType.LPS_SCRIPT_CACHE, new ScriptSerializor()
            {
                UseLZ78Compress = true
            });
        }

        void InitializeI18NEnvironemnt()
        {
            Log.Info("Perpearing i18n environemnt");
            TranslationManager.Instance.SetLanguage(SupportLanguages.CHINESE_SIM);
        }

        void RestoringCaches()
        {
            
        }

        void CheckResources()
        {
            Log.Info("Checking and releasing external resources");
            resourcesHandler.ReleaseResources(Assembly.GetExecutingAssembly().GetManifestResourceNames(),
                currentFolder,
                Assembly.GetExecutingAssembly());
        }

        void PerpearThemeColor()
        {
            Log.Info("Loaidng themes");
            LThemeManager.Instance.SelectTheme(0);
        }
    }
}
