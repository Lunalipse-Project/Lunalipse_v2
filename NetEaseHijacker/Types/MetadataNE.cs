using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEaseHijacker.Types
{
    public class MetadataNE
    {
        [JsonProperty(PropertyName = "songCount", Required = Required.Always)]
        public int total = 0;
        [JsonProperty(PropertyName = "songs", Required = Required.Always)]
        public List<MusicDetail> list;
    }
}
