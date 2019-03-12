using Lunalipse.Common;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.IThemes;
using Lunalipse.Utilities;
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
                    ThemeContainer tc = parseTheme(file);
                    Tuples.Add(tc);
                    LunalipseLogger.GetLogger().Info("Loaded Theme " + tc.Name);
                }
                return true;
            }
            catch(FileNotFoundException)
            {
                Log.Warning("Theme File Not Found.");
                return false;
            }
            catch(Exception e)
            {
                Log.Error(e.Message, e.StackTrace);
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
        #region
        /*
        private List<ThemeTuple> _parse_t(string path)
        {
            List<ThemeTuple> tuples = new List<ThemeTuple>();
            JObject jobj = JObject.Parse(_load_j(path));
            foreach(var jt in jobj)
            {
                JToken theme = jt.Value;
                if (!theme.HasValues) continue;
                JObject componentBody = (JObject)theme;

                ThemeTuple tt = new ThemeTuple();
                tt.ComponentID = jt.Key;
                foreach(JToken field in componentBody.Children())
                {
                    if (field.HasValues) continue;
                    switch (((JProperty)field).Name)
                    {
                        case "Primary":
                            tt.PrimaryColor = CreateBrush(field as JObject) ?? tt.PrimaryColor;
                            break;
                        case "Secondary":
                            tt.SecondaryColor = CreateBrush(field as JObject) ?? tt.SecondaryColor;
                            break;
                        case "Tertiary":
                            tt.TertiaryColor = CreateBrush(field as JObject) ?? tt.TertiaryColor;
                            break;
                    }
                }
                tuples.Add(tt);
            }
            return tuples;
        }*/
        #endregion

        private ThemeContainer parseTheme(string path)
        {
            JObject json = JObject.Parse(_load_j(path));
            if (!json.ContainsKey("Theme")) return null;
            ThemeContainer tc = new ThemeContainer();
            ThemeTuple tt = new ThemeTuple();
            foreach (JProperty field in json.Properties())
            {
                string NodeName = field.Name;
                if (NodeName=="Theme")
                {
                    foreach(JProperty inner in field.First.Children<JProperty>())
                    {
                        string name = inner.Name;
                        switch (name)
                        {
                            case "Name":
                                tc.Name = inner.Value.Value<string>();
                                break;
                            case "Desc":
                                tc.Description = inner.Value.Value<string>();
                                break;
                            case "Uid":
                                tc.Uid = inner.Value.Value<string>();
                                break;
                        }
                    }
                }
                else
                {
                    switch(NodeName)
                    {
                        case "Primary":
                            tt.Primary = new SolidColorBrush(ToColor(field.Value.Value<string>()));
                            break;
                        case "Secondary":
                            tt.Secondary = new SolidColorBrush(ToColor(field.Value.Value<string>()));
                            break;
                        case "Foreground":
                            tt.Foreground = new SolidColorBrush(ToColor(field.Value.Value<string>()));
                            break;
                        case "Surface":
                            tt.Surface = new SolidColorBrush(ToColor(field.Value.Value<string>()));
                            break;
                    }
                }
            }
            if (tt.Primary == null || tt.Secondary == null) return null;
            if (tt.Surface == null)
                tt.Surface = tt.Primary;
            if (tt.Foreground == null) tt.Foreground = tt.Primary.GetForegroundBrush();
            tc.ColorBlend = tt;
            return tc;
        }

        private Color ToColor(string colorToken)
        {
            if (colorToken.ToLower() == "#transparent") return Colors.Transparent;
            return colorToken.ToColor();
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

        /*                        [Obsolete]
         * JSON FORMAT
         * // GLOBAL: APPLY THE STYLE TO ALL COMPONENTS
         * {COMPONENT_NAME | GLOBAL}: (
         *      {PRIMARY_COLOR | SECONDARY_COLOR | TERTIARY_COLOR} :(
         *          VALUE: STRING(
         *              SELECTABLE[
         *                  FORMAT[HEX],
         *                  TRANSPARENT
         *              ]
         *          ),
         *          OPACITY: FLOAT(
         *              RANGE[0-1]
         *          )
         *          INHERIT: STRING(
         *              SELECTABLE[
         *                  COMPONENT_NAME
         *              ]
         *          )
         *      )
         * )
         * 
         */

    }
}
