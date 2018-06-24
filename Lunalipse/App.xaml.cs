using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Core.Cache;
using Lunalipse.Core.I18N;
using Lunalipse.Core.PlayList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            I18T = new I18NTokenizer();
            CataloguePool cp = CataloguePool.INSATNCE;
            cacheSystem = CacheHub.INSTANCE(Environment.CurrentDirectory);

            InitializeI18NEnvironemnt();
            RegisterOperators();

            RestoringCaches();
        }

        void RegisterOperators()
        {
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
            if (!I18T.LoadFromFile(@"Data\i18n.lang"))
            {
                Current.Shutdown();
            }
            I18T.GetPages(SupportLanguages.CHINESE_SIM);
        }

        void RestoringCaches()
        {
            if (cacheSystem.ComponentCacheExists(CacheType.MUSIC_CATALOGUE_CACHE))
            {
                foreach (Catalogue cat in cacheSystem.RestoreObjects<Catalogue>(
                    x => x.markName == "CATALOGUE", 
                    CacheType.MUSIC_CATALOGUE_CACHE))
                {
                    cp.AddCatalogue(cat);
                }
}
        }
    }
}
