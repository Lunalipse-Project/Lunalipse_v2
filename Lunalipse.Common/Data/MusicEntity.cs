using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Interfaces.ICache;
using Lunalipse.Utilities;
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
        public string Album, ID3Name="", Year;

        /// <summary>
        /// This is the fucking FILE NAME!!!
        /// </summary>
        public string Name;
        public string Path, Extension;
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

        //Internet music support
        public bool IsInternetLocation = false;

        public bool HasLyricLocal = false;
        public bool HasLyricOnline = false;
        public string LyricPath = "";
        public string MusicURI;
        public string LyricURI;

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
                if (ID3Name == "") return Name;
                return ID3Name;
            }
        }

        // Name stored in ID3v2 tag
        public string IDv3Name
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

        public override string ToString()
        {
            return ("{6}=>(\n" +
                    "\tName = \"{0}\"\n" +
                    "\tFullName = \"{1}\"\n" +
                    "\tArtist = \"{2}\"\n" +
                    "\tAlbum = \"{3}\"\n" +
                    "\tHasAlbumImage = {7}\n" +
                    "\tIsInternetLocation = {8}\n" +
                    "\tHasLyric = {4}\n" +
                    "\tDuration = {5}\n" +
                    ")").FormateEx(MusicName, 
                                  Path, 
                                  Artist.Length == 0 ? "Unknown" : ArtistFrist, 
                                  Album, 
                                  HasLyricLocal.ToString(), 
                                  EstimateDurSecond.ToString(@"hh\:mm\:ss"),
                                  GetType().Name,
                                  HasImage.ToString(),
                                  IsInternetLocation.ToString());
        }
    }
}
