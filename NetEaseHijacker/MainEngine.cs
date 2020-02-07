using Lunalipse.Common.Interfaces.IWebMusic;
using NetEaseHijacker.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetEaseHijacker
{
    [EngineEntryPoint("Netease_Hijacker")]
    public class MainEngine : MarshalByRefObject, IWebMusicsSearchEngine
    {
        Hijack hijack = new Hijack();
        MusicDetail currentSelectedMusic;
        MetadataNE metadata;
        Tuple<string, string> currentDownloadURL = null;
        string currentLyric;
        Action<Exception> ExceptionHandler;

        public Tuple<string, string> GetDownloadURL(IWebMusicDetail musicDetail, EngineAudioQuality audioQuality)
        {
            MusicSource source = null;
            MusicDetail detail = musicDetail as MusicDetail;
            switch(audioQuality)
            {
                case EngineAudioQuality.QUALITY_HIGH:
                    source = detail.SourceHighQ;
                    break;
                case EngineAudioQuality.QUALITY_STANDARD:
                    source = detail.SourceMediumQ;
                    break;
                case EngineAudioQuality.QUALITY_LOW:
                    source = detail.SourceLowQ;
                    break;
            }
            if (source == null) return null;
            currentDownloadURL = null;
            hijack.DownloadURL(detail.ID.ToString(), source.BitRate.ToString()).Wait();
            return currentDownloadURL;
        }

        public string GetLyric(IWebMusicDetail musicDetail)
        {
            hijack.Lyric(musicDetail.getID()).Wait();
            return currentLyric;
        }

        public IWebMusicDetail GetMusicDetail(string id)
        {
            currentSelectedMusic = metadata.list.Find(x => x.getID().Equals(id));
            return currentSelectedMusic;
        }

        public List<IWebMusicDetail> GetMusics()
        {
            if (metadata != null)
            {
                return metadata.list.ConvertAll<IWebMusicDetail>(x => x);
            }
            return null;
        }

        public void OnLoad()
        {
            
        }

        public void OnUnload()
        {
            hijack.ClearAllEventSubscribers();
            ExceptionHandler = null;
        }

        public void SearchMusicByKeyword(string keyword, int eachPageEntries, int whichPage)
        {
            hijack.SearchSong(keyword, eachPageEntries, eachPageEntries * whichPage);
        }

        public void SetExceptionRaised(Action<Exception> handler)
        {
            ExceptionHandler = handler;
            hijack.E_ErrorOccured(handler);
        }

        public void SetOnQueryRequesting(Action<EngineActionType> handler)
        {
            hijack.E_Requesting(id =>
            {
                EngineActionType engineAction = EngineActionType.QUERY_DOWNLOAD_URL;
                if (Enum.TryParse(id, out engineAction))
                {
                    handler?.Invoke(engineAction);
                }
            });
        }

        public void SetOnQueryReturned(Action<EngineActionType> handler)
        {
            hijack.E_Responded((id,result)=>
            {
                try
                {
                    if (result == string.Empty) return;

                    EngineActionType engineAction = EngineActionType.QUERY_DOWNLOAD_URL;
                    if (Enum.TryParse(id, out engineAction))
                    {
                        switch (engineAction)
                        {
                            case EngineActionType.QUERY_DOWNLOAD_URL:
                                currentDownloadURL = hijack.ParseDownloadURL(result);
                                break;
                            case EngineActionType.QUERY_LYRIC:
                                currentLyric = hijack.ParseLyric(result);
                                break;
                            case EngineActionType.QUERY_SONGS_LIST:
                                metadata = hijack.ParseSongList(result);
                                break;
                        }
                        handler?.Invoke(engineAction);
                    }
                }
                catch(Exception e)
                {
                    ExceptionHandler?.Invoke(e);
                }
            });
        }

        public void SetProxy(IWebProxy proxy)
        {
            hijack.LNetWebProxy = proxy;
        }
    }
}
