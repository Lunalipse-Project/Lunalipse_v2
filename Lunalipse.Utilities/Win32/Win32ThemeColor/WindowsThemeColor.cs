using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;

namespace Lunalipse.Utilities.Win32.Win32ThemeColor
{
    public class WindowsThemeColor
    {
        const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x320;

        public static event Action OnThemeColorChanged;
        private HwndSource hsource;

        public WindowsThemeColor(IntPtr HWND)
        {
            hsource = HwndSource.FromHwnd(HWND);
            hsource.AddHook(WndProc);
        }

        public static Color GetWindowColorizationColor(bool opaque)
        {
            DWMCOLORIZATIONPARAMS Params = new DWMCOLORIZATIONPARAMS();
            NativeMethods.DwmGetColorizationParameters(ref Params);
            return Color.FromArgb(
                (byte)(opaque ? 255 : Params.ColorizationColor >> 24), 
                (byte)(Params.ColorizationColor >> 16), 
                (byte)(Params.ColorizationColor >> 8), 
                (byte)Params.ColorizationColor);
        }

        public static Color GetWindowsImmersiveColor()
        {
            uint colour1 = NativeMethods.GetImmersiveColorFromColorSetEx(
                (uint)NativeMethods.GetImmersiveUserColorSetPreference(false, false), 
                NativeMethods.GetImmersiveColorTypeFromName(
                    Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")
                    ), 
                false, 
                0);
            Color colour = Color.FromArgb(
                (byte)((0xFF000000 & colour1) >> 24), 
                (byte)(0x000000FF & colour1), 
                (byte)((0x0000FF00 & colour1) >> 8), 
                (byte)((0x00FF0000 & colour1) >> 16));
            return colour;
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_DWMCOLORIZATIONCOLORCHANGED:
                    OnThemeColorChanged?.Invoke();
                    return IntPtr.Zero;

                default:
                    return IntPtr.Zero;
            }
        }
    }
}
