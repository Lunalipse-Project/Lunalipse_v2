using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Win32.Win32Msg
{
    public class WndMsgHook
    {
        HookProc GetMsgProc;
        static int MessageHook;
        public void Start()
        {
            GetMsgProc = new HookProc(GetMsgProcHook);
            MessageHook = NativeMethods.SetWindowsHookEx(HOOK.WH_GETMESSAGE, GetMsgProc, NativeMethods.LoadWin32Library("user32.dll"), 0);
            NativeMethods.SetWindowsHookEx(HOOK.WH_GETMESSAGE, GetMsgProc, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            if (MessageHook == 0)
            {
                
                throw new Exception("Error when install, with error code:" + Marshal.GetLastWin32Error());
            }
        }

        private int GetMsgProcHook(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode > 0)
            {
                tagMSG message = (tagMSG)Marshal.PtrToStructure(lParam, typeof(tagMSG));
                Console.WriteLine(message.message.ToString());
            }
            return NativeMethods.CallNextHookEx(MessageHook, nCode, wParam, lParam);
        }
    }
}
