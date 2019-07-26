using Lunalipse.Resource.Generic.Types;
using Lunalipse.Resource.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static Lunalipse.Resource.Generic.Structure;
using static Lunalipse.Resource.Generic.Delegates;
using System.Text;

namespace Lunalipse.Resource
{
    public class LrssReader : ILrssReader, IDisposable
    {
        LPS_HEADER HEADER;

        MemoryStream fs;

        int len_header, len_fheader, len_dblock, len_verified;
        public int MAGIC { get; private set; }
        public bool Encrypted {
            get
            {
                return HEADER.H_ENCRYPTED;
            }
        }
        public string SIGNATURE
        {
            get
            {
                return HEADER.H_SIG;
            }
        }

        public LrssReader()
        {
            len_header = Marshal.SizeOf(typeof(LPS_HEADER));
            len_fheader = Marshal.SizeOf(typeof(LPS_FHEADER));
            len_dblock = Marshal.SizeOf(typeof(LPS_FBLOCK));
            len_verified = Marshal.SizeOf(typeof(LPS_VERIFIED));
        }

        public LrssReader(string path, byte[] DecKey = null) : this()
        {
            LoadLrssCompressed(path);
        }

        public void LoadLrssCompressed(string path)
        {
            if (fs != null) fs.Close();
            fs = Compression.DecompressTo(path);
            ReadHeader();
        }

        public void LoadLrss(string path)
        {
            if (fs != null) fs.Close();
            fs = new MemoryStream();
            using(FileStream fs_ = new FileStream(path, FileMode.Open))
            {
                byte[] content = new byte[fs_.Length];
                fs_.Read(content, 0, content.Length);
                fs.Write(content, 0, content.Length);
            }
            fs.Seek(0, SeekOrigin.Begin);
            ReadHeader();
        }

        public void LoadLrssRaw(byte[] lrss)
        {
            if (fs != null) fs.Close();
            fs = new MemoryStream(lrss);
            ReadHeader();
        }

        public void LoadLrssRawCompressed(byte[] lrss)
        {
            if (fs != null) fs.Close();
            fs = Compression.DecompressTo(lrss);
            ReadHeader();
        }

        public bool RestoringMagic(byte[] DecKey = null)
        {
            if (Encrypted && DecKey == null)
            {
                fs.Close();
                return false;
            }
            else if (HEADER.H_ENCRYPTED)
            {
                MAGIC = HEADER.H_MAGIC.XorDecrypt(DecKey);
                return Verification(Encoding.ASCII.GetString(DecKey));
            }
            return false;
        }

        public List<LrssIndex> GetIndex()
        {
            List<LrssIndex> lis = new List<LrssIndex>();
            for (int i = 0; i < HEADER.H_FH_LOC.Length; i++)
            {
                long l = HEADER.H_FH_LOC[i];
                if (l == 0) continue;
                lis.Add(new LrssIndex(ReadFileHeader(l), l));
            }
            return lis;
        }

        public LrssResource ReadResource(LrssIndex li)
        {
            try
            {
                return new LrssResource(li.Size, li.Name, li.Type)
                {
                    Data = GetContent(li.Size, li.Address, li.Occupied)
                };
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                OnEndpointReached?.Invoke(li.Occupied);
            }
        }

        private bool Verification(string key)
        {
            byte[] b = new byte[len_verified];
            fs.Seek(len_header, SeekOrigin.Begin);
            fs.Read(b, 0, len_verified);
            LPS_VERIFIED lv = (LPS_VERIFIED)b.XorCrypt(MAGIC).ToStruct(typeof(LPS_VERIFIED));
            byte[] pwd = new byte[HEADER.H_PWD_ACT_LEN];
            Array.Copy(lv.VE_KEY, 0, pwd, 0, pwd.Length);
            return Encoding.ASCII.GetString(pwd).Equals(key);
        }

        private void ReadHeader()
        {
            byte[] b = new byte[len_header];
            fs.Read(b, 0, len_header);
            HEADER = (LPS_HEADER)b.ToStruct(typeof(LPS_HEADER));
            MAGIC = HEADER.H_MAGIC;
            fs.Seek(0, SeekOrigin.Begin);
        }

        private LPS_FHEADER ReadFileHeader(long pos)
        {
            fs.Seek(pos, SeekOrigin.Begin);
            byte[] b = new byte[len_fheader];
            fs.Read(b, 0, len_fheader);
            fs.Seek(0, SeekOrigin.Begin);
            return (LPS_FHEADER)b.XorCrypt(MAGIC).ToStruct(typeof(LPS_FHEADER));
        }

        private byte[] GetContent(long size, long addr, int dcount)
        {
            long dblock = addr + len_fheader;
            int left = (int)(size % 1024);
            byte[] _dat = new byte[size];
            fs.Seek(dblock, SeekOrigin.Begin);
            for (int i = 0; i < dcount; i++)
            {
                Array.Copy(GetBlock().FB_DAT, 0, _dat, i * 1024, (i == dcount - 1 && left != 0) ? left : 1024);
                OnChuckOperated?.Invoke(i, dcount);
            }
            return _dat;
        }

        private LPS_FBLOCK GetBlock()
        {
            byte[] b = new byte[len_dblock];
            fs.Read(b, 0, len_dblock);
            return (LPS_FBLOCK)b.XorCrypt(MAGIC).ToStruct(typeof(LPS_FBLOCK));
        }

        public void Dispose()
        {
            if (fs != null)
                fs.Close();
        }
    }
}
