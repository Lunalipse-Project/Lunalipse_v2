using System;
using System.IO;

namespace Lunalipse.Resource.Generic.Types
{
    public class LrssIndex
    {
        Structure.LPS_FHEADER __HEADER;

        public long Address { get; internal set; }
        public string ResourcePath { get; protected set; }
        public string Name
        {
            get
            {
                return __HEADER.FH_NAME;
            }
        }
        public string Type
        {
            get
            {
                return __HEADER.FH_TYPE;
            }
        }
        public long Size
        {
            get
            {
                return __HEADER.FH_SIZE;
            }
        }
        public int Occupied
        {
            get
            {
                return __HEADER.FH_BCOUNT;
            }
        }
        public int Index
        {
            get
            {
                return __HEADER.FH_INDEX;
            }
            set
            {
                __HEADER.FH_INDEX = value;
            }
        }
        internal Structure.LPS_FHEADER Header
        {
            get
            {
                return __HEADER;
            }
        }

        internal LrssIndex(Structure.LPS_FHEADER HEADER, long Location)
        {
            __HEADER = HEADER;
            Address = Location;
        }

        internal LrssIndex(string path)
        {
            FileInfo fi = new FileInfo(ResourcePath = path);
            Address = -1;
            __HEADER = new Structure.LPS_FHEADER()
            {
                FH_NAME = Path.GetFileNameWithoutExtension(path),
                FH_TYPE = fi.Extension,
                FH_BCOUNT = (int)Math.Ceiling(fi.Length / 1024d),
                FH_SIZE = fi.Length
            };
        }
    }
}
