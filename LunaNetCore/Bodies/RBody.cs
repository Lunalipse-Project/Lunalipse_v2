using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LunaNetCore.Bodies.ParasitismBundle;

namespace LunaNetCore.Bodies
{
    /// <summary>
    /// 请求体
    /// </summary>
    public class RBody
    {
        public RBody()
        {
            RequestParameter = new Dictionary<string, string>();
            RequestCookie = new CookieCollection();
        }

        /// <summary>
        /// 添加一个请求参数。
        /// <para>
        /// 此参数将会被自动拼接在请求URL上，如
        /// <code>
        /// www.example.com/expl.html?key=val
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="val">参数值</param>
        public void AddParameter(string key,string val)
        {
            RequestParameter.Add(key, val);
        }

        /// <summary>
        /// 获取参数个数
        /// </summary>
        /// <returns></returns>
        public int GetTotalParameters()
        {
            return RequestParameter.Count;
        }

        /// <summary>
        /// 获取或设置请求方式
        /// </summary>
        public HttpMethod RequestMethod { get; set; }

        /// <summary>
        /// 获取或设置请求目标URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 获取或设置Cookie
        /// <para>
        /// 推荐使用<seealso cref="AddCoockie"/>来逐个添加Cookie
        /// </para>
        /// </summary>
        public CookieCollection RequestCookie { get; set; }

        /// <summary>
        /// 获取请求参数，如需设置参数请使用<seealso cref="AddParameter"/>
        /// </summary>
        public IDictionary<string, string> RequestParameter { get; }

        /// <summary>
        /// 设置Referer
        /// </summary>
        public string Referer { get; set; } = "";
        public string Accept { get; set; } = "*/*";
        public string ContentType { get; set; } = "";
        public DecompressionMethods DecompressionMethods { get; set; }
            = DecompressionMethods.Deflate | DecompressionMethods.GZip;

        public Action<string, string> BodyBundle { get; set; }


        public string PatchParameter()
        {
            string para = "?";
            foreach(var v in RequestParameter)
            {
                para += v.Key + "=" + System.Web.HttpUtility.UrlEncode(v.Value) + "&";
            }
            para.Remove(para.Length - 1,1);
            return para;
        }

        /// <summary>
        /// 添加一个请求Cookie
        /// </summary>
        /// <param name="name">Cookie名称</param>
        /// <param name="value">Cookie的值</param>
        /// <param name="expr">Cookie过期时间</param>
        public void AddCoockie(string name,string value,DateTime expr)
        {
            Cookie c = new Cookie();
            c.Name = name;
            c.Value = value;
            c.Expires = expr;
            c.Domain = "music.163.com";
            RequestCookie.Add(c);
        }
    }
}
