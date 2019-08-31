using Lunalipse.Common.Data.BehaviorScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV1
{
    internal class ScriptUtil
    {
        public static string SanitaizeParameter(string para)
        {
            if (para.EndsWith("\""))
            {
                para = para.Remove(para.Length - 1, 1);
            }
            if (para.StartsWith("\""))
            {
                para = para.Remove(0, 1);
            }
            if (para.StartsWith(":"))
            {
                para = para.Remove(0, 1);
            }
            return para;
        }

        public static string RemoveExtension(string name)
        {
            if(!Regex.IsMatch(name, @".*\.(mp3|flac|acc|wav|aiff)", RegexOptions.IgnoreCase))
            {
                return name;
            }
            string[] p = name.Split('.');
            int len = p[p.Length - 1].Length;
            return name.Remove(name.Length - len - 1, len + 1);
        }
    }
}
