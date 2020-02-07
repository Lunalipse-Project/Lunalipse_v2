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
        public delegate void ODF(Exception e);

        /// <summary>
        /// 委托：下载状态更新
        /// </summary>
        /// <param name="currentLength"></param>
        /// <param name="totalLength"></param>
        public delegate void Update(long currentLength, long totalLength);

        /// <summary>
        /// 事件：下载完成
        /// </summary>
        public event ODF OnDownloadFinish;

        /// <summary>
        /// 事件：下载状态更新
        /// </summary>
        public event Update OnTaskUpdate;

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="URL">文件URL</param>
        /// <param name="filename">文件保存路径</param>
        /// <param name="webProxy">使用的代理</param>
        public void DownloadFile(string URL, string filename, IWebProxy webProxy)
        {
            try
            {
                HttpWebRequest Myrq = (HttpWebRequest)WebRequest.Create(URL);
                Myrq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36";
                Myrq.Method = "GET";
                Myrq.Proxy = webProxy;
                Myrq.Credentials = CredentialCache.DefaultNetworkCredentials;

                HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
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
                        OnTaskUpdate?.Invoke(totalDownloadedByte, totalBytes);
                        osize = st.Read(by, 0, 1024);
                    }
                }
                so.Close();
                st.Close();
                OnDownloadFinish?.Invoke(null);
            }
            catch (Exception e)
            {
                OnDownloadFinish?.Invoke(e);
            }
        }
    }
}
