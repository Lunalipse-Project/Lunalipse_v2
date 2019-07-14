using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                            fs2.CopyTo(gzs);
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
                        memoryStream.CopyTo(gzs);
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
                        gzs.CopyTo(ms);
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
