using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core
{
    public class ResourcesHandler
    {
        public string version;
        public const string LUNALIPSE_DATA_FILE_EXTENSION = ".lp";
        public ResourcesHandler(Version version)
        {
            this.version = version.ToString().Replace(".", "");
        }
        public void ReleaseResources(string[] ManifestName, string basePath, Assembly asm)
        {
            foreach(string names in ManifestName)
            {
                if (names.StartsWith("Lunalipse.Resources."))
                {
                    string FileName = names.TrimStart("Lunalipse.Resources.".ToCharArray());
                    if (FileName.StartsWith("Data+"))
                        FileName = FileName.Split('.')[0] + LUNALIPSE_DATA_FILE_EXTENSION;
                    FileName = FileName.Replace("+", @"\");
                    FileName = FileName.Replace("@V", version);
                    LunalipseLogger.GetLogger().Debug("Checking " + FileName);
                    WriteToDisk(basePath + @"\" + FileName, asm, names);
                }
            }
        }

        private void WriteToDisk(string path, Assembly asm, string ManifestName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
#if !DEBUG
            else
            {
                if (File.Exists(path))
                    return;
            }
#endif
            byte[] fileContent = new byte[1024];
            using (FileStream fs = new FileStream(path,FileMode.Create))
            {
                using (Stream stream = asm.GetManifestResourceStream(ManifestName))
                {
                    int n = 0;
                    while ((n = stream.Read(fileContent, 0, fileContent.Length)) != 0)
                    {
                        fs.Write(fileContent, 0, n);
                    }
                }
            }
            LunalipseLogger.GetLogger().Debug("{0} -> {1}".FormateEx(ManifestName, path));
        }
    }
}
