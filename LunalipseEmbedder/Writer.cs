using Lunalipse.Resource;
using Lunalipse.Resource.Generic.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseEmbedder
{
    public class Writer
    {
        string[] str;
        LrssWriter lwriter;
        public int MagicNumber { get; set; } = 0;
        public string Signature { get; set; } = "";
        public string outPutPath { get; set; } = "";
        public byte[] Passwd { get; set; } = null;

        public Writer()
        {

        }
        public Writer(string[] resources)
        {
            str = resources;
            lwriter = new LrssWriter();
            
        }
        

        public void Prepare()
        {
            foreach(string p in str)
            {
                if (File.GetAttributes(p).HasFlag(FileAttributes.Directory))
                {
                    lwriter.AppendResourcesDir(p);
                }
                else
                {
                    lwriter.AppendResource(p);
                }
            }
            lwriter.Initialize(MagicNumber, Signature, Passwd);
        }

        public List<LrssIndex> GetResources()
        {
            return lwriter.Resources;
        }

        public async Task DoSeal(bool useCompression)
        {
            lwriter.Export();
            if (useCompression) Compression.CompressTo(lwriter.OuputStream, outPutPath);
            else
            {
                lwriter.OuputStream.Seek(0, SeekOrigin.Begin);
                using(FileStream fs = new FileStream(outPutPath, FileMode.Create))
                {
                    lwriter.OuputStream.CopyTo(fs);
                }
            }
            lwriter.Dispose();
        }

    }
}
