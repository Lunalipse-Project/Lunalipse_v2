using System;
using System.IO;

namespace Lunalipse.Resource.Generic.Types
{
    public class LrssResource
    {
        public long Size { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public byte[] Data { get; set; }

        public LrssResource(long size, string name, string type)
        {
            Size = size;
            Name = name;
            Type = type;
        }

        public LrssResource(string path)
        {
            using (FileStream fr = new FileStream(path, FileMode.Open))
            {
                Size = fr.Length;
                Data = new byte[Size];
                Name = Path.GetFileNameWithoutExtension(path);
                Type = Path.GetExtension(path);
                fr.Read(Data, 0, Data.Length);
            }
        }

        public bool ToFile(string path)
        {
            string expp = String.Format(@"{0}\{1}{2}", path, Name, Type);
            return __save(expp);
        }

        public bool ToFileA(string path)
        {
            return __save(path);
        }

        private bool __save(string fp)
        {
            try
            {
                using (FileStream fs = new FileStream(fp, FileMode.OpenOrCreate))
                {
                    fs.Write(Data, 0, Data.Length);
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
