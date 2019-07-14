using Lunalipse.Common;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.IThemes;
using Lunalipse.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace Lunalipse.Core.Theme
{
    public class LThemeParser : IJThemeParser
    {
        private readonly string ENV_PATH;

        LunalipseLogger Log;
        public List<ThemeContainer> Tuples { get; private set; } = null;

        public event Action<string, string[]> ErrorRaised;

        public LThemeParser()
        {
            ENV_PATH = Environment.CurrentDirectory;
            Tuples = new List<ThemeContainer>();
            Log = LunalipseLogger.GetLogger();
            if (!Directory.Exists(ENV_PATH + "/Themes"))
                Directory.CreateDirectory(ENV_PATH + "/Themes");
        }

        public bool LoadAllTheme()
        {
            Tuples.Clear();
            try
            {
                foreach(var file in Directory.GetFiles(ENV_PATH+"/Themes"))
                {
                    try
                    {
                        ThemeContainer tc = CreateThemeFromJson(file);
                        Tuples.Add(tc);
                        LunalipseLogger.GetLogger().Info("Loaded Theme " + tc.Name);
                        //if (Path.GetExtension(file) == ".lstyle")
                        //{
                        // TODO
                        //}
                    }
                    catch (JsonSerializationException e)
                    {
                        Log.Error(e.Message, e.StackTrace);
                        Log.Warning("Error to load selected theme, skipping...");
                    }
                }
                return true;
            }
            catch(FileNotFoundException)
            {
                Log.Warning("Theme File Not Found.");
                Log.Warning("Error to load selected theme, use default.");
                return false;
            }
        }

        private string _load_j(string path)
        {
            string ThemeJson = "";
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader sreader = new StreamReader(fs, Encoding.UTF8))
                {
                    ThemeJson = sreader.ReadToEnd();
                }
            }
            return ThemeJson;
        }

        private ThemeContainer CreateThemeFromJson(string path)
        {
            ThemeBody themeBody = JsonConvert.DeserializeObject<ThemeBody>(_load_j(path));
            ThemeContainer themeContainer = new ThemeContainer()
            {
                author = themeBody.themeInfo.Author,
                Name = themeBody.themeInfo.Name,
                Description = themeBody.themeInfo.Description,
                Uid = themeBody.themeInfo.UUID
            };
            Brush F = GetThemeComponentValue(themeBody.Forground, themeBody);
            Brush P = GetThemeComponentValue(themeBody.Primary, themeBody);
            Brush S = GetThemeComponentValue(themeBody.Secondary, themeBody);
            ThemeTuple themeTuple = new ThemeTuple(F, P, S);
            themeContainer.ColorBlend = themeTuple;
            return themeContainer;
        }


        private Color ToColor(string colorToken)
        {
            if (colorToken.ToLower() == "#transparent") return Colors.Transparent;
            return colorToken.ToColor();
        }

        private SolidColorBrush ToBrush(ThemeColor themeColor)
        {
            SolidColorBrush solidColorBrush = new SolidColorBrush(themeColor.ColorValue.ToColor());
            if (themeColor.ColorOpacity > 1 || themeColor.ColorOpacity < 0)
                ErrorRaised?.Invoke("CORE_THEMEPARSER_ERR_INVALIDRANGE", new string[]{ "cOpacity", "0", "1"});
            else
                solidColorBrush.Opacity = themeColor.ColorOpacity;
            return solidColorBrush;
        }

        private LinearGradientBrush ToGradient(Gradient gradient)
        {
            LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
            System.Windows.Point start = ToPoint(gradient.Start);
            System.Windows.Point end = ToPoint(gradient.End);
            linearGradientBrush.StartPoint = start;
            linearGradientBrush.EndPoint = end;
            foreach(ThemeColor themeColor in gradient.GradientStops)
            {
                linearGradientBrush.GradientStops.Add(new GradientStop(ToColor(themeColor.ColorValue), themeColor.GradientOffset));
            }
            linearGradientBrush.Opacity = gradient.ColorOpacity;
            linearGradientBrush.ColorInterpolationMode = gradient.colorInterpolationMode;
            linearGradientBrush.SpreadMethod = gradient.gradientSpreadMethod;
            linearGradientBrush.MappingMode = gradient.gradientMappingMode;
            return linearGradientBrush;
        }

        private System.Windows.Point ToPoint(TPoint point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }

        private Brush GetThemeComponentValue(ThemeColor themeColor,ThemeBody themeBody)
        {
            Brush brush = null;
            if(!string.IsNullOrEmpty(themeColor.ColorRefer))
            {
                Gradient gradient = null;
                if(themeBody.gradients.ContainsKey(themeColor.ColorRefer))
                {
                    gradient = themeBody.gradients[themeColor.ColorRefer];
                    brush = ToGradient(gradient);
                }
                else
                {
                    ErrorRaised?.Invoke("CORE_THEMEPARSER_ERR_COLORNOTFOUND", new string[] { themeColor.ColorRefer });
                }
            }
            else
            {
                brush = ToBrush(themeColor);
            }
            return brush;
        }

        /*
         * JSON FORMAT
         * // GLOBAL: APPLY THE STYLE TO ALL COMPONENTS
         * 
         * Theme:(
         *      Name: STRING(),
         *      Disc: STRING()
         * ),
         * {Primary|Secondary}: STRING(FORMAT[HEX]),
         * [Optional]{Foreground}: STRING(FORMAT[HEX])
         * 
         */

    }

    class ThemeBody
    {
        [JsonProperty("Theme")]
        public ThemeInfo themeInfo;
        [JsonProperty("Foreground")]
        public ThemeColor Forground;
        [JsonProperty("Primary")]
        public ThemeColor Primary;
        [JsonProperty("Secondary")]
        public ThemeColor Secondary;
        [JsonProperty("gDefinitions")]
        public Dictionary<string,Gradient> gradients;
    }
    class ThemeInfo
    {
        [JsonProperty("Name")]
        public string Name;
        [JsonProperty("Desc")]
        public string Description;
        [JsonProperty("Author")]
        public string Author;
        [JsonProperty("Uid")]
        public string UUID;
    }
    class Gradient
    {
        //[JsonProperty("gKey")]
        //public string Key;
        [JsonProperty("gStart")]
        public TPoint Start;
        [JsonProperty("gEnd")]
        public TPoint End;
        [JsonProperty("gInterpolation")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ColorInterpolationMode colorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;
        [JsonProperty("gSpread")]
        [JsonConverter(typeof(StringEnumConverter))]
        public GradientSpreadMethod gradientSpreadMethod = GradientSpreadMethod.Pad;
        [JsonProperty("gMapping")]
        [JsonConverter(typeof(StringEnumConverter))]
        public BrushMappingMode gradientMappingMode = BrushMappingMode.RelativeToBoundingBox;
        [JsonProperty("gStops")]
        public ThemeColor[] GradientStops;
        [JsonProperty("gOpacity")]
        public double ColorOpacity = 1d;
    }
    class ThemeColor
    {
        /// <summary>
        /// This will refer to a "key" value of gradient if it is defined
        /// </summary>
        [JsonProperty("cRefer")]
        public string ColorRefer;
        [JsonProperty("cValue")]
        public string ColorValue;
        [JsonProperty("cOpacity")]
        public double ColorOpacity = 1;
        [JsonProperty("gOffset")]
        public double GradientOffset;
    }
    class TPoint
    {
        [JsonProperty("x")]
        public double X;
        [JsonProperty("y")]
        public double Y;
    }
}
