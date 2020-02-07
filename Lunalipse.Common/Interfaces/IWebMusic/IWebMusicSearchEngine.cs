using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IWebMusic
{
    public interface IWebMusicsSearchEngine
    {
        void OnLoad();
        void OnUnload();
        void SetOnQueryRequesting(Action<EngineActionType> handler);
        void SetOnQueryReturned(Action<EngineActionType> handler);
        void SetExceptionRaised(Action<Exception> handler);
        IWebMusicDetail GetMusicDetail(string id);
        string GetLyric(IWebMusicDetail musicDetail);
        List<IWebMusicDetail> GetMusics();
        void SearchMusicByKeyword(string keyword, int eachPageEntries, int whichPage);
        Tuple<string, string> GetDownloadURL(IWebMusicDetail musicDetail, EngineAudioQuality audioQuality);

        void SetProxy(IWebProxy proxy);
    }

    public enum EngineActionType
    {
        /// <summary>
        /// Search the song by keyword and return a list
        /// </summary>
        QUERY_SONGS_LIST,
        /// <summary>
        /// Get selected song's playback url (audio file url)
        /// </summary>
        QUERY_DOWNLOAD_URL,
        /// <summary>
        /// Get selected song's lyric
        /// </summary>
        QUERY_LYRIC,
        /// <summary>
        /// Get selected song's detail
        /// </summary>
        QUERY_SONG_DETAIL
    }

    public enum EngineAudioQuality
    {
        QUALITY_LOW,
        QUALITY_STANDARD,
        QUALITY_HIGH
    }
}
