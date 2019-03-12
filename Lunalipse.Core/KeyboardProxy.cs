using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces;
using Lunalipse.Common.Interfaces.IConsole;
using Lunalipse.Utilities.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lunalipse.Core
{
    public class KeyboardProxy : ComponentHandler, IKeyboardProxy , IDisposable
    {
        static volatile KeyboardProxy K_PROXY_INSTANCE;
        static readonly object K_PROXY_LOCK = new object();
        /// <summary>
        /// 全局键盘监听器，当任意按键按下时总会触发。
        /// 建议使用时将<see cref="IgnoreAllCustomAction"/>设为<c>true</c>，这将屏蔽所有的响应按键的事件。
        /// </summary>
        public Action<int, Keys> KeyboardGlobalListener;

        public bool IgnoreAllCustomAction { get; set; }
        public static KeyboardProxy INSTANCE
        {
            get
            {
                if(K_PROXY_INSTANCE==null)
                {
                    lock(K_PROXY_LOCK)
                    {
                        K_PROXY_INSTANCE = K_PROXY_INSTANCE ?? new KeyboardProxy();
                    }
                }
                return K_PROXY_INSTANCE;
            }
        }

        KeyboardHook keyboard;
        List<KeyEventProc> EventList;

        protected KeyboardProxy()
        {
            keyboard = new KeyboardHook();
            EventList = new List<KeyEventProc>();
            keyboard.Start();
            keyboard.KeyDownEvent += KeyDown;
            keyboard.KeyUpEvent   += KeyUp;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if(!IgnoreAllCustomAction)
            {
                foreach (KeyEventProc kep in EventList)
                {
                    if (e.KeyValue == kep.SubKey && (int)Control.ModifierKeys == kep.ModifierKey)
                    {
                        kep.ProcInvoke_Down?.Invoke();
                        break;
                    }
                }
            }
            KeyboardGlobalListener?.Invoke(e.KeyValue, Control.ModifierKeys);
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if (IgnoreAllCustomAction) return;
            foreach (KeyEventProc kep in EventList)
            {
                if (e.KeyValue == kep.SubKey && (int)Control.ModifierKeys == kep.ModifierKey && kep.WaitRelease)
                {
                    kep.ProcInvoke_Up?.Invoke();
                    break;
                }
            }
        }

        public bool RegistKeyEvent(KeyEventProc proc)
        {
            if (EventList.Exists(x => x.Name == proc.Name)) return false;
            EventList.Add(proc);
            return true;
        }

        public bool RemoveKeyEvent(KeyEventProc proc)
        {
            return EventList.Remove(proc);
        }

        public bool RemoveKeyEvent(string name)
        {
            return EventList.Remove(EventList.Find(x => x.Name == name));
        }

        public KeyEventProc GetKeyEvent(string name)
        {
            return EventList.Find(x => x.Name == name);
        }

        public KeyEventProc GetKeyEvent(int index)
        {
            return index < EventList.Count ? EventList[index] : null;
        }

        public bool ChangeShortCut(int index, int Key, int Modifier)
        {
            if (EventList.Exists(x => x.SubKey == Key && x.ModifierKey == Modifier))
                return false;
            EventList[index].ModifierKey = Modifier;
            EventList[index].SubKey = Key;
            return true;
        }

        public bool ChangeShortCut(string name, int Key, int Modifier)
        {
            if (EventList.Exists(x => x.SubKey == Key && x.ModifierKey == Modifier))
                return false;
            KeyEventProc kep = EventList.Find(x => x.Name == name);
            if (kep == null) return false;
            kep.ModifierKey = Modifier;
            kep.SubKey = Key;
            return true;
        }

        public List<KeyEventProc> GetEventsList()
        {
            return EventList;
        }

        public override bool OnCommand(params string[] args)
        {
            return base.OnCommand(args);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)。
                }
                keyboard.Stop();
                K_PROXY_INSTANCE = null;
                disposedValue = true;
            }
        }
        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
