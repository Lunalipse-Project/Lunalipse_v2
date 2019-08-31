using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Misc
{
    public class BitmapAnalyser
    {
        const int IMAGE_TARGET_SIZE = 32;
        const int IMAGE_SAMPLE_COUNT = 4;
        static readonly int SAMPLE_CHOP_SIZE = IMAGE_TARGET_SIZE / IMAGE_SAMPLE_COUNT;
        static readonly int CHOP_SIZE_SQUARED = SAMPLE_CHOP_SIZE * SAMPLE_CHOP_SIZE;


        Rectangle rectangle = new Rectangle(0, 0, SAMPLE_CHOP_SIZE, SAMPLE_CHOP_SIZE);
        Rectangle origin = new Rectangle(0, 0, SAMPLE_CHOP_SIZE, SAMPLE_CHOP_SIZE);
        Color background = Color.Black,
              inter = Color.Aqua,
              foreground = Color.White;

        List<Color> bitmapList = new List<Color>();

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

        public void ResetAll()
        {
            rectangle.X = 0;
            rectangle.Y = 0;
            bitmapList.Clear();
        }

        public void CalcHighestContrastColor(Bitmap bitmap)
        {
            ResetAll();
            Bitmap Reducedbitmap = Graphic.ResizeImage(bitmap, IMAGE_TARGET_SIZE, IMAGE_TARGET_SIZE);
            for (int i = 0; i < IMAGE_SAMPLE_COUNT; i++)
            {
                for (int j = 0; j < IMAGE_SAMPLE_COUNT; j++)
                {
                    rectangle.X = SAMPLE_CHOP_SIZE * j + 1;
                    rectangle.Y = SAMPLE_CHOP_SIZE * i + 1;
                    Bitmap bitmap_parts = new Bitmap(SAMPLE_CHOP_SIZE, SAMPLE_CHOP_SIZE);

                    using (Graphics graphics = Graphics.FromImage(bitmap_parts))
                    {
                        graphics.DrawImage(Reducedbitmap, origin, rectangle, GraphicsUnit.Pixel);
                    }
                    Color color = GetAvgColor(bitmap_parts, origin);
                    bitmapList.Add(color);
                }
            }
            FurthestColors furthestColors = GetDisguishedColor2(bitmapList);
            inter = furthestColors.intermedian;
            foreground = furthestColors.foreground;
            background = furthestColors.background;
        }

        Color GetAvgColor(Bitmap bitmap, Rectangle rectangle)
        {
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int stride = bitmapData.Stride;
            int[] rgb = new int[3] { 0, 0, 0 };
            unsafe
            {
                byte* data = (byte*)(void*)bitmapData.Scan0;
                for (int y = 0; y < SAMPLE_CHOP_SIZE; y++)
                {
                    for (int x = 0; x < SAMPLE_CHOP_SIZE; x++)
                    {
                        rgb[0] += data[(y * stride) + x * 4 + 0];
                        rgb[1] += data[(y * stride) + x * 4 + 1];
                        rgb[2] += data[(y * stride) + x * 4 + 2];
                    }
                }
            }
            
            rgb[0] = rgb[0] / CHOP_SIZE_SQUARED;
            rgb[1] = rgb[1] / CHOP_SIZE_SQUARED;
            rgb[2] = rgb[2] / CHOP_SIZE_SQUARED;
            return Color.FromArgb(0xff, (byte)rgb[2], (byte)rgb[1], (byte)rgb[0]);
        }

        FurthestColors GetDisguishedColor(List<Color> bitmapList)
        {

            FurthestColors furthestColors = new FurthestColors();
            List<ColorDist> distance = new List<ColorDist>();
            int count = bitmapList.Count;
            for (int i = 0; i < count; i++)
            {
                distance.Add(new ColorDist()
                {
                    distance = ColorSystem.DistanceBetweenSquare(bitmapList[i], Color.White),
                    i = i
                });
            }
            List<ColorDist> distanceList = distance.OrderBy(x => x.distance).ToList();
            furthestColors.background = bitmapList[distanceList[count - 1].i];
            furthestColors.foreground = bitmapList[distanceList[0].i];
            furthestColors.intermedian = bitmapList[(int)Math.Floor(count / 2d)];
            return furthestColors;
        }

        FurthestColors GetDisguishedColor2(List<Color> bitmapColorList)
        {
            FurthestColors furthestColors = new FurthestColors();
            List<ColorDist> distance = new List<ColorDist>();
            int count = bitmapColorList.Count;
            for (int i = 0; i < count - 1; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    distance.Add(new ColorDist()
                    {
                        distance = ColorSystem.DistanceBetweenSquare(bitmapColorList[i], bitmapColorList[j]),
                        i = i,
                        j = j
                    });
                }
            }
            List<ColorDist> cdist = distance.OrderByDescending(x => x.distance).ToList();
            int backgroundIndex = cdist[0].i;
            Color c1 = bitmapColorList[cdist[0].i];
            Color c2 = bitmapColorList[cdist[0].j];
            furthestColors.background = c1;
            if (ColorSystem.DistanceBetweenSquare(c1, Color.White) < ColorSystem.DistanceBetweenSquare(c2, Color.White))
            {
                furthestColors.background = c2;
                backgroundIndex = cdist[0].j;
            }
            ColorDist colorDist = cdist[(int)Math.Round(cdist.Count / 2d)];
            furthestColors.intermedian = bitmapColorList[colorDist.i];
            if (colorDist.i == backgroundIndex)
            {
                furthestColors.intermedian = bitmapColorList[colorDist.j];
            }
            furthestColors.foreground = furthestColors.background.GetForeground();
            return furthestColors;
        }
    }

    class ColorDist
    {
        public double distance;
        public int i;
        public int j;
    }

    class FurthestColors
    {
        public Color background;
        public Color foreground;
        public Color intermedian;
    }
}
