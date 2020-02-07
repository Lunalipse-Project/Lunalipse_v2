using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Lunalipse.Utilities
{
    public static class Extended
    {
        private static DateTime creationDate = new DateTime(2018, 9, 25, 0, 0, 0);

        public static string FormateEx(this string target, params object[] s)
        {
            return string.Format(target, s);
        }

        public static bool AvailableEx(this string target)
        {
            return !(String.IsNullOrEmpty(target) || String.IsNullOrWhiteSpace(target));
        }

        public static bool DExist(this string path, FType ft)
        {
            if (ft == FType.FILE)
                return File.Exists(path);
            else
                return Directory.Exists(path);
        }

        public static int ToLunalipseTimeStamp(this DateTime now)
        {
            return (int)(now - creationDate).TotalSeconds;
        }

        /// <summary>
        /// 对Dictinary字典类进行遍历操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="d"></param>
        /// <param name="Act"></param>
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> d, Action<TKey, TValue> Act)
        {
            foreach (var i in d)
            {
                Act(i.Key, i.Value);
            }
        }

        public static bool True4All<TKey, TValue>(this IDictionary<TKey, TValue> d, Func<TKey, TValue, bool> proc)
        {
            foreach (var i in d)
            {
                if (!proc(i.Key, i.Value)) return false;
            }
            return true;
        }

        public static bool AddNonRepeat<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key, TValue val)
        {
            if (!d.ContainsKey(key))
            {
                d.Add(key, val);
                return true;
            }
            return false;
        }

        public static string LimitLength(this string str, int max_length)
        {
            if (str.Length > max_length)
            {
                return str.Substring(0, max_length) + "...";
            }
            return str;
        }

        public static T Possible<T>(this T[] a, Func<T, bool> condition)
        {
            foreach (T t in a)
            {
                if (condition(t))
                    return t;
            }
            return default(T);
        }

        public static bool Contains<T>(this T[] a, Func<T, bool> condition)
        {
            foreach (T t in a)
            {
                if (condition(t))
                    return true;
            }
            return false;
        }

        public static T GetAncestor<T>(this DependencyObject reference) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T) && parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent != null)
                return (T)parent;
            else
                return null;
        }

        public static string DigitPadding(this int digit, int places)
        {
            string stringDigit = digit.ToString();
            string padding = "";
            if (stringDigit.Length >= places) return stringDigit;
            for (int j = 0; j < places - stringDigit.Length; j++)
            {
                padding += "0";
            }
            return padding + stringDigit;
        }

        public static string ComputeHash(this object obj)
        {
            MemoryStream ms = new MemoryStream();
            MD5 md5 = MD5.Create();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] hash = md5.ComputeHash(ms);
            ms.Close();
            md5.Clear();
            return Convert.ToBase64String(hash);
        }

        public static string ComputeHash(this byte[] bytes)
        {
            string hashcode;
            using (MD5 md5 = MD5.Create())
            {
                hashcode = BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", "");
            }
            return hashcode;
        }

        public static bool isLinearGradientBrush(this Brush brush)
        {
            return brush.GetType() == typeof(LinearGradientBrush);
        }

        public static byte[] ToBytes<T>(this T structure) where T : struct
        {
            int size = Marshal.SizeOf(structure);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static T ToStruct<T>(this byte[] bytes) where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            T structure = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return structure;
        }

        public enum FType
        {
            FILE,
            DICT
        }
    }
}
