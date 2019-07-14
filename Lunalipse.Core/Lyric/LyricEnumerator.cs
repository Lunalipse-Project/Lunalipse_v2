using Lunalipse.Common.Interfaces.ILyric;
using System;
using System.Collections.Generic;
using Lunalipse.Common.Data;
using System.IO;
using Lunalipse.Utilities;
using System.Diagnostics;

namespace Lunalipse.Core.Lyric
{
    public class LyricEnumerator : ILyricEnumerator
    {

        public static event Action<List<LyricToken>> OnLyricPrepared;
        public static event Func<List<LyricToken>> OnTryGetLyric;

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
            OnLyricPrepared?.Invoke(tokens);
            if (tokens == null) return false;
            OnTryGetLyric += () =>
            {
                return tokens;
            };
            return true;
        } 

        public static List<LyricToken> TryGetLyric()
        {
            return OnTryGetLyric?.Invoke();
        }

        public LyricToken Enumerating(TimeSpan current, LyricToken CurrentToken)
        {
            if (tokens != null)
            {
                if (CurrentToken != null)
                {
                    int index = tokens.IndexOf(CurrentToken);
                    int next = Math.Min(tokens.Count, index + 1);
                    int prev = Math.Max(0, index - 1);
                    if (isInRangeBetween(tokens[prev].TimeStamp, tokens[next].TimeStamp, current)) return CurrentToken;
                }
                int middle = (int)Math.Ceiling(tokens.Count / 2.0);
                int last = tokens.Count - 1;
                int first = 0;
                while (!isInRangeBetween(tokens[middle].TimeStamp, middle + 1 == tokens.Count ? TimeSpan.MaxValue : tokens[middle + 1].TimeStamp, current))
                {
                    if (last == first) 
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
                    middle = (int)Math.Ceiling((last + first) / 2.0);
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
