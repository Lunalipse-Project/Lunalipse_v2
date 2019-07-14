using LunaNetCore.Bodies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LunaNetCore
{
    /// <summary>
    /// Http请求驱动类
    /// </summary>
    internal class HttpHelper
    {
        const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.103 Safari/537.36";
        public static HttpWebResponse CreateGetHttpResponse(RBody rBody, int timeout, IWebProxy proxy = null)
        {
            HttpWebRequest request = null;
            request = WebRequest.Create(rBody.URL) as HttpWebRequest;
            request.Method = "GET";
            request.AutomaticDecompression = rBody.DecompressionMethods;
            request.ContentType = rBody.ContentType;
            request.UserAgent = USER_AGENT;
            request.Accept = rBody.Accept;
            request.Referer = rBody.Referer;
            if(proxy!=null)
            {
                request.Proxy = proxy;
            }

            //设置代理UserAgent和超时
            request.Timeout = timeout;
            if (rBody.RequestCookie != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(rBody.RequestCookie);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public static HttpWebResponse CreatePostHttpResponse(RBody rBody, int timeout, IWebProxy proxy = null)
        {
            HttpWebRequest request = null;
            request = WebRequest.Create(rBody.URL) as HttpWebRequest;
            request.Method = "POST";
            request.AutomaticDecompression = rBody.DecompressionMethods;
            request.ContentType = rBody.ContentType;
            request.UserAgent = USER_AGENT;
            request.Accept = rBody.Accept;
            request.Referer = rBody.Referer;
            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            request.Timeout = timeout;
            if (proxy != null)
            {
                request.Proxy = proxy;
            }


            if (rBody.RequestCookie != null || rBody.RequestCookie.Count != 0)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(rBody.RequestCookie);
            }
            //发送POST数据  
            if (!(rBody.RequestParameter == null || rBody.RequestParameter.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in rBody.RequestParameter.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, rBody.RequestParameter[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, rBody.RequestParameter[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            try
            {
                using (Stream s = webresponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(s, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }
}
