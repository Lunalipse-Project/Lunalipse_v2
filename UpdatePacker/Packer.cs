using Lunalipse.Resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UpdatePacker
{
    class Packer
    {
        LrssWriter lrssWriter = new LrssWriter();
        public string BaseDirectory { get; private set; } = string.Empty;
        public List<string> SupportedExtension = new List<string> { ".exe", ".dll" };
        public string ManifestContent { get; private set; }

        public void setFileRoot(string directory)
        {
            foreach(string file in Directory.GetFiles(directory))
            {
                string ext = Path.GetExtension(file);
                if (ext == ".dll" || ext == ".exe")
                {
                    FileInfo fileInfo = new FileInfo(file);
                    WatchedFile watchedFile = new WatchedFile();
                    watchedFile.AbsolutePath = file;
                    watchedFile.FileName = Path.GetFileName(file);
                    watchedFile.State = ModifiedState.NOCHANGE;
                    watchedFile.LastModify = fileInfo.LastWriteTime;
                    WatchedFiles.Add(watchedFile);
                }
            }
            BaseDirectory = directory;
        }

        public ObservableCollection<WatchedFile> WatchedFiles { get; private set; } = new ObservableCollection<WatchedFile>();

        public bool parseCache()
        {
            if (!File.Exists("files.xml")) return false;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileTreeCache));
            FileTreeCache cache;
            using (StreamReader streamReader = new StreamReader("files.xml"))
            {
                cache = (FileTreeCache)xmlSerializer.Deserialize(streamReader);
            }
            foreach(WatchedFile watchedFile in cache.watchedFiles)
            {
                WatchedFiles.Add(watchedFile);
            }
            BaseDirectory = cache.RootDirectory;
            return true;
        }

        public void MarkChanged()
        {
            if (BaseDirectory == string.Empty) return;
            string[] currentFile = Directory.GetFiles(BaseDirectory);
            List<WatchedFile> existedFile = WatchedFiles.ToList();
            foreach (WatchedFile watchedFile in WatchedFiles)
            {
                if (!currentFile.Contains(watchedFile.AbsolutePath))
                {
                    watchedFile.State = ModifiedState.DELETED;
                }
            }
            foreach (string file in currentFile)
            {
                if (!SupportedExtension.Contains(Path.GetExtension(file))) continue;
                WatchedFile FILE;
                DateTime dateTime = File.GetLastWriteTime(file);
                if ((FILE = existedFile.Find(x => x.AbsolutePath == file)) != null)
                {
                    if (dateTime != FILE.LastModify)
                    {
                        FILE.LastModify = dateTime;
                        FILE.State = ModifiedState.CHANGED;
                    }
                }
                else
                {
                    WatchedFiles.Add(new WatchedFile()
                    {
                        State = ModifiedState.ADD,
                        AbsolutePath = file,
                        FileName = Path.GetFileName(file),
                        LastModify = dateTime
                    });
                }
            }
        }

        public void ExportTreeCache()
        {
            FileTreeCache cache = new FileTreeCache()
            {
                RootDirectory = BaseDirectory,
                watchedFiles = WatchedFiles.ToList()
            };
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileTreeCache));
            using (StreamWriter writer = File.CreateText("files.xml"))
            {
                xmlSerializer.Serialize(writer, cache);
            }
        }

        public void PreparePack()
        {
            if (!Directory.Exists("temp")) Directory.CreateDirectory("temp");
            List<UpgradeManifestItem> upgradeManifestItems = new List<UpgradeManifestItem>();
            UpgradeManifest upgradeManifest = new UpgradeManifest();
            foreach(WatchedFile watchedFile in WatchedFiles)
            {
                if (watchedFile.State == ModifiedState.NOCHANGE) continue;
                upgradeManifestItems.Add(new UpgradeManifestItem()
                {
                    fileName = watchedFile.FileName,
                    operation = ModifState2Opts(watchedFile.State),
                    targetPath = watchedFile.FileName
                });
                if(watchedFile.State != ModifiedState.DELETED)
                {
                    if(!File.Exists("temp/"+watchedFile.FileName))
                    {
                        File.Copy(watchedFile.AbsolutePath, "temp/" + watchedFile.FileName);
                    }
                }
            }
            upgradeManifest.Items = upgradeManifestItems;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpgradeManifest));
            using (MemoryStream ms = new MemoryStream())
            {
                xmlSerializer.Serialize(ms, upgradeManifest);
                ms.Seek(0, SeekOrigin.Begin);
                ManifestContent = Encoding.UTF8.GetString(ms.ToArray());
                ms.Seek(0, SeekOrigin.Begin);
                using (FileStream sw = new FileStream("temp/manifest.xml", FileMode.Create))
                {
                    ms.CopyTo(sw);
                }
            }
        }

        public void ExportUpdatePack(string TargetPath)
        {
            lrssWriter.Initialize(0x7EFF, "Lunalipse Update Package");
            lrssWriter.AppendResourcesDir("temp/");
            lrssWriter.Export();
            Compression.CompressTo(lrssWriter.OuputStream, TargetPath + "/update.clrss");
            Directory.Delete("temp", true);
            WatchedFiles.ToList().RemoveAll(x => x.State == ModifiedState.DELETED);
            foreach(WatchedFile watchedFile in WatchedFiles)
            {
                watchedFile.State = ModifiedState.NOCHANGE;
            }
        }

        private Operations ModifState2Opts(ModifiedState modifiedState)
        {
            switch(modifiedState)
            {
                case ModifiedState.ADD:
                case ModifiedState.CHANGED:
                    return Operations.ReplaceOrAdd;
                case ModifiedState.RUN:
                    return Operations.Run;
                case ModifiedState.DELETED:
                    return Operations.Delete;
                default:
                    return Operations.ReplaceOrAdd;
            }
        }
    }
}
