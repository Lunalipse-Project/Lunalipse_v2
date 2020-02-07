using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEaseHijacker
{
    static class NeParams
    {
        /// <summary>
        /// 二次加密params所使用的随机数
        /// </summary>
        public const string RandomString = "9QHGy1GtOmUBkN1c";
        /// <summary>
        /// 安全密钥，根据上述随机数生成
        /// </summary>
        public const string encSecKey = "d123fac38adb3e3e326b672b73a86558" +
                                        "4f40dbb515c33c4b12d7092bf0a22695" +
                                        "820742ed91a47b129bc4a7b62ee2166c" +
                                        "b0a1b963c834a9807f4addd2bc9556e2" +
                                        "c71f1fc90a9a0d158459494c36ca1887" +
                                        "f2c6aa868d70e01c6c00ecfe14aac966" +
                                        "a854c43730fe70ceec15e37211bd3c21" +
                                        "940dec2b62d77f9cab5856d96fdc5e9a";
        /// <summary>
        /// 第一次加密params所使用的秘钥
        /// </summary>
        public const string PrimaryKey = "0CoJUm6Qyw8W8jud";

        //URL

        //public const string NE_SEARCH = "http://music.163.com/weapi/search/suggest/web?csrf_token=";
        public const string NE_SEARCH =   "https://music.163.com/weapi/cloudsearch/get/web?csrf_token=";
        public const string NE_DETAIL =   "https://music.163.com/weapi/v3/song/detail?csrf_token=";
        public const string NE_DOWNLOAD = "https://music.163.com/weapi/song/enhance/player/url?csrf_token=";
        public const string NE_LYRIC =    "https://music.163.com/weapi/song/lyric?csrf_token=";

        //Template

        //public static string SEARCH =   "{{\"s\":\"{0}\",\"limit\":\"{1}\",\"csrf_token\":\"\"}}";
        public const string DETAIL =   "{{\"id\":\"{0}\",\"c\":\"[{{\\\"id\\\":\\\"{0}\\\"}}]\",\"csrf_token\":\"\"}}";
        public const string DOWNLOAD = "{{\"ids\":\"[{0}]\",\"br\":{1},\"csrf_token\":\"\"}}";
        public const string LYRIC =    "{{\"id\":\"{0}\",\"lv\":-1,\"tv\":-1,\"csrf_token\":\"\"}}";
        public const string SEARCH = "{{\"s\":\"{0}\",\"type\":\"1\",\"offset\":\"{3}\",\"total\":\"{2}\",\"limit\":\"{1}\",\"csrf_token\":\"\"}}";
        
    }
}
