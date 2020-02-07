using CSCore.DSP;
using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Generic.Audio;
using Lunalipse.Common.Interfaces.ISetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Lunalipse
{
    [Serializable]
    public class GLS : IGlobalSetting
    {
        [NonConfigField]
        static volatile GLS GS_Instance;
        [NonConfigField]
        static readonly object GS_Lock = new object();

        [field: NonSerialized]
        [NonConfigField]
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
            SpectrumDisplayers = new Dictionary<string, SpectrumDisplayCfg>();
        }

        [field: NonSerialized]
        public string UpdateArguments = string.Empty;

        public List<string> MusicBaseDirs;

        public float Volume = 0.4f;

        public bool LyricEnabled = true;

        public bool FFTEnabled = true;

        public string DefaultThemeUUID = "d21d0d06-4583-463c-b949-c2b40978ee7a";

        /// <summary>
        /// 是否在切换歌曲时提示歌曲名称
        /// </summary>
        public bool ShowNextSongHint = true;

        public string LyricFontFamily = "Microsoft YaHei UI";

        [field: NonSerialized]
        public FontFamily LyricFontFamilyInternal;

        public int LyricFontSize = 40;

        public string CurrentSelectedLanguage = "CHINESE_SIM";

        public bool UseSystemDefaultLanguage = true;

        /// <summary>
        /// 是否启用列表懒加载
        /// </summary>
        public bool UseLazyLoading = false;

        /// <summary>
        /// 启用毛玻璃效果
        /// </summary>
        public bool EnableGuassianBlur = true;

        /// <summary>
        /// 网络代理，是否启用
        /// </summary>
        public bool EnableProxy = false;

        /// <summary>
        /// 网络代理，IP地址
        /// </summary>
        public string ProxyIPAddr = string.Empty;

        /// <summary>
        /// 网络代理，端口
        /// </summary>
        public int ProxyPort = -1;

        /// <summary>
        /// 主题颜色跟随专辑封面
        /// </summary>
        public bool ThemeColorFollowAlbum = true;

        public bool ImmerseMode = false;

        public ScalingStrategy scalingStrategy = ScalingStrategy.Decibel;

        public FftSize fftSize = FftSize.Fft4096;

        public int SpectrumFPS = 64;

        public int AudioLatency = 100;

        public string LogRecordLevel =
#if DEBUG
            "DEBUG";
#else
            "INFO";
#endif
        public string SelectedController = string.Empty;

        public double[] EqualizerSets = new double[10];

        public Dictionary<string, SpectrumDisplayCfg> SpectrumDisplayers;

        public void InvokeSettingChange(string settingkey)
        {
            OnSettingUpdated?.Invoke(settingkey);
        }

        [field:NonSerialized]
        private WebProxy _webProxy;

        public WebProxy ProxySetting
        {
            get
            {
                if (EnableProxy) return _webProxy;
                return null;
            }
        }
        public void PrepareProxy()
        {
            if (ProxyIPAddr == string.Empty || ProxyPort <= 0) return;
            _webProxy = new WebProxy(ProxyIPAddr, ProxyPort);
            //_webProxy.UseDefaultCredentials = true;
        }
    }

    [Serializable]
    public class SpectrumDisplayCfg
    {
        public string Style = "";
        public int Resolution = 0;
    }
}
