using Lunalipse.Common.Generic.Themes;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Lunalipse.Core.Theme
{
    public class LThemeManager : ThemeManagerBase
    {
        protected List<ThemeContainer> LoadedTheme;
        private LThemeParser TParser;
        private ThemeContainer SelectedContainer;

        static volatile LThemeManager JTM_INSTANCE = null;
        static readonly object LOCK = new object();

        bool UseOtherTheme = false;
        private ThemeTuple ControledTuple = null;


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



        protected override void Reflush()
        {
            InvokEvent(SelectedContainer.ColorBlend);
            UseOtherTheme = false;
        }

        public override void Reflush(ThemeTuple themeTuple)
        {
            UseOtherTheme = true;
            InvokEvent(ControledTuple = themeTuple);
        }

        public void Restore()
        {
            if (UseOtherTheme)
                Reflush();
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
            LoadedTheme.Add(CreateBuiltIn_NightmareMoon());
            TParser.LoadAllTheme();
            LoadedTheme.AddRange(TParser.Tuples);
        }

        private ThemeTuple JThemeManager_OnTupleAcquire()
        {
            if (!UseOtherTheme)
            {
                if (SelectedContainer != null) return SelectedContainer.ColorBlend;
                return null;
            }
            else
                return ControledTuple;
        }

        private ThemeContainer CreateBuiltIn_Luna()
        {
            LinearGradientBrush LunaStyledGradient = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop((Color)ColorConverter.ConvertFromString("#373A77"),0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#656CB9"),1)
            }, 45);
            ThemeTuple tt = new ThemeTuple(Brushes.White, LunaStyledGradient, "#FF81B9FF".ToColor().ToBrush());
            return new ThemeContainer()
            {
                Name = "Princess Luna",
                Description = "Lunalipse Built-In Theme (Default). Adapted from Princess Luna in My Little Pony",
                ColorBlend = tt,
                isBuildIn = true,
                Uid = "d21d0d06-4583-463c-b949-c2b40978ee7a"
            };
        }

        private ThemeContainer CreateBuiltIn_NightmareMoon()
        {
            ThemeTuple tt = new ThemeTuple(Brushes.White,"#FF000000".ToColor().ToBrush().ToCelestia(0.08f), "#FF3B49AB".ToColor().ToBrush());
            return new ThemeContainer()
            {
                Name = "Nightmare Moon",
                Description = "Lunalipse Built-In Theme (Default). Adapted from Nightmare Moon in My Little Pony",
                ColorBlend = tt,
                isBuildIn = true,
                Uid = "521bbea0-4e4e-45c2-8aad-bf0b80befed4 "
            };
        }

        private ThemeContainer CreateBuiltIn_Celestia()
        {
            LinearGradientBrush CelestiaStyledGradient = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop((Color)ColorConverter.ConvertFromString("#F5ADFF"),0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#93B9FF"),0.5),
                new GradientStop((Color)ColorConverter.ConvertFromString("#64DCB7"),1),
            }, 45);
            ThemeTuple tt = new ThemeTuple("#FFFDF5FB".ToColor().ToBrush(), CelestiaStyledGradient, "#FFE18FE4".ToColor().ToBrush());
            return new ThemeContainer()
            {
                Name = "Princess Celestia",
                Description = "Lunalipse Built-In Theme. Adapted from Princess Celestia in My Little Pony。",
                ColorBlend = tt,
                isBuildIn = true,
                Uid = "c087d6b3-f4e2-4d44-a906-cb1085d58fb5",
                author = "Lunaixsky"
            };
        }

        
    }
}
