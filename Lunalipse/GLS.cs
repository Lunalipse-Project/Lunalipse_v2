using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Interfaces.ISetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse
{
    [Serializable]
    public class GLS : IGlobalSetting
    {
        static volatile GLS GS_Instance;
        static readonly object GS_Lock = new object();
        public static GLS INSTANCE
        {
            get
            {
                if (GS_Instance == null)
                {
                    lock (GS_Lock)
                    {
                        GS_Instance = GS_Instance ?? new GLS();
                    }
                }
                return GS_Instance;
            }
        }

        public static void SetINSTANCE(GLS Instance)
        {
            GS_Instance = Instance;
        }

        private GLS()
        {
            GS_Instance = this;
            MusicBaseDirs = new List<string>();
        }

        [ConfigField]
        public List<string> MusicBaseDirs;

        [ConfigField]
        public float Volume = 0.4f;

        [ConfigField]
        public bool LyricEnabled = true;

        [ConfigField]
        public bool FFTEnabled = true;
    }
}
