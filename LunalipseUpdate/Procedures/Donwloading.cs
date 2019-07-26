using System.Net;

namespace LunalipseUpdate.Procedures
{
    public class Downloading : IProcedure
    {
        string downloadUrl;
        int size = 0;
        const string Progress = "正在下载Lunalipse更新包...";
        public Downloading(string downloadUrl, int size)
        {
            this.downloadUrl = downloadUrl;
            this.size = size;
        }
        public void Main()
        {
            ProcedureHelper.UpdateProgress(Progress, -1);
            DownloadViaGeneric();
        }

        private void DownloadViaWebClient()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFile(downloadUrl, ProcedureHelper.updateCompressedPackage);
        }

        private void DownloadViaGeneric()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest Myrq = (HttpWebRequest)WebRequest.Create(downloadUrl);
            Myrq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:53.0) Gecko/20100101 Firefox/53.0";
            Myrq.Method = "GET";
            Myrq.Credentials = CredentialCache.DefaultNetworkCredentials;

            HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
            long totalBytes = size;
            ProcedureHelper.UpdateProgress(Progress, 0);
            System.IO.Stream st = myrp.GetResponseStream();
            System.IO.Stream so = new System.IO.FileStream(ProcedureHelper.updateCompressedPackage, System.IO.FileMode.Create);
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
                    ProcedureHelper.UpdateProgress(Progress, (double)totalDownloadedByte / (double)totalBytes);
                    osize = st.Read(by, 0, 1024);
                }
            }
            so.Close();
            st.Close();
        }
    }
}
