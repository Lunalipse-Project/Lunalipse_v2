using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Win32
{
    class SWP_FLAGS
    {
        public const uint SWP_ASYNCWINDOWPOS = 0x4000;
        public const uint SWP_DEFERERASE = 0x2000;
        public const uint SWP_DRAWFRAME = 0x20;
        public const uint SWP_FRAMECHANGED = 0x20;
        public const uint SWP_HIDEWINDOW = 0x80;
        public const uint SWP_NOACTIVATE = 0x10;
        public const uint SWP_NOCOPYBITS = 0x100;
        public const uint SWP_NOMOVE = 0x2;
        public const uint SWP_NOOWNERZORDER = 0x200;
        public const uint SWP_NOREDRAW = 0x8;
        public const uint SWP_NOREPOSITION = 0x200;
        public const uint SWP_NOSENDCHANGING = 0x400;
        public const uint SWP_NOSIZE = 0x1;
        public const uint SWP_NOZORDER = 0x4;
        public const uint SWP_SHOWWINDOW = 0x40;
    }
}
