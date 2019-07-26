using System;
using System.IO;
using System.IO.Compression;

namespace Lunalipse.Resource
{
    public class Compression
    {
        public static bool CreateCompressedLrss(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string clrss = dir + "/" + name + ".clrss";
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                using (FileStream fs1 = new FileStream(clrss, FileMode.Create))
                {
                    using (GZipStream gzs = new GZipStream(fs1, CompressionMode.Compress, false))
                    {
                        using (FileStream fs2 = new FileStream(path, FileMode.Open))
                        {
                            byte[] b = new byte[fs2.Length];
                            fs2.Read(b, 0, b.Length);
                            gzs.Write(b, 0, b.Length);
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool CompressTo(MemoryStream memoryStream, string path)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (GZipStream gzs = new GZipStream(fs, CompressionMode.Compress, false))
                    {
                        byte[] b = new byte[memoryStream.Length];
                        memoryStream.Read(b, 0, b.Length);
                        gzs.Write(b, 0, b.Length);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static MemoryStream DecompressTo(string path)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (GZipStream gzs = new GZipStream(fs, CompressionMode.Decompress, false))
                    {
                        byte[] b = new byte[1024];
                        int len = 0;
                        while((len = gzs.Read(b, 0, 1024)) > 0)
                        {
                            ms.Write(b, 0, len);
                        }
                    }
                }
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
            catch (Exception)
            {
                return ms;
            }
        }

        public static MemoryStream DecompressTo(byte[] raw)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                using (MemoryStream ms_src = new MemoryStream(raw))
                {
                    ms_src.Seek(0, SeekOrigin.Begin);
                    using (GZipStream gzs = new GZipStream(ms_src, CompressionMode.Decompress, false))
                    {
                        byte[] b = new byte[1024];
                        int len = 0;
                        while ((len = gzs.Read(b, 0, 1024)) > 0)
                        {
                            ms.Write(b, 0, len);
                        }
                    }
                }
                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
            catch (Exception)
            {
                return ms;
            }
        }
    }
}
