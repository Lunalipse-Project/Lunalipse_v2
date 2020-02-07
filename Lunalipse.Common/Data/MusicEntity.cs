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
    public class MusicEntity :ICachable, ICloneable
    {
        /// <summary>
        /// This is the fucking FILE NAME!!!
        /// </summary>
        [NonSerialized]
        public string Name;
        [NonSerialized]
        public bool AlbumClassfied = false;
        [NonSerialized]
        public bool ArtistClassfied = false;
        [NonSerialized]
        public byte[] AlbumPicture = null;
        [NonSerialized]
        public string LyricPath = "";
        [NonSerialized]
        public string LyricContent = "";
        [NonSerialized]
        public string Extension;
        
        public string Path;
        public string MusicID;
        public bool HasImage = false;
        //Internet music support
        public bool IsInternetLocation = false;
        public string[] Artist;
        public TimeSpan EstDuration;
        public string Album;
        public string ID3Name = "";

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

        public void InitializePicture(byte[] picture)
        {
            AlbumPicture = picture;
        }

        public void DisposePicture()
        {
            AlbumPicture = null;
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
                                  LyricPath!= string.Empty, 
                                  EstimateDurSecond.ToString(@"hh\:mm\:ss"),
                                  GetType().Name,
                                  HasImage.ToString(),
                                  IsInternetLocation.ToString());
        }

        public object Clone()
        {
            MusicEntity me = new MusicEntity();
            me.Album = Album;
            me.AlbumClassfied = AlbumClassfied;
            me.Artist = Artist;
            me.ArtistClassfied = ArtistClassfied;
            me.EstDuration = EstDuration;
            me.Extension = Extension;
            me.HasImage = HasImage;
            me.ID3Name = ID3Name;
            me.IsInternetLocation = IsInternetLocation;
            me.LyricContent = LyricContent;
            me.LyricPath = LyricPath;
            me.MusicID = MusicID;
            me.Name = Name;
            me.Path = Path;
            if (AlbumPicture != null)
            {
                me.AlbumPicture = new byte[AlbumPicture.Length];
                Array.Copy(AlbumPicture, me.AlbumPicture, AlbumPicture.Length);
            }
            return me;
        }
    }
}
