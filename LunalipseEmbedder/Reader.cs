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
    public class Reader
    {
        LrssReader lr;
        public int MagicNumber { get; private set; } = 0;
        public string Signature { get; private set; } = "";
        public string OutputDir { get; set; } = "";
        public byte[] Passwd { get; set; } = null;

        public Reader(string path)
        {
            lr = new LrssReader();
            lr.LoadLrss(path);
            Signature = lr.SIGNATURE;
            if (!IsPasswordRequired())
            {
                MagicNumber = lr.MAGIC;
            }
        }

        public bool IsPasswordRequired()
        {
            return lr.Encrypted;
        }

        public bool Verify()
        {
            if (Passwd == null) return false;
            if (lr.RestoringMagic(Passwd))
            {
                MagicNumber = lr.MAGIC;
                return true;
            }
            return false;
        }

        public List<LrssIndex> Indexes()
        {
            return lr.GetIndex();
        }

        public async Task<bool> OutputResource(LrssIndex lri)
        {
            LrssResource lrs = await lr.ReadResource(lri);
            if (lrs.Data == null || lrs == null) return false;
            string outp = OutputDir;
            if (outp != "")
            {
                FileAttributes attributes = File.GetAttributes(OutputDir);
                if (attributes.HasFlag(FileAttributes.Directory))
                {
                    outp += @"\{0}.{1}".FormateEx(lri.Name, lri.Type);
                }
            }
            else
                outp += "{0}{1}".FormateEx(lri.Name, lri.Type);
            try
            {
                using (FileStream fs = new FileStream(outp, FileMode.Create))
                {
                    fs.Write(lrs.Data, 0, lrs.Data.Length);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
