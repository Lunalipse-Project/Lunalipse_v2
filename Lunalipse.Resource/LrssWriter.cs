using Lunalipse.Resource.Generic.Types;
using Lunalipse.Resource.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Lunalipse.Resource.Generic.Structure;
using static Lunalipse.Resource.Generic.Delegates;

namespace Lunalipse.Resource
{
    public class LrssWriter : ILrssWriter
    {
        LPS_HEADER header;
        LPS_VERIFIED lve;
        int len_header, len_fheader, len_dblock, len_verified;
        public List<LrssIndex> Resources { get; set; }
        FileStream fs;
        int magic;

        public LrssWriter()
        {
            Resources = new List<LrssIndex>();
            len_header = Marshal.SizeOf(typeof(LPS_HEADER));
            len_fheader = Marshal.SizeOf(typeof(LPS_FHEADER));
            len_dblock = Marshal.SizeOf(typeof(LPS_FBLOCK));
            len_verified = Marshal.SizeOf(typeof(LPS_VERIFIED));
        }
        public void Initialize(int Magic, string signature, string dest, byte[] EncKey = null)
        {
            magic = Magic;
            header = new LPS_HEADER();
            lve = new LPS_VERIFIED();
            lve.VE_KEY = new byte[26];
            if (EncKey != null)
            {
                header.H_PWD_ACT_LEN = EncKey.Length;
                Array.Copy(EncKey, 0, lve.VE_KEY, 0, EncKey.Length);
            }
            header.H_MAGIC = EncKey == null ? Magic : Magic.XorEncrypt(EncKey);
            header.H_FH_LOC = new long[32];
            header.H_SIG = signature;
            header.H_ENCRYPTED = EncKey != null;
            if (File.Exists(dest)) File.Delete(dest);
            fs = new FileStream(dest, FileMode.OpenOrCreate);
        }

        public async Task<bool> Export()
        {
            await Task.Run(() =>
            {
                fs.Write(new byte[len_header], 0, len_header);
                if (header.H_ENCRYPTED)
                    fs.Write(lve.ToBytes(len_verified).XorCrypt(magic), 0, len_verified);
                for (int i = 0; i < Resources.Count;)
                {
                    OnSingleEndpointReached?.Invoke(Resources[i].ResourcePath);
                    _writeHeader(Resources[i], ref i);
                    Resources[i - 1] = null;
                }
                Resources.Clear();
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(header.ToBytes(len_header), 0, len_header);
                OnEndpointReached?.Invoke(Resources.Count);
            });
            fs.Flush();
            fs.Close();
            return true;
        }

        public void RemoveResource(int index)
        {
            Resources.RemoveAt(index);
        }

        public bool AppendResource(string path)
        {
            if (Resources.Count >= 32) return false;
            Resources.Add(new LrssIndex(path));
            return true;
        }

        public bool AppendResourcesDir(string baseDir)
        {
            DirectoryInfo diri = new DirectoryInfo(baseDir);
            foreach (FileInfo path in diri.GetFiles())
            {
                if (path.Attributes == FileAttributes.Hidden) continue;
                AppendResource(path.FullName);
            }
            return true;
        }

        public bool AppendResources(params string[] pathes)
        {
            foreach (string path in pathes)
            {
                AppendResource(path);
            }
            return true;
        }

        private void _writeHeader(LrssIndex lr, ref int index)
        {
            header.H_FH_LOC[index] = fs.Length;
            lr.Index = index;
            lr.Address = fs.Position;
            fs.Write(lr.Header.ToBytes(len_fheader).XorCrypt(magic), 0, len_fheader);
            using(FileStream resource = new FileStream(lr.ResourcePath, FileMode.Open))
            {
                byte[] data = new byte[lr.Size];
                resource.Read(data, 0, data.Length);
                _writeBlocks(data, lr.Occupied);
            }
            index++;
        }

        private void _writeBlocks(byte[] b, int bcount)
        {
            int paddingBytes = b.Length % 1024;
            int index = 0;
            for (int i = 0; i < bcount; i++)
            {
                LPS_FBLOCK lfb = new LPS_FBLOCK();
                lfb.FB_INDEX = index;
                bool bl = i == bcount - 1 && paddingBytes != 0;
                lfb.FB_DAT = new byte[1024];
                Array.Copy(b, i * 1024, lfb.FB_DAT, 0, bl ? paddingBytes : 1024);
                fs.Write(lfb.ToBytes(len_dblock).XorCrypt(magic), 0, len_dblock);
                index++;
                OnChuckOperated?.Invoke(index, bcount);
            }
        }
    }
}
