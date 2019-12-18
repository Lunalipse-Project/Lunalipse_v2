using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Data
{
    public static class SupportFormat
    {
        public const string MP3 = ".mp3";
        public const string AIFF = ".aiff";
        public const string FLAC = ".flac";
        public const string WAV = ".wav";
        public const string ACC = ".acc";
        public const string OGG = ".ogg";

        public static bool AllQualified(string extension)
        {
            return (extension.Equals(MP3) || extension.Equals(AIFF) || extension.Equals(FLAC) || extension.Equals(WAV) || extension.Equals(ACC) || 
                extension.Equals(OGG)) ;
        }
    }
}
