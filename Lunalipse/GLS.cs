using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Interfaces.ISetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Lunalipse
{
    [Serializable]
    public class GLS : IGlobalSetting
    {
        static volatile GLS GS_Instance;
        static readonly object GS_Lock = new object();

        [field: NonSerialized]
        public event Action<string> OnSettingUpdated;
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

        [ConfigField]
        public string DefaultThemeUUID = "d21d0d06-4583-463c-b949-c2b40978ee7a";

        /// <summary>
        /// 是否在切换歌曲时提示歌曲名称
        /// </summary>
        [ConfigField]
        public bool ShowNextSongHint = true;

        [ConfigField]
        public string LyricFontFamily = "Microsoft YaHei UI";

        [NonSerialized]
        public FontFamily LyricFontFamilyInternal;

        [ConfigField]
        public int LyricFontSize = 40;

        [ConfigField]
        public string CurrentSelectedLanguage = "CHINESE_SIM";

        [ConfigField]
        public bool UseSystemDefaultLanguage = true;

        /// <summary>
        /// 是否启用列表懒加载
        /// </summary>
        [ConfigField]
        public bool UseLazyLoading = false;

        /// <summary>
        /// 启用毛玻璃效果
        /// </summary>
        [ConfigField]
        public bool EnableGuassianBlur = true;

        /// <summary>
        /// 网络代理，是否启用
        /// </summary>
        [ConfigField]
        public bool EnableProxy = false;

        /// <summary>
        /// 网络代理，IP地址
        /// </summary>
        [ConfigField]
        public string ProxyIPAddr = "";

        /// <summary>
        /// 网络代理，端口
        /// </summary>
        [ConfigField]
        public int ProxyPort = 0;

        /// <summary>
        /// 主题颜色跟随专辑封面
        /// </summary>
        [ConfigField]
        public bool ThemeColorFollowAlbum = true;

        [ConfigField]
        public string LogRecordLevel =
#if DEBUG
            "DEBUG";
#else
            "INFO";
#endif

        public void InvokeSettingChange(string settingkey)
        {
            OnSettingUpdated?.Invoke(settingkey);
        }
    }
}
