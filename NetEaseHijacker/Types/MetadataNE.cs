using MinJSON.Serialization;
using System.Collections.Generic;

namespace NetEaseHijacker.Types
{
    public class MetadataNE
    {
        [JsonProperty("songCount")]
        public int total = 0;
        [JsonProperty("songs")]
        public List<MusicDetail> list;
    }
}
