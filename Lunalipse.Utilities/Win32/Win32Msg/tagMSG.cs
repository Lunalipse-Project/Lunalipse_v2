using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Win32.Win32Msg
{
    public struct tagMSG
    {
        public IntPtr hwnd;
        public uint message;
        public int wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }
}
