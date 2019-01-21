using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Interfaces.ICache;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lunalipse.Common.Data
{
    [Serializable]
    public class MusicEntity :ICachable
    {
        [NonSerialized]
        public string Album, ID3Name, Year;

        public string Name, Path, Extension;
        [NonSerialized]
        public string[] Artist;
        [NonSerialized]
        public TimeSpan EstDuration;
        [NonSerialized]
        public bool AlbumClassfied = false;
        [NonSerialized]
        public bool ArtistClassfied = false;
        [NonSerialized]
        public bool HasImage = false;

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
        public string AlbumProperty
        {
            get => Album;
        }

        public TimeSpan EstimateDurSecond
        {
            get => EstDuration;
        }

    }
}
