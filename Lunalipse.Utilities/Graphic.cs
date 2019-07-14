
using Lunalipse.Utilities.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Lunalipse.Utilities
{
    public class Graphic
    {
        private static readonly ImageCodecInfo JpegCodec;

        static Graphic()
        {
            JpegCodec = ImageCodecInfo.GetImageEncoders().First(encoder => encoder.MimeType == "image/jpeg");
        }

        public static byte[] Image2ByteArray(string filename)
        {
            byte[] byteArray = null;

            try
            {
                if (string.IsNullOrEmpty(filename))
                    return null;

                Bitmap bmp = default(Bitmap);

                using (Bitmap tempBmp = new Bitmap(filename))
                {
                    bmp = new Bitmap(tempBmp);
                }

                ImageConverter converter = new ImageConverter();

                byteArray = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
            }
            catch (Exception)
            {
                throw;
            }

            return byteArray;
        }

        public static void Image2File(Image img, ImageCodecInfo codec, string filename, int width, int height,
            long qualityPercent)
        {
            var encoderParams = new EncoderParameters
            {
                Param = { [0] = new EncoderParameter(Encoder.Quality, qualityPercent) }
            };

            Image scaledImg = null;
            try
            {
                if (width > 0 && height > 0)
                {
                    scaledImg = img.GetThumbnailImage(width, height, null, IntPtr.Zero);
                    img = scaledImg;
                }

                if (File.Exists(filename))
                    File.Delete(filename);
                img.Save(filename, codec, encoderParams);
            }
            finally
            {
                scaledImg?.Dispose();
            }
        }

        public static void Byte2ImageFile(byte[] imageData, ImageCodecInfo codec, string filename, int width, int height,
            long qualityPercent)
        {
            using (var ms = new MemoryStream(imageData))
            {
                using (var img = Image.FromStream(ms))
                {
                    Image2File(img, codec, filename, width, height, qualityPercent);
                }
            }
        }

        public static void Byte2Jpg(byte[] imageData, string filename, int width, int height, long qualityPercent)
        {
            Byte2ImageFile(imageData, JpegCodec, filename, width, height, qualityPercent);
        }

        public static long GetImageDataSize(byte[] imageData)
        {
            int size = 0;

            try
            {
                using (System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(imageData)))
                {
                    size = img.Width * img.Height;
                }

            }
            catch (Exception)
            {
            }

            return size;
        }

        public static BitmapImage PathToBitmapImage(string path, int imageWidth, int imageHeight)
        {
            if (System.IO.File.Exists(path))
            {
                BitmapImage bi = new BitmapImage();

                bi.BeginInit();

                if (imageWidth > 0 && imageHeight > 0)
                {
                    bi.DecodePixelWidth = imageWidth;
                    bi.DecodePixelHeight = imageHeight;
                }

                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri(path);
                bi.EndInit();
                bi.Freeze();

                return bi;
            }

            return null;
        }

        public static BitmapImage ByteToBitmapImage(byte[] byteData, int imageWidth, int imageHeight, int maxLength)
        {
            if (byteData != null && byteData.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(byteData))
                {

                    BitmapImage bi = new BitmapImage();

                    bi.BeginInit();

                    if (imageWidth > 0 && imageHeight > 0)
                    {
                        var size = new System.Drawing.Size(imageWidth, imageHeight);
                        if (maxLength > 0) size = GetScaledSize(new System.Drawing.Size(imageWidth, imageHeight), maxLength);

                        bi.DecodePixelWidth = size.Width;
                        bi.DecodePixelHeight = size.Height;
                    }

                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();
                    bi.Freeze();

                    return bi;
                }
            }

            return null;
        }

        public static System.Drawing.Size GetScaledSize(System.Drawing.Size originalSize, int maxLength)
        {
            var scaledSize = new System.Drawing.Size();

            if (originalSize.Height > originalSize.Width)
            {
                scaledSize.Height = maxLength;
                scaledSize.Width = Convert.ToInt32(((double)originalSize.Width / maxLength) * 100);
            }
            else
            {
                scaledSize.Width = maxLength;
                scaledSize.Height = Convert.ToInt32(((double)originalSize.Height / maxLength) * 100);
            }

            return scaledSize;
        }

        public static System.Windows.Media.Color GetDominantColor(string path)
        {
            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(path);

            return GetDominantColor(bitmap);
        }

        public static System.Windows.Media.Color GetDominantColor(byte[] imageData)
        {
            System.Drawing.Bitmap bitmap;

            using (var ms = new MemoryStream(imageData))
            {
                bitmap = new Bitmap(ms);
            }

            return GetDominantColor(bitmap);
        }

        public static System.Windows.Media.Color GetDominantColor(Bitmap bitmap)
        {
            var newBitmap = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bitmap, new Rectangle(0, 0, 1, 1));
            }
            Color color = newBitmap.GetPixel(0, 0);
            return System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
        }

        public static void PrepareGraphics(Graphics graphics, bool highQuality)
        {
            if (highQuality)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.AssumeLinear;
                graphics.PixelOffsetMode = PixelOffsetMode.Default;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            }
            else
            {
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.None;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            }
        }

        public static BitmapSource Bitmap2BitmapSource(Bitmap b)
        {
            IntPtr ip = b.GetHbitmap();
            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ip,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            NativeMethods.DeleteObject(ip);
            return bs;
        }

        public static BitmapSource Byte2BitmapSource(byte[] b)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(b);
                return Bitmap2BitmapSource((Bitmap)Image.FromStream(stream));
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Bitmap BitmapSource2Bitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(source.PixelWidth,source.PixelHeight,PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size),ImageLockMode.WriteOnly,PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty,data.Scan0,data.Height * data.Stride,data.Stride);bmp.UnlockBits(data);
            return bmp;
        }

    }
}
