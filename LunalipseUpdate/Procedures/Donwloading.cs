using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseUpdate.Procedures
{
    public class Downloading : IProcedure
    {
        string downloadUrl;
        const string Progress = "正在下载Lunalipse更新包...";
        public Downloading(string downloadUrl)
        {
            this.downloadUrl = downloadUrl;
        }
        public void Main()
        {
            ProcedureHelper.UpdateProgress(Progress, -1);
            WebClient webClient = new WebClient();
            webClient.DownloadFile(downloadUrl, ProcedureHelper.updateCompressedPackage);
        }
    }
}
