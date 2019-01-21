using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Win32.Win32ThemeColor
{
    public struct DWMCOLORIZATIONPARAMS
    {
        public uint ColorizationColor,
            ColorizationAfterglow,
            ColorizationColorBalance,
            ColorizationAfterglowBalance,
            ColorizationBlurBalance,
            ColorizationGlassReflectionIntensity,
            ColorizationOpaqueBlend;
    }
}
