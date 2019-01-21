using LunaNetCore.Bodies;
using NetEaseHijacker.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEaseHijacker
{
    public class Hijack
    {
        Raw r;
        public Hijack()
        {
            r = new Raw();
        }

        public void E_AllComplete(Action e)
        {
            r.Event_AllComplete(() => e?.Invoke());
        }

        public void E_Requesting(Action<string> e)
        {
            r.Event_Requesting((x) => e?.Invoke(x));
        }

        public void E_Responded(Action<string,RResult> e)
        {
            r.Event_Responded((x, y) => e?.Invoke(x, y));
        }

        public void E_TimeOut(Action e)
        {
            r.Event_TimeOut(() => e?.Invoke());
        }

        public async Task SearchSong(string keyw, int limit = 30, int offset = 0)
        {
            await r.Get(SearchType.SONGS, keyw, limit.ToString(), (offset == 0 ? true : false).ToString().ToLowerInvariant(), offset+"");
        }

        public async Task SongDetail(string id)
        {
            await r.Get(SearchType.DETAIL, id);
        }

        public async Task DownloadURL(string id, string bitRate)
        {
            await r.Get(SearchType.DOWNLOAD, id, bitRate);
        }

        public async Task Lyric(string id)
        {
            await r.Get(SearchType.LYRIC, id);
        }

        public MetadataNE ParseSongList(string result)
        {
            try
            {
                JObject jo = JObject.Parse(result);
                MetadataNE mne = new MetadataNE()
                {
                    total = jo["result"]["songCount"].ToObject<int>(),
                    list = new List<SongDetail>()
                };
                
                foreach (var v in jo["result"]["songs"])
                {
                    SongDetail sd = new SongDetail();
                    sd.id = v["id"].ToString();
                    sd.name = v["name"].ToString();
                    sd.al_pic = v["al"]["picUrl"].ToString();
                    sd.ar_name = v["ar"][0]["name"].ToString();
                    sd.al_name = v["al"]["name"].ToString();
                    sd.duration = v["dt"].ToObject<long>();
                    int i = 0;

                    foreach (char c in "lmh")
                    {
                        string c_ = c.ToString();
                        if (v[c_].HasValues)
                        {
                            sd.sizes[i] = Convert.ToInt64(v[c_]["size"].ToString());
                            sd.bitrate[i] = Convert.ToInt32(v[c_]["br"].ToString());
                            i++;
                        }
                    }
                    mne.list.Add(sd);
                }
                return mne;
            }
            catch
            {
                return null;
            }
        }

        public string ParseLyric(string result)
        {
            JObject jo = JObject.Parse(result);
            try
            {
                return jo["lrc"]["lyric"] != null ? jo["lrc"]["lyric"].ToString() : null;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public string ParseDownloadURL(string result)
        {
            try
            {
                JToken jt = JObject.Parse(result)["data"][0];
                return jt["url"] != null ? jt["url"].ToString() : "";
            }
            catch
            {
                return "";
            }
        }
    }
}
