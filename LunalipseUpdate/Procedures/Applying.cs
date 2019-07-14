using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        File.Delete(item.targetPath);
                        break;
                    case Operations.ReplaceOrAdd:
                        if(File.Exists(item.targetPath))
                        {
                            File.Delete(item.targetPath);
                        }
                        File.Move(ProcedureHelper.updateFolder + item.fileName, item.targetPath);
                        break;
                    case Operations.Run:
                        Process.Start(ProcedureHelper.updateFolder + item.fileName);
                        break;
                }
            }
        }
    }
}
