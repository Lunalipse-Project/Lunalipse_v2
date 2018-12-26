using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Interfaces.ICache;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lunalipse.Common.Data
{
    public class MusicEntity :ICachable
    {
        [Cachable]
        public string Album, Name, ID3Name, Extension, Path, Year;
        [Cachable]
        public string[] Artist;
        [Cachable]
        public TimeSpan EstDuration;

        public string ArtistFrist
        {
            get
            {
                return Artist[0];
            }
        }

        // Use file name as music name
        public string MusicName
        {
            get
            {
                return Name;
            }
        }

        // Name stored in ID3v2 tag
        public string IDv23Name
        {
            get
            {
                return ID3Name;
            }
        }

        public string DefaultArtist
        {
            get;
            set;
        }

        public string DefaultAlbum
        {
            get;
            set;
        }

        public TimeSpan EstimateDurSecond
        {
            get => EstDuration;
        }
    }
}
