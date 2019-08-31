using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.ILyric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Lunalipse.Core.Lyric
{
    public class LyricTokenizer : ILyricTokenizer
    {
        static volatile LyricTokenizer lt_instance;
        static readonly object lt_lock = new object();
        public static LyricTokenizer INSTANCE
        {
            get
            {
                if(lt_instance == null)
                {
                    lock (lt_lock)
                    {
                        lt_instance = lt_instance ?? new LyricTokenizer();
                    }
                }
                return lt_instance;
            }
        }

        private LyricTokenizer() {}

        Regex regex = new Regex(@"\[([0-9.:]*)\]+(.*)", RegexOptions.Compiled);
        public List<LyricToken> CreateTokens(string lyrics)
        {
            List<LyricToken> l = new List<LyricToken>();
            int offset = 0, p = 0 ;
            foreach (string s in lyrics.Split('\n'))
            {
                LyricToken t;
                if ((t = ParseStatement(s, ref offset, p)) != null)
                {
                    l.Add(t);
                    p++;
                }
            }
            return l;
        }

        public List<LyricToken> CreateTokensFromFile(string path)
        {
            List<LyricToken> l;
            if (!File.Exists(path)) return null;
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                l = CreateTokens(sr.ReadToEnd()); 
            }
            return l;
        }

        public LyricToken ParseStatement(string statement, ref int offset, int pos)
        {
            if (!string.IsNullOrEmpty(statement))
            {
                if (statement.StartsWith("[offset:"))
                {
                    if (!int.TryParse(SplitInfo(statement), out offset))
                    {
                        return null;
                    }
                }
                else if(regex.IsMatch(statement))
                {
                    MatchCollection mc = regex.Matches(statement);
                    string[] word = mc[0].Groups[2].Value.Split('|');
                    return new LyricToken(word[0], word.Length > 1 ? word[1] : "", TimeSpan.Parse("00:" + mc[0].Groups[1].Value), offset, pos);
                }
            }
            return null;
        }

        private string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }

        public List<LyricToken> CreateTokensFromWeb(string uri)
        {
            throw new NotImplementedException();
        }
    }
}
