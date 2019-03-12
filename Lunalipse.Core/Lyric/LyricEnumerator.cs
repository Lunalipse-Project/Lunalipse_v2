using Lunalipse.Common.Interfaces.ILyric;
using System;
using System.Collections.Generic;
using Lunalipse.Common.Data;
using System.IO;
using Lunalipse.Utilities;

namespace Lunalipse.Core.Lyric
{
    class LyricEnumerator : ILyricEnumerator
    {
        public ILyricTokenizer Tokenizer { get; set; }
        public string LyricDefaultDir{ get; set; }
        private List<LyricToken> tokens = null;
        public bool AcquireLyric(MusicEntity Music)
        {
            if (Tokenizer == null)
            {
                return false;
            }
            tokens = Tokenizer.CreateTokensFromFile(TryGetLyric(Music));
            if(tokens == null) return false;
            return true;
        }

        public LyricToken Enumerating(TimeSpan current)
        {
            if (tokens != null)
            {
                int middle = (int)Math.Floor(tokens.Count / 2.0);
                int last = tokens.Count - 1;
                int first = 0;
                while (!isInRangeBetween(tokens[middle].TimeStamp, middle + 1 == tokens.Count ? TimeSpan.MaxValue : tokens[middle + 1].TimeStamp, current))
                {
                    if (last - first == 0)
                    {
                        return null;
                    }
                    if (current > tokens[middle].TimeStamp)
                    {
                        first = middle + 1;
                    }
                    else
                    {
                        last = middle - 1;
                    }
                    middle = (int)Math.Floor((last - first) / 2.0) + first;
                    if (middle < 0) return null;
                    if (middle + 1 == tokens.Count) return tokens[tokens.Count - 1];
                }
                return tokens[middle];
            }
            return null;
        }

        private string GetLyricFile(string path, string name)
        {
            return "{0}/{1}/{2}.lrc".FormateEx(Path.GetDirectoryName(path), LyricDefaultDir, name);
        }

        private string TryGetLyric(MusicEntity me)
        {
            string path = "";
            if (File.Exists(path = GetLyricFile(me.Path, me.IDv3Name)))
            {
                return path;
            }
            else
            {
                return path = GetLyricFile(me.Path, me.Name);
            }
        }

        private bool isInRangeBetween(TimeSpan first, TimeSpan last, TimeSpan current)
        {
            return current >= first && current <= last;
        }
    }
}
