using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Core.Cache
{
    public static class CacheUtils
    {
        public static string GenerateName(this WinterWrapUp cw)
        {
            return "cch_{3}_{0}_{1}{2}".FormateEx(cw.deletable ? "t" : "f", cw.uid, CACHE_FILE_EXT, cw.markName);
        }

        public static WinterWrapUp ConvertToWWU(string name)
        {
            string[] sequence = name.Split('_');
            return new WinterWrapUp()
            {
                markName = sequence[1],
                deletable = sequence[2] == "t" ? true : false,
                uid = sequence[3]
            };
        }

        public static string GenerateMarkName(string type,params string[] additions)
        {
            string name = type;
            foreach(string s in additions)
            {
                name += "@{0}".FormateEx(s);
            }
            return name;
        }

        public static string[] GetMarkNames(this WinterWrapUp wwu)
        {
            return wwu.markName.Split('@');
        }

        public static string getMajorMark(string[] marknames)
        {
            return marknames[0];
        }

        public static bool IsNonValueType(this Type type)
        {
            return type.IsClass && !type.IsValueType && !type.Equals(typeof(String));
        }
        public static bool IsCachable(this Type type,string filter)
        {
            return type.IsClass && type.GetInterface(filter) != null;
        }

        //public static string CleanFormat(string ins)
        //{
        //    String pattern = @"[\r|\n|\t]";
        //    String replaceValue = String.Empty;
        //    return Regex.Replace(ins, pattern, replaceValue);
        //}
    }
}
