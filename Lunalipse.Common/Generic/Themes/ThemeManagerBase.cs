using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Generic.Themes
{
    public class ThemeManagerBase
    {
        public static event Action<ThemeTuple> OnThemeApplying;
        protected static event Func<ThemeTuple> OnTupleAcquire;

        protected string ENV_PATH = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        protected virtual void Reflush()
        {
            
        }

        public virtual void Reflush(ThemeTuple themeTuple)
        {

        }

        public virtual void Reload()
        {

        }

        public static ThemeTuple AcquireSelectedTheme()
        {
            return OnTupleAcquire?.Invoke();
        }

        public IEnumerable<string> GetAvailableThemes()
        {
            foreach (string fi in Directory.GetFiles(ENV_PATH + "/Themes"))
            {
                yield return Path.GetFileNameWithoutExtension(fi);
            }
        }

        protected void InvokEvent(ThemeTuple t_tuple) => OnThemeApplying?.Invoke(t_tuple);
    }
}

