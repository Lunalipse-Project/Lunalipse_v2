using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEaseHijacker.Types
{
    public class MusicDetail
    {
        public string id, name, ar_name, al_pic,al_name;
        //Store as <bitRate>|<size>
        public long[] sizes = new long[3];
        public int[] bitrate = new int[3];
        public int totalCount = 0;

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

        [JsonProperty(PropertyName = "duration")]
        public long DurationMillisecond;
        [JsonProperty(PropertyName = "publishTime")]
        public long TimeStampMillisecond;
    }

    public class MusicSource
    {
        [JsonProperty(PropertyName = "br")]
        public int BitRate;
        [JsonProperty(PropertyName = "size")]
        public long SizeInByte;
    }
}
