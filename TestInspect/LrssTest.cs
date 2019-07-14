using Lunalipse.Resource;
using Lunalipse.Resource.Generic.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestInspect
{
    class LrssTest
    {

        static void Write()
        {
            Console.WriteLine(" ======= LRSS WRITE TEST =========");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            LrssWriter lw = new LrssWriter();
            lw.Initialize(0x2333, "MLP_SONGS", "export.lrss", Encoding.ASCII.GetBytes("2WS_=3+?"));
            lw.AppendResourcesDir(@"F:\MLPMUSIC");
            lw.Export().Wait();
            sw.Stop();
            Console.WriteLine(sw.Elapsed.ToString());
        }

        static async void Read()
        {
            Console.WriteLine(" ======= LRSS READ TEST =========");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            LrssReader lr = new LrssReader("export.lrss", Encoding.ASCII.GetBytes("2WS_=3+?"));
            Directory.CreateDirectory("EXPORT");
            foreach (LrssIndex li in lr.GetIndex())
            {
                LrssResource lrr = await lr.ReadResource(li);
                if (lrr == null)
                {
                    Console.WriteLine("Error: Can not export.\n\t Wrong key or damaged file.");
                    break;
                }
                lrr.ToFile("EXPORT");
            }
            lr.Dispose();
            sw.Stop();
            Console.WriteLine(sw.Elapsed.ToString());
        }
    }
}
