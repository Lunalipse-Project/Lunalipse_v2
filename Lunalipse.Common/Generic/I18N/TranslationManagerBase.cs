using Lunalipse.Common.Interfaces.II18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Generic.I18N
{
    public class TranslationManagerBase
    {
        public static event Action<II18NConvertor> OnI18NEnvironmentChanged;
        protected static event Func<II18NConvertor> OnConvertorAcquired;

        protected void ChangeI18NEnvironment(II18NConvertor convertor)
        {
            OnI18NEnvironmentChanged?.Invoke(convertor);
        }

        public static II18NConvertor AquireConverter()
        {
            return OnConvertorAcquired?.Invoke();
        }
    }
}
