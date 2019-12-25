using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEaseHijacker.Types
{
    public class MusicBrief
    {
        [JsonProperty(PropertyName = "id")]
        public int ID;
        [JsonProperty(PropertyName = "name")]
        public string MusicName;
        [JsonProperty(PropertyName = "artist")]
        public MusicArtist[] Artists;
        [JsonProperty(PropertyName = "album")]
        public MusicAlbum Album;
        [JsonProperty(PropertyName = "duration")]
        public long DurationMillisecond;
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
        [JsonProperty(PropertyName = "picUrl",Required = Required.Default)]
        public string AlbumPicUrl;
    }
}
