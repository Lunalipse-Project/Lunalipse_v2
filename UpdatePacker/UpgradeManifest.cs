using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LunalipseUpdate
{
    public class UpgradeManifestItem
    {
        public string fileName;
        public string targetPath;
        public Operations operation;
    }

    [XmlRoot("Manifest")]
    public class UpgradeManifest
    {
        public List<UpgradeManifestItem> Items;
        public UpgradeManifest()
        {
            Items = new List<UpgradeManifestItem>();
        }
    }

    public enum Operations
    {
        [XmlEnum(Name = "Delete")]
        Delete,
        [XmlEnum(Name = "Execution")]
        Run,
        [XmlEnum(Name = "ReplaceOrAdd")]
        ReplaceOrAdd
    }
}
