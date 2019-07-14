using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LunaNetCore
{
    /// <summary>
    /// 下载器
    /// </summary>
    public class Downloader
    {
        /// <summary>
        /// 委托：下载完成
        /// </summary>
        /// <param name="gotError">是否产生错误</param>
        /// <param name="e">如果有错误产生，错误的详情</param>
        public delegate void ODF(bool gotError, Exception e);

        /// <summary>
        /// 委托：下载器已就位
        /// </summary>
        /// <param name="totalLength"></param>
        public delegate void Prepared(long totalLength);

        /// <summary>
        /// 委托：下载状态更新
        /// </summary>
        /// <param name="currentLength"></param>
        public delegate void Update(long currentLength);

        /// <summary>
        /// 事件：下载完成
        /// </summary>
        public event ODF OnDownloadFinish;

        /// <summary>
        /// 事件：下载器准备就绪
        /// </summary>
        public event Prepared OnPrepared;

        /// <summary>
        /// 事件：下载状态更新
        /// </summary>
        public event Update OnTaskUpdate;

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="URL">文件URL</param>
        /// <param name="filename">文件保存路径</param>
        public void DownloadFile(string URL, string filename)
        {
            try
            {
                HttpWebRequest Myrq = (HttpWebRequest)WebRequest.Create(URL);
                Myrq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";
                Myrq.Method = "GET";
                Myrq.Credentials = CredentialCache.DefaultNetworkCredentials;

                HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                OnPrepared?.Invoke(totalBytes);
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = 0;
                while (totalDownloadedByte < totalBytes)
                {
                    osize = st.Read(by, 0, (int)by.Length);
                    while (osize > 0)
                    {
                        totalDownloadedByte = osize + totalDownloadedByte;
                        so.Write(by, 0, osize);
                        OnTaskUpdate?.Invoke(totalDownloadedByte);
                        osize = st.Read(by, 0, 1024);
                    }
                }
                so.Close();
                st.Close();
                OnDownloadFinish?.Invoke(false, null);
            }
            catch (Exception e)
            {
                OnDownloadFinish?.Invoke(true, e);
            }
        }
    }
}
