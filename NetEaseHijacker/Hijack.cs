using LunaNetCore.Bodies;
using NetEaseHijacker.Types;
using Newtonsoft.Json;
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

        /// <summary>
        /// 绑定动作：完成时触发
        /// </summary>
        /// <param name="e">接受动作的方法</param>
        public void E_AllComplete(Action e)
        {
            r.Event_AllComplete(() => e?.Invoke());
        }

        /// <summary>
        /// 绑定动作：正在请求
        /// </summary>
        /// <param name="e">接受动作的方法</param>
        public void E_Requesting(Action<string> e)
        {
            r.Event_Requesting((x) => e?.Invoke(x));
        }

        /// <summary>
        /// 绑定动作：单个请求得到相应
        /// </summary>
        /// <param name="e">接受动作的方法</param>
        public void E_Responded(Action<string,RResult> e)
        {
            r.Event_Responded((x, y) => e?.Invoke(x, y));
        }

        /// <summary>
        /// 绑定动作：请求超时
        /// </summary>
        /// <param name="e">接受动作的方法</param>
        public void E_TimeOut(Action e)
        {
            r.Event_TimeOut(() => e?.Invoke());
        }

        /// <summary>
        /// 从网易云上搜索音乐
        /// </summary>
        /// <param name="keyw">关键字</param>
        /// <param name="limit">单次搜索返回结果的个数</param>
        /// <param name="offset">返回的结果的起始索引</param>
        /// <returns></returns>
        public async Task SearchSong(string keyw, int limit = 30, int offset = 0)
        {
            await r.Get(SearchType.SONGS, keyw, limit.ToString(), (offset == 0 ? true : false).ToString().ToLowerInvariant(), offset+"");
        }

        /// <summary>
        /// 获取歌曲详情
        /// </summary>
        /// <param name="id">歌曲ID</param>
        /// <returns></returns>
        public async Task SongDetail(string id)
        {
            await r.Get(SearchType.DETAIL, id);
        }

        /// <summary>
        /// 获取歌曲下载地址
        /// </summary>
        /// <param name="id">歌曲ID</param>
        /// <param name="bitRate">目标比特率</param>
        /// <returns></returns>
        public async Task DownloadURL(string id, string bitRate)
        {
            await r.Get(SearchType.DOWNLOAD, id, bitRate);
        }

        /// <summary>
        /// 获取歌曲相应的歌词
        /// </summary>
        /// <param name="id">歌曲ID</param>
        /// <returns></returns>
        public async Task Lyric(string id)
        {
            await r.Get(SearchType.LYRIC, id);
        }

        /// <summary>
        /// 将网易云返回的原始数据解析为歌曲列表
        /// </summary>
        /// <param name="result">网易云返回的原始数据</param>
        /// <returns></returns>
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

        /// <summary>
        /// 将网易云返回的原始数据解析为歌词
        /// </summary>
        /// <param name="result">网易云返回的原始数据</param>
        /// <returns></returns>
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

        /// <summary>
        /// 将网易云返回的原始数据解析为歌曲下载地址
        /// </summary>
        /// <param name="result">网易云返回的原始数据</param>
        /// <returns></returns>
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
