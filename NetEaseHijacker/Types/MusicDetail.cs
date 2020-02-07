using Lunalipse.Common.Interfaces.IWebMusic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEaseHijacker.Types
{
    public class MusicDetail : IWebMusicDetail
    {

        [JsonProperty(PropertyName = "id")]
        public int ID;
        [JsonProperty(PropertyName = "name")]
        public string MusicName;
        [JsonProperty(PropertyName = "ar")]
        public MusicArtist[] Artists;
        [JsonProperty(PropertyName = "al")]
        public MusicAlbum Album;

        [JsonProperty(PropertyName = "h")]
        public MusicSource SourceHighQ;
        [JsonProperty(PropertyName = "m")]
        public MusicSource SourceMediumQ;
        [JsonProperty(PropertyName = "l")]
        public MusicSource SourceLowQ;

        [JsonProperty(PropertyName = "dt")]
        public long DurationMillisecond;
        [JsonProperty(PropertyName = "publishTime")]
        public long TimeStampMillisecond;

        public string getAlbumName()
        {
            return Album.AlbumName;
        }

        public string getAlbumPicture()
        {
            return Album.AlbumPicUrl;
        }

        public string getArtistName()
        {
            MusicArtist artist = Artists[0];
            return artist != null ? artist.name : "Unknow";
        }

        public string getID()
        {
            return ID.ToString();
        }

        public string getMusicName()
        {
            return MusicName;
        }

        public double getTotalSeconds()
        {
            return DurationMillisecond;
        }
    }

    public class MusicSource
    {
        [JsonProperty(PropertyName = "br")]
        public int BitRate;
        [JsonProperty(PropertyName = "size")]
        public long SizeInByte;
    }
    public class MusicArtist
    {
        [JsonProperty(PropertyName = "name")]
        public string name;
    }

    public class MusicAlbum
    {
        [JsonProperty(PropertyName = "id")]
        public int AlbumID;
        [JsonProperty(PropertyName = "name")]
        public string AlbumName;
        [JsonProperty(PropertyName = "picUrl", Required = Required.Default)]
        public string AlbumPicUrl;
    }
}
