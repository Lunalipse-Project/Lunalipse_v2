using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Resource
{
    public static class Utils
    {
        private static string[] Prefixes = { "B", "K", "M", "G", "T" };

        public static object ToStruct(this byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        /// <summary>
        /// 数据结构体转化为字节数组
        /// </summary>
        /// <param name="size">结构体长度</param>
        /// <param name="structObj">数据结构类型</param>
        /// <returns></returns>
        public static byte[] ToBytes<T>(this T structObj, int size) where T : struct
        {
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }

        public static byte[] XorCrypt(this byte[] t, int key)
        {
            for(int i = 0; i < t.Length; i++)
            {
                t[i] = (byte)(t[i] ^ key);
            }
            return t;
        }

        public static int XorEncrypt(this int t, byte[] key)
        {
            for (int i = 0; i < key.Length; i++)
            {
                t = t ^ (int)key[i];
            }
            return t;
        }

        public static int XorDecrypt(this int t, byte[] key)
        {
            for (int i = key.Length - 1; i >= 0; i--)
            {
                t = t ^ key[i];
            }
            return t;
        }
        public static string ToStorage(this long size)
        {
            int inx = 0;
            while (size > 1024)
            {
                size = size / 1024;
                inx++;
            }
            if (inx > Prefixes.Length - 1) throw new OverflowException("values is too big to convert");
            return "{0}{1}".LrssFormateEx(size, Prefixes[inx]);
        }
        public static string LrssFormateEx(this string target, params object[] s)
        {
            return string.Format(target, s);
        }

    }
}
