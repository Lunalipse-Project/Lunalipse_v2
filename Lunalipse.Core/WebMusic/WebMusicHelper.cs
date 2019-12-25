using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunalipse.Common.Data;
using NetEaseHijacker;
using NetEaseHijacker.Types;

namespace Lunalipse.Core.WebMusic
{
    public class WebMusicHelper
    {
        
        private Hijack NeteaseHijacker;
        private MetadataNE currentQueryListResult;
        //private MusicDetail currentSongDetail;

        public int TotalPage { get; private set; }
        public int EntriesPrePage { get; set; }
        public int CurrentPage { get; private set; }
        public string currentSongsLyric { get; private set; }
        public string currentDownloadURI { get; private set; }

        public WebMusicHelper()
        {
            NeteaseHijacker = new Hijack();
            
        }

        /// <summary>
        /// 绑定查询结果可用时事件
        /// </summary>
        /// <param name="handler">接受事件的函数，参数：string 查询类型，为<see cref="SearchType"/>枚举类型下枚举项的等效字符串</param>
        public void BindOnResultArrived(Action<string> handler)
        {
            NeteaseHijacker.E_Responded((searchType, result) =>
            {
                switch(searchType)
                {
                    //case "DETAIL":
                    //    currentSongDetail = NeteaseHijacker.
                    //    break;
                    case "DOWNLOAD":
                        currentDownloadURI = NeteaseHijacker.ParseDownloadURL(result);
                        break;
                    case "SONGS":
                        currentQueryListResult = NeteaseHijacker.ParseSongList(result);
                        break;
                    case "LYRIC":
                        currentSongsLyric = NeteaseHijacker.ParseLyric(result);
                        break;
                }
                handler?.Invoke(searchType);
            });
        }

        /// <summary>
        /// 绑定查询开始事件
        /// </summary>
        /// <param name="handler">接受事件的函数，参数：string 查询类型，为<see cref="SearchType"/>枚举类型下枚举项的等效字符串</param>
        public void BindOnQuerying(Action<string> handler)
        {
            NeteaseHijacker.E_Requesting(x => handler?.Invoke(x));
        }

        public List<MusicEntity> GetWebMusicEntities()
        {
            if (currentQueryListResult == null) return null;
            List<MusicEntity> musicEntities = new List<MusicEntity>();
            foreach(MusicDetail detail in currentQueryListResult.list)
            {
                MusicEntity entity = new MusicEntity();
                entity.Album = detail.Album.AlbumName;
            }
            return null;
        }
    }
}
