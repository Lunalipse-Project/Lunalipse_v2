using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LunalipseUpdate.Procedures
{
    public class CleanUp : IProcedure
    {
        public void Main()
        {
            ProcedureHelper.UpdateProgress("清理临时文件....", -1);
            Directory.Delete(ProcedureHelper.updateFolder, true);
        }
    }
}
