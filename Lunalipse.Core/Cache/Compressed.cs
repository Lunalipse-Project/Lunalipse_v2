using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.Cache
{
    public class Compression
    {
        public static bool Compress(byte[] b, string path,bool enableCompress = true)
        {
            if (File.Exists(path)) return true;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    if (enableCompress)
                    {
                        using (GZipStream gzs = new GZipStream(fs, CompressionMode.Compress, false))
                        {
                            gzs.Write(b, 0, b.Length);
                        }
                    }
                    else
                    {
                        fs.Write(b, 0, b.Length);
                    }
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public static byte[] Decompress(string path, bool isCompressed = true)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    if (isCompressed)
                    {
                        using (GZipStream gzs = new GZipStream(fs, CompressionMode.Decompress, false))
                        {
                            gzs.CopyTo(ms);
                        }
                    }
                    else
                    {
                        fs.CopyTo(ms);
                    }
                }
                return ms.ToArray();
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }
    }
}
