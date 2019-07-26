using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lunalipse.Resource;
using Lunalipse.Resource.Generic.Types;

namespace LunalipseUpdate.Procedures
{
    public class Unpacking : IProcedure
    {
        const string Progress = "正在展开Lunalipse更新包...";
        const string ProgressDetail = "正在展开Lunalipse更新包...({0}{1})";
        LrssReader lrssReader;
        public Unpacking(params object[] args)
        {
            lrssReader = new LrssReader();
        }

        public void Main()
        {
            ProcedureHelper.UpdateProgress(Progress, 0);
            lrssReader.LoadLrssCompressed(ProcedureHelper.updateCompressedPackage);
            List<LrssIndex> indexes = lrssReader.GetIndex();
            int total = indexes.Count, current = 1;
            foreach (LrssIndex index in indexes)
            {
                LrssResource lrssResource = lrssReader.ReadResource(index);
                ProcedureHelper.UpdateProgress(string.Format(ProgressDetail, lrssResource.Name, lrssResource.Type), current / total);
                Thread.Sleep(200);
                string path = string.Format("{0}{1}{2}", ProcedureHelper.updateFolder, lrssResource.Name, lrssResource.Type);
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    fs.Write(lrssResource.Data, 0, lrssResource.Data.Length);
                }
                current++;
            }
        }
    }
}
