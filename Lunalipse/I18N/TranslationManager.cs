using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core;
using Lunalipse.Core.I18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.I18N
{
    public class TranslationManager : TranslationManagerBase
    {
        static volatile TranslationManager T_MANAGER;
        static readonly object T_MANAGER_LOCK = new object();

        public static TranslationManager Instance
        {
            get
            {
                if (T_MANAGER == null)
                {
                    lock (T_MANAGER_LOCK)
                    {
                        T_MANAGER = T_MANAGER ?? new TranslationManager();
                    }
                }
                return T_MANAGER;
            }
        }

        I18NConvertor convertor = null;
        I18NTokenizer I18T;
        I18NPages Pages;

        

        protected TranslationManager()
        {
            Pages = new I18NPages();
            I18T = new I18NTokenizer();
            Version v = Assembly.GetEntryAssembly().GetName().Version;
            if (!I18T.LoadFromFile(string.Format(@"Data\i18n{0}{1}",
                v.ToString().Replace(".", ""),
                ResourcesHandler.LUNALIPSE_DATA_FILE_EXTENSION)))
            {
                LunalipseLogger.GetLogger().Warning("Unable to load i18n environment, shutting down Lunalipse");
                System.Windows.Application.Current.Shutdown();
            }
            OnConvertorAcquired += TranslationManager_OnConvertorAcquired;
        }

        private II18NConvertor TranslationManager_OnConvertorAcquired()
        {
            return convertor;
        }

        public void SetLanguage(SupportLanguages language)
        {
            Pages.Clear();
            Pages = I18T.GetPages(language);
            convertor = new I18NConvertor(Pages);
            ChangeI18NEnvironment(convertor);
        }
    }
}
