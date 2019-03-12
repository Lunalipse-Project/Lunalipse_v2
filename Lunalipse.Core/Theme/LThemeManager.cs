using Lunalipse.Common.Generic.Themes;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.Theme
{
    public class LThemeManager : ThemeManagerBase
    {
        protected List<ThemeContainer> LoadedTheme;
        private LThemeParser TParser;
        private ThemeContainer SelectedContainer;

        static volatile LThemeManager JTM_INSTANCE = null;
        static readonly object LOCK = new object();

        public static LThemeManager Instance
        {
            get
            {
                if (JTM_INSTANCE == null)
                {
                    lock (LOCK)
                    {
                        JTM_INSTANCE = JTM_INSTANCE ?? new LThemeManager();
                    }
                }
                return JTM_INSTANCE;
            }
        }

        private LThemeManager()
        {
            TParser = new LThemeParser();
            LoadedTheme = new List<ThemeContainer>();
            OnTupleAcquire += JThemeManager_OnTupleAcquire;
            Initialize();
        }

        public void SelectTheme(int index)
        {
            if (index < 0 || index > LoadedTheme.Count) return;
            SelectedContainer = LoadedTheme[index];
            LunalipseLogger.GetLogger().Info("Select Theme " + SelectedContainer.Name);
            Reflush();
        }
        public void SelectTheme(string uid)
        {
            SelectedContainer = LoadedTheme.Find(x => x.Uid.Equals(uid));
            if(SelectedContainer == null)
            {
                LunalipseLogger.GetLogger().Info("Theme with UID:{0} not found, using default..".FormateEx(uid));
                SelectTheme(0);
                return;
            }
            LunalipseLogger.GetLogger().Info("Select Theme " + SelectedContainer.Name);
            Reflush();
        }

        public override void Reflush()
        {
            InvokEvent(SelectedContainer.ColorBlend);
        }

        public List<ThemeContainer> GetLoadedThemes()
        {
            return LoadedTheme;
        }

        private void Initialize()
        {
            LoadedTheme.Clear();
            LoadedTheme.Add(CreateBuiltIn_Luna());
            LoadedTheme.Add(CreateBuiltIn_Celestia());
            TParser.LoadAllTheme();
            LoadedTheme.AddRange(TParser.Tuples);
        }

        private ThemeTuple JThemeManager_OnTupleAcquire()
        {
            if (SelectedContainer != null) return SelectedContainer.ColorBlend;
            return null;
        }

        private ThemeContainer CreateBuiltIn_Luna()
        {
            ThemeTuple tt = new ThemeTuple();
            tt.Primary = "#CC2449BE".ToColor().ToBrush();
            tt.Secondary = "#FF4AA8A8".ToColor().ToBrush();
            tt.Foreground = tt.Primary.GetForegroundBrush();
            tt.Surface = "#FF8E8AC4".ToColor().ToBrush();
            return new ThemeContainer()
            {
                Name = "Princess Luna",
                Description = "Lunalipse Built-In Theme (Default). Adapted from Princess Luna in My Little Pony",
                ColorBlend = tt,
                isBuildIn = true,
                Uid = "d21d0d06-4583-463c-b949-c2b40978ee7a"
            };
        }

        private ThemeContainer CreateBuiltIn_Celestia()
        {
            ThemeTuple tt = new ThemeTuple();
            tt.Foreground = "#FF474747".ToColor().ToBrush();
            tt.Secondary = "#EEF84CFF".ToColor().ToBrush();
            tt.Primary = "#CCFDF5FB".ToColor().ToBrush();
            tt.Surface = "#FF93B9FF".ToColor().ToBrush();
            return new ThemeContainer()
            {
                Name = "Princess Celestia",
                Description = "Lunalipse Built-In Theme. Adapted from Princess Celestia in My Little Pony。 (仍然处在测试中)",
                ColorBlend = tt,
                isBuildIn = true,
                Uid= "c087d6b3-f4e2-4d44-a906-cb1085d58fb5"
            };
        }

        
    }
}
