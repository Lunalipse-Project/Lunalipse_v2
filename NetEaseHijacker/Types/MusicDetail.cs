using Lunalipse.Common.Interfaces.IWebMusic;
using MinJSON.Serialization;

namespace NetEaseHijacker.Types
{
    public class MusicDetail : IWebMusicDetail
    {

        [JsonProperty("id")]
        public int ID;
        [JsonProperty("name")]
        public string MusicName;
        [JsonProperty("ar")]
        public MusicArtist[] Artists;
        [JsonProperty("al")]
        public MusicAlbum Album;

        [JsonProperty("h")]
        public MusicSource SourceHighQ;
        [JsonProperty("m")]
        public MusicSource SourceMediumQ;
        [JsonProperty("l")]
        public MusicSource SourceLowQ;

        [JsonProperty("dt")]
        public long DurationMillisecond;
        [JsonProperty("publishTime")]
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
        [JsonProperty("br")]
        public int BitRate;
        [JsonProperty("size")]
        public long SizeInByte;
    }
    public class MusicArtist
    {
        [JsonProperty("name")]
        public string name;
    }

    public class MusicAlbum
    {
        [JsonProperty("id")]
        public int AlbumID;
        [JsonProperty("name")]
        public string AlbumName;
        [JsonProperty("picUrl")]
        public string AlbumPicUrl;
    }
}
