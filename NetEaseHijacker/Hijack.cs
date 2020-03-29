using MinJSON.JSON;
using MinJSON.Serialization;
using NetEaseHijacker.Types;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NetEaseHijacker
{
    public class Hijack : MarshalByRefObject
    {
        Raw raw;
        public Hijack()
        {
            raw = new Raw();
        }

        /// <summary>
        /// 绑定动作：完成时触发
        /// </summary>
        /// <param name="e">接受动作的方法</param>
        public void E_AllComplete(Action e)
        {
            raw.NetworkCore.OnAllQueueRequestCompletely += 
                new LunaNetCore.LNetC.AllQueueRequestCompletely(e);
        }

        /// <summary>
        /// 绑定动作：正在请求
        /// </summary>
        /// <param name="e">接受动作的方法</param>
        public void E_Requesting(Action<string> e)
        {
            raw.NetworkCore.OnHttpRequesting += 
                new LunaNetCore.LNetC.HttpRequesting(e);
        }

        /// <summary>
        /// 绑定动作：单个请求得到相应
        /// </summary>
        /// <param name="e">接受动作的方法</param>
        public void E_Responded(Action<string,string> e)
        {
            raw.NetworkCore.OnHttpResponded +=
                new LunaNetCore.LNetC.HttpResponded((x, result) => e?.Invoke(x, result == null ? string.Empty : result.ResultData));
        }

        /// <summary>
        /// 绑定动作：请求超时
        /// </summary>
        /// <param name="e">接受动作的方法</param>
        public void E_TimeOut(Action e)
        {
            raw.NetworkCore.OnHttpTimeOut += 
                new LunaNetCore.LNetC.HttpTimeOut(e);
        }

        /// <summary>
        /// 绑定动作：请求出错
        /// </summary>
        /// <param name="e">接受动作的方法</param>
        public void E_ErrorOccured(Action<Exception> e)
        {
            raw.NetworkCore.OnErrorOccurs += new LunaNetCore.LNetC.ErrorOccurs(e);
        }

        public void ClearAllEventSubscribers() => raw.NetworkCore.ClearAllEventSubscribers();

        public IWebProxy LNetWebProxy
        {
            get => raw.NetworkCore.LunaNetProxy;
            set => raw.NetworkCore.LunaNetProxy = value;
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
            await raw.Get(
                        SearchType.QUERY_SONGS_LIST, 
                        keyw, 
                        limit.ToString(), 
                        (offset == 0 ? 
                            true : false)
                                .ToString()
                                .ToLowerInvariant(),
                        offset+""
                 );
        }

        /// <summary>
        /// 获取歌曲详情
        /// </summary>
        /// <param name="id">歌曲ID</param>
        /// <returns></returns>
        public async Task SongDetail(string id)
        {
            await raw.Get(SearchType.QUERY_SONG_DETAIL, id);
        }

        /// <summary>
        /// 获取歌曲下载地址
        /// </summary>
        /// <param name="id">歌曲ID</param>
        /// <param name="bitRate">目标比特率</param>
        /// <returns></returns>
        public async Task DownloadURL(string id, string bitRate)
        {
            await raw.Get(SearchType.QUERY_DOWNLOAD_URL, id, bitRate);
        }

        /// <summary>
        /// 获取歌曲相应的歌词
        /// </summary>
        /// <param name="id">歌曲ID</param>
        /// <returns></returns>
        public async Task Lyric(string id)
        {
            await raw.Get(SearchType.QUERY_LYRIC, id);
        }

        /// <summary>
        /// 将网易云返回的原始数据解析为歌曲列表
        /// </summary>
        /// <param name="result">网易云返回的原始数据</param>
        /// <returns></returns>
        public MetadataNE ParseSongList(string result)
        {
            JsonObject jObject = JsonObject.Parse(result);
            return JsonConversion.DeserializeJsonObject<MetadataNE>(jObject["result"].As<JsonObject>());
        }

        /// <summary>
        /// 将网易云返回的原始数据解析为歌词
        /// </summary>
        /// <param name="result">网易云返回的原始数据</param>
        /// <returns></returns>
        public string ParseLyric(string result)
        {
            JsonObject jo = JsonObject.Parse(result);
            JsonObject lrc = jo["lrc"].As<JsonObject>();
            try
            {
                return lrc["lyric"] != null ? lrc["lyric"].ToString() : null;
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
        public Tuple<string,string> ParseDownloadURL(string result)
        {
            try
            {
                JsonObject jt = JsonObject.Parse(result)["data"]
                                    .As<JsonArray>()[0]
                                    .As<JsonObject>();
                return new Tuple<string, string>(jt["url"] != null ? jt["url"].As<string>() : "",jt["type"].As<string>());
            }
            catch
            {
                return null;
            }
        }
    }
}
