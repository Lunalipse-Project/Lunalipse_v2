using LunaNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LunaNetCore.Bodies;

namespace NetEaseHijacker
{
    public class Raw
    {
        public LNetC NetworkCore { get; private set; }
        public Raw()
        {
            NetworkCore = new LNetC();
        }

        public async Task Get(SearchType st,params string[] args)
        {
            string param = "";
            string url = "";
            string id;
            //lnc.ClearReqSeq();
            switch(st)
            {
                case SearchType.QUERY_SONGS_LIST:
                    param = NeParams.SEARCH.FormatE(args);
                    url = NeParams.NE_SEARCH;
                    id = st.ToString();
                    break;
                case SearchType.QUERY_SONG_DETAIL:
                    param = NeParams.DETAIL.FormatE(args);
                    url = NeParams.NE_DETAIL;
                    id = st.ToString();
                    break;
                case SearchType.QUERY_DOWNLOAD_URL:
                    param = NeParams.DOWNLOAD.FormatE(args);
                    url = NeParams.NE_DOWNLOAD;
                    id = st.ToString();
                    break;
                case SearchType.QUERY_LYRIC:
                    param = NeParams.LYRIC.FormatE(args);
                    url = NeParams.NE_LYRIC;
                    id = st.ToString();
                    break;
                default:
                    return;
            }
            param = Utils.GetEncodedParams(param);
            RBody r = new RBody()
            {
                URL = url,
                RequestMethod = HttpMethod.POST,
                ContentType = "application/x-www-form-urlencoded",
                Referer = "http://music.163.com/search/"
            };
            r.AddParameter("params", param);
            r.AddParameter("encSecKey", NeParams.encSecKey);
            NetworkCore.AddRequestBody(r, id);
            await NetworkCore.RequestAsyn();
        }
    }
}
