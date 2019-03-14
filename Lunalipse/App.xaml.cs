using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Core;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Core.Cache;
using Lunalipse.Core.GlobalSetting;
using Lunalipse.Core.I18N;
using Lunalipse.Core.PlayList;
using Lunalipse.Core.Theme;
using Lunalipse.I18N;
using Lunalipse.Utilities;
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
            Log = LunalipseLogger.GetLogger();
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            I18T = new I18NTokenizer();
            cp = CataloguePool.INSATNCE;
            cacheSystem = CacheHub.INSTANCE(Environment.CurrentDirectory);
            resourcesHandler = new ResourcesHandler(Assembly.GetEntryAssembly().GetName().Version);
            RestoringConfig();

            CheckResources();
            InitializeI18NEnvironemnt();
            RegisterOperators();

            
            PerpearThemeColor();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Exception(e.Exception);
            Log.Release();
            e.Handled = true;
            MessageBox.Show(e.Exception.Message, "致命错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }

        void RegisterOperators()
        {
            Log.Info("Registering caching operators..");
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
            SupportLanguages supportLanguages;
            if(GLS.INSTANCE.UseSystemDefaultLanguage)
            {
                supportLanguages = TranslationManager.GetSystemLanguage();
            }
            else
            {
                if(!Enum.TryParse(GLS.INSTANCE.CurrentSelectedLanguage,out supportLanguages))
                {
                    supportLanguages = SupportLanguages.CHINESE_SIM;
                    Log.Warning("Unknow Language '{0}'. Use default".FormateEx(GLS.INSTANCE.CurrentSelectedLanguage));
                }
            }
            TranslationManager.Instance.SetLanguage(supportLanguages);
        }

        void RestoringConfig()
        {
            GLS.SetINSTANCE(GlobalSettingHelper<GLS>.INSTANCE.ReadSetting());
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
            LThemeManager.Instance.SelectTheme(GLS.INSTANCE.DefaultThemeUUID);
        }
    }
}
