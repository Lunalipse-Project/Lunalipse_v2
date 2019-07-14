using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Misc
{
    public class BitmapAnalyser
    {
        Bitmap[] regions = new Bitmap[16];
        Rectangle cropRect = new Rectangle(0, 0, 64, 64);
        Rectangle ZoomRect = new Rectangle(0, 0, 256, 256);
        Rectangle cropLocateRect = new Rectangle(0, 0, 64, 64);
        double wfurthest = double.MinValue,
               wshorest = double.MaxValue;
        Dictionary<Color, int> counter = new Dictionary<Color, int>();
        Color background = Color.Black,
              inter = Color.Aqua,
              foreground = Color.White;

        public Color Foreground
        {
            get => foreground;
        }

        public Color Background
        {
            get => background;
        }

        public Color Intermediate
        {
            get => inter;
        }

        public void GetRegions(Bitmap bitmap)
        {
            Bitmap Reducedbitmap = Graphic.ResizeImage(bitmap, 256, 256);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    regions[4 * i + j] = new Bitmap(64, 64);
                    int x = 64 * j + 1;
                    int y = 64 * i + 1;
                    using (Graphics graphics = Graphics.FromImage(regions[4 * i + j]))
                    {
                        cropLocateRect.X = x;
                        cropLocateRect.Y = y;
                        graphics.DrawImage(Reducedbitmap, cropRect, cropLocateRect, GraphicsUnit.Pixel);
                    }
                }
            }
        }

        public void CalcHighestContrastColor(double tolerance = 25.0, int xpoints = 8, int ypoints = 8)
        {
            //Color[] avgColor = new Color[16];
            Bitmap region;
            Color c;
            int mean_xpoint = xpoints / 2;
            int mean_ypoint = ypoints / 2;
            Tuple<Color, double>[] colorWeight = new Tuple<Color, double>[16];
            //Sampling the bitmap
            for (int i = 0; i < regions.Length; i++)
            {
                region = regions[i];
                for (int j = 1, y = 4; j <= 8; j++, y = ypoints * j - mean_ypoint)
                {
                    for (int k = 1, x = 4; k <= 8; k++, x = xpoints * k - mean_xpoint)
                    {
                        c = region.GetPixel(x, y);
                        if (hasShortestDistance(counter, tolerance, c, out Color key))
                            counter[key]++;
                        else
                            counter.Add(c, 1);
                    }
                }
                Color avgc = counter.OrderByDescending(x => x.Value).First().Key;
                double r = ColorSystem.DistanceBetweenSquare(avgc, Color.White);
                colorWeight[i] = new Tuple<Color, double>(avgc, r);
                counter.Clear();
            }
            Sort(ref colorWeight);
            background = colorWeight[0].Item1;
            inter = colorWeight[7].Item1;
            foreground = colorWeight[15].Item1;
        }

        private void Sort(ref Tuple<Color, double>[] src)
        {
            double max = double.MinValue;
            int maxIndex = 0, sorted = 0;
            while (sorted < src.Length)
            {
                maxIndex = sorted;
                for(int i = sorted; i < src.Length; i++)
                {
                    if (src[i].Item2 >= max)
                    {
                        max = src[i].Item2;
                        maxIndex = i;
                    }
                }
                Tuple<Color, double> temp = src[maxIndex];
                src[maxIndex] = src[sorted];
                src[sorted] = temp;
                sorted++;
                max = double.MinValue;
            }
        }

        public bool hasShortestDistance(Dictionary<Color, int> dict, double maxDist, Color target, out Color minKey)
        {
            double Mininum = double.MaxValue;
            Color minK = Color.Black;
            maxDist *= maxDist;
            foreach (KeyValuePair<Color, int> kv in dict)
            {
                double result = ColorSystem.DistanceBetweenSquare(kv.Key, target);
                if (result < Mininum)
                {
                    Mininum = result;
                    minK = kv.Key;
                }
            }
            minKey = minK;
            if (Mininum <= maxDist) return true;
            return false;
        }
    }
}
