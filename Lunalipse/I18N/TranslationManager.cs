using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core;
using Lunalipse.Core.I18N;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        string baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

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
            if (!I18T.LoadFromFile(string.Format(@"{2}\Data\i18n{0}{1}",
                v.ToString().Replace(".", ""),
                ResourcesHandler.LUNALIPSE_DATA_FILE_EXTENSION, baseDir)))
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

        public static SupportLanguages GetSystemLanguage()
        {
            CultureInfo cultureInfo = CultureInfo.InstalledUICulture;
            string identifier = cultureInfo.Name;
            if (identifier.StartsWith("en")) return SupportLanguages.ENGLISH;
            else if (identifier == "zh-CN") return SupportLanguages.CHINESE_SIM;
            else if (identifier == "zh-TW" || identifier == "zh-HK") return SupportLanguages.CHINESE_TRA;
            else if (identifier == "ru-RU") return SupportLanguages.RUSSIAN;
            else LunalipseLogger.GetLogger().Warning("Language or location : {0} is not supported yet, use default.".FormateEx(identifier));
            return SupportLanguages.CHINESE_SIM;
        }
    }
}
