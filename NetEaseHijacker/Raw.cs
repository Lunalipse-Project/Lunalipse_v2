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
        LNetC lnc;
        public Raw()
        {
            lnc = new LNetC();
        }

        public void Event_AllComplete(LNetC.AllQueueRequestCompletely e)
        {
            lnc.OnAllQueueRequestCompletely += e;
        }

        public void Event_Requesting(LNetC.HttpRequesting e)
        {
            lnc.OnHttpRequesting += e;
        }

        public void Event_Responded(LNetC.HttpResponded e)
        {
            lnc.OnHttpResponded += e;
        }

        public void Event_TimeOut(LNetC.HttpTimeOut e)
        {
            lnc.OnHttpTimeOut += e;
        }

        public async Task Get(SearchType st,params string[] args)
        {
            string param = "";
            string url = "";
            string id;
            //lnc.ClearReqSeq();
            switch(st)
            {
                case SearchType.SONGS:
                    param = NeParams.SEARCH.FormatE(args);
                    url = NeParams.NE_SEARCH;
                    id = st.ToString();
                    break;
                case SearchType.DETAIL:
                    param = NeParams.DETAIL.FormatE(args);
                    url = NeParams.NE_DETAIL;
                    id = st.ToString();
                    break;
                case SearchType.DOWNLOAD:
                    param = NeParams.DOWNLOAD.FormatE(args);
                    url = NeParams.NE_DOWNLOAD;
                    id = st.ToString();
                    break;
                case SearchType.LYRIC:
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
            lnc.AddRequestBody(r, id);
            await lnc.RequestAsyn();
        }
    }
}
