using Lunalipse.Common.Generic.Cache;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Core.Cache
{
    public static class CacheUtils
    {
        public const string CACHE_MAGIC_PREFIX = "maggie";
        public static string GenerateName(this CacheFileInfo cfi)
        {
            return "{0}{1}{2}".FormateEx(CACHE_MAGIC_PREFIX, ((int)cfi.cacheType).ToString(),cfi.id);
        }

        public static CacheFileInfo ConvertToWWU(string name)
        {
            CacheFileInfo cacheFileInfo = new CacheFileInfo();
            try
            {
                if (name.StartsWith(CACHE_MAGIC_PREFIX))
                {
                    int type = int.Parse(name[CACHE_MAGIC_PREFIX.Length] + "");
                    cacheFileInfo.cacheType = (CacheType)type;
                    cacheFileInfo.id = name.Substring(CACHE_MAGIC_PREFIX.Length + 1);
                }
            }
            catch
            {
                // The cache name is invalid! (but has valid CACHE_MAGIC_PREFIX)
            }
            return cacheFileInfo;
        }

        public static IEnumerable<string> FindAllCaches(string baseDir, CacheType cacheType)
        {
            string prefix = CACHE_MAGIC_PREFIX + ((int)cacheType).ToString();
            foreach(string path in Directory.GetFiles(baseDir))
            {
                if (Path.GetFileName(path).StartsWith(prefix))
                {
                    yield return path;
                }
            }
        }

        public static bool IsNonValueType(this Type type)
        {
            return type.IsClass && !type.IsValueType && !type.Equals(typeof(String));
        }

        //public static string CleanFormat(string ins)
        //{
        //    String pattern = @"[\r|\n|\t]";
        //    String replaceValue = String.Empty;
        //    return Regex.Replace(ins, pattern, replaceValue);
        //}
    }
}
