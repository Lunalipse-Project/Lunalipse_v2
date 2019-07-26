using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace LunalipseUpdate.Procedures
{
    public class Applying : IProcedure
    {

        public Applying()
        {

        }
        public void Main()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UpgradeManifest));
            UpgradeManifest upgradeManifest;
            using (StreamReader streamReader = new StreamReader(ProcedureHelper.updateManifest))
            {
                upgradeManifest = (UpgradeManifest)xmlSerializer.Deserialize(streamReader);
            }
            foreach(UpgradeManifestItem item in upgradeManifest.Items)
            {
                switch(item.operation)
                {
                    case Operations.Delete:
                        ProcedureHelper.UpdateProgress("移除" + item.targetPath, -1);
                        File.Delete(item.targetPath);
                        break;
                    case Operations.ReplaceOrAdd:
                        ProcedureHelper.UpdateProgress("替换" + item.targetPath, -1);
                        if (File.Exists(item.targetPath))
                        {
                            File.Delete(item.targetPath);
                        }
                        File.Move(ProcedureHelper.updateFolder + item.fileName, item.targetPath);
                        break;
                    case Operations.Run:
                        ProcedureHelper.UpdateProgress("运行" + item.fileName, -1);
                        Process.Start(ProcedureHelper.updateFolder + item.fileName);
                        break;
                }
            }
        }
    }
}
