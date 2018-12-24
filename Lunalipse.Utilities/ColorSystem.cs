using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Lunalipse.Utilities
{
    public static class ColorSystem
    {
        const float LUNA_RATIO = 0.15f;
        const float CELESTIA_RATIO = 0.15f;

        public static SolidColorBrush Darken(this SolidColorBrush brush, float percent)
        {
            return new SolidColorBrush(brush.Color.Concentrate(percent));
        }

        public static Color Concentrate(this Color brush, float percent)
        {
            percent = 1 - percent;
            int r = (int)Math.Round(brush.R * percent);
            int g = (int)Math.Round(brush.G * percent);
            int b = (int)Math.Round(brush.B * percent);
            return Color.FromArgb(brush.A, (byte)r, (byte)g, (byte)b);
        }

        public static Color Dilute(this Color brush, float percent)
        {
            int r = (int)Math.Round(brush.R + (255 - brush.R) * percent);
            int g = (int)Math.Round(brush.G + (255 - brush.G) * percent);
            int b = (int)Math.Round(brush.B + (255 - brush.B) * percent);
            return Color.FromArgb(brush.A, (byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// 给定背景色返回一个合适的前景色
        /// </summary>
        /// <param name="background">背景色</param>
        /// <param name="tolerance">容差，取值范围[0,1]，如果为正，则取白色的颜色范围会变大，为负则相反</param>
        /// <returns></returns>
        public static Color GetForeground(this Color background,double tolerance=0)
        {
            if (tolerance < 0 && tolerance > 100) tolerance = 0;
            if (background.Distance() > 220.8*(1- tolerance)) return Colors.White;
            else return Colors.Black;
        }

        public static SolidColorBrush GetForegroundBrush(this SolidColorBrush brush,double tolerance = 0)
        {
            return new SolidColorBrush(brush.Color.GetForeground(tolerance));
        }

        public static double Distance(this Color color)
        {
            int dr = 255 - color.R;
            int dg = 255 - color.G;
            int db = 255 - color.B;
            return Math.Sqrt(dr * dr + dg * dg + db * db);
        }

        /// <summary>
        /// 转换为深色主题颜色
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static SolidColorBrush ToLuna(this SolidColorBrush brush)
        {
            return new SolidColorBrush(brush.Color.ToLuna());
        }

        /// <summary>
        /// 转换为淡色主题颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SolidColorBrush ToCelestia(this SolidColorBrush brush)
        {
            return new SolidColorBrush(brush.Color.ToCelestia());
        }

        /// <summary>
        /// 转换为深色主题颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToLuna(this Color color)
        {
            return color.Concentrate(LUNA_RATIO);
        }

        /// <summary>
        /// 设置颜色的透明度
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="opacity">透明度，范围是[0,1]</param>
        /// <returns></returns>
        public static SolidColorBrush SetOpacity(this SolidColorBrush brush, double opacity)
        {
            return new SolidColorBrush(brush.Color.SetOpacity(opacity));
        }

        /// <summary>
        /// 转换为淡色主题颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToCelestia(this Color color)
        {
            return color.Dilute(CELESTIA_RATIO);
        }

        public static SolidColorBrush ToBrush(this Color color)
        {
            return new SolidColorBrush(color);
        }

        public static Color ToColor(this string colorInHex)
        {
            return (Color)ColorConverter.ConvertFromString(colorInHex);
        }

        public static Color SetOpacity(this Color color,double opacity)
        {
            if (opacity < 0 || opacity > 1) return color;
            int alpha = (int)Math.Round(opacity * 255);
            return Color.FromArgb((byte)alpha, color.R, color.G, color.B);
        }
    }
}
