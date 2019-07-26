using System.Runtime.InteropServices;

namespace Lunalipse.Resource.Generic
{
    public static class Structure
    {
        public struct LPS_HEADER
        {
            public int H_MAGIC;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string H_SIG;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public long[] H_FH_LOC;
            public bool H_ENCRYPTED;
            public int H_PWD_ACT_LEN;
        };

        internal struct LPS_FHEADER
        {
            public int FH_INDEX;
            public int FH_BCOUNT;
            public long FH_SIZE;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string FH_NAME;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string FH_TYPE;
        };

        internal struct LPS_FBLOCK
        {
            public int FB_INDEX;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] FB_DAT;
        };

        internal struct LPS_VERIFIED
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
            public byte[] VE_KEY;
        }

        public static int len_header;
        public static int len_fheader;
        public static int len_dblock;
    }
}
