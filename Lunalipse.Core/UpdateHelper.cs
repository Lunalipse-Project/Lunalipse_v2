using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Lunalipse.Common;
using LunaNetCore;
using LunaNetCore.Bodies;
using Newtonsoft.Json;

namespace Lunalipse.Core
{
    public class UpdateHelper
    {
        const string UPDATE_QUERY_LATEST = "https://api.github.com/repos/Lunalipse-Project/Lunalipse_v2/releases/latest";
        const string UPDATE_QUERY_ALL = "https://api.github.com/repos/Lunalipse-Project/Lunalipse_v2/releases";

        /// <summary>
        /// 紧急URL，当GitHub上的项目发生变化时，从这个地方获取更新后的信息
        /// </summary>
        const string EMERGENCY_URL = "https://product.lunaixsky.com/lunalipse/api/emergency";

        LNetC LunaNet;
        LunalipseLogger logger;
        VersionHelper versionHelper;
        List<ReleaseInfo> releaseInfos = null;

        readonly Regex TagPatternMatch = new Regex("v([0-9.]+)-([a-z]+)", RegexOptions.IgnoreCase);
        readonly Regex UpdatePackDownload = new Regex("https://.*/updatepack.lrss", RegexOptions.IgnoreCase);
        readonly Regex UpdateProgram = new Regex("https://.*/update.exe", RegexOptions.IgnoreCase);

        public event Action OnQueryCompleted;
        public event Action<WebExceptionStatus, HttpStatusCode, string, string> OnErrorOccurs;

        public UpdateHelper()
        {
            LunaNet = new LNetC();
            logger = LunalipseLogger.GetLogger();
            LunaNet.OnHttpResponded += LunaNet_OnHttpResponded;
            LunaNet.OnErrorOccurs += LunaNet_OnErrorOccurs;
            versionHelper = VersionHelper.Instance;
        }

        private void LunaNet_OnErrorOccurs(WebExceptionStatus webExceptionStatus, HttpStatusCode httpStatusCode, string Description, string message)
        {
            System.Console.WriteLine("Exception:{0}\n Status:{1} {2}", webExceptionStatus.ToString(), (int)httpStatusCode, Description);
            OnErrorOccurs?.Invoke(webExceptionStatus, httpStatusCode, Description, message);
            OnQueryCompleted?.Invoke();
        }

        private void LunaNet_OnHttpResponded(string RequestID, RResult rrs)
        {
            if(rrs!=null)
            {
                if (RequestID == "UPDATE_QUERY_LATEST")
                {
                    ReleaseInfo releaseInfo = JsonConvert.DeserializeObject<ReleaseInfo>(rrs.ResultData);
                    releaseInfos = new List<ReleaseInfo>() { releaseInfo };
                    OnQueryCompleted?.Invoke();
                }
                else if (RequestID == "UPDATE_QUERY_ALL")
                {
                    releaseInfos = JsonConvert.DeserializeObject<List<ReleaseInfo>>(rrs.ResultData);
                    OnQueryCompleted?.Invoke();
                }
            }
        }

        public void QueryLatestUpdate()
        {
            LunaNet.AddRequestBody(new RBody()
            {
                RequestMethod = HttpMethod.GET,
                URL = UPDATE_QUERY_LATEST
            }, "UPDATE_QUERY_LATEST");
            LunaNet.RequestAsyn();
        }

        public void QueryUpdateAll()
        {
            LunaNet.AddRequestBody(new RBody()
            {
                RequestMethod = HttpMethod.GET,
                URL = UPDATE_QUERY_ALL
            }, "UPDATE_QUERY_ALL");
            LunaNet.RequestAsyn();
        }

        public ReleaseInfo UpdateAvailability(bool PreReleaseOnly = false)
        {
            if (releaseInfos == null || releaseInfos.Count == 0)
            {
                return null;
            }
            ReleaseInfo releaseInfo = releaseInfos.Find((ri) => ri.IsPreRelease == PreReleaseOnly);
            Match match = TagPatternMatch.Match(releaseInfo.Tag);
            if (match.Groups.Count == 0) return null;
            Version v = new Version(match.Groups[1].Value);
            releaseInfo.postFix = match.Groups[2].Value;
            return v > versionHelper.Version ? releaseInfo : null;
        }

        public string FindPackDownloadURI(ReleaseInfo releaseInfo)
        {
            foreach(Asset asset in releaseInfo.Assets)
            {
                if (UpdatePackDownload.IsMatch(asset.DownloadURL))
                {
                    return asset.DownloadURL;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// This method will return a uri when there is newer version of update program available
        /// </summary>
        /// <param name="releaseInfo"></param>
        /// <returns></returns>
        public string FindUpdateExeDownloadURI(ReleaseInfo releaseInfo)
        {
            foreach (Asset asset in releaseInfo.Assets)
            {
                if (UpdateProgram.IsMatch(asset.DownloadURL))
                {
                    return asset.DownloadURL;
                }
            }
            return string.Empty;
        }

        public string CurrentVersion
        {
            get
            {
                return versionHelper.getFullVersion();
            }
        }
    }

    /*
     * Tag formate:
     *      v{VERSION_NUMBER}-{POSTFIX}
     */

    public class ReleaseInfo
    {
        [JsonProperty("tag_name")]
        public string Tag;
        //[JsonProperty("id")]
        //public int ID;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("published_at")]
        public string PublishDate;
        [JsonProperty("assets")]
        public Asset[] Assets;
        [JsonProperty("body")]
        public string body;
        [JsonProperty("prerelease")]
        public bool IsPreRelease;

        /// <summary>
        /// Something like "build", "alpha", "beta" or "release"
        /// </summary>
        public string postFix = "";
    }

    public class Asset
    {
        [JsonProperty("name")]
        public string name;
        [JsonProperty("content_type")]
        public string ContentType;
        [JsonProperty("browser_download_url")]
        public string DownloadURL;
        [JsonProperty("size")]
        public int FileSize;
        /// <summary>
        /// 下载次数
        /// </summary>
        [JsonProperty("download_count")]
        public int DownloadCount;
    }
}
