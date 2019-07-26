using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lunalipse.Resource;
using Lunalipse.Resource.Generic.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunalipseCoreTest
{
    [TestClass]
    public class LrssTest
    {
        [TestMethod]
        public void LrssExporter()
        {
            LrssWriter le = new LrssWriter();
            le.Initialize(0x2F30, "Song", Encoding.ASCII.GetBytes("PrincessLuna"));
            le.AppendResource(@"F:\M2\Daniel Ingram - Luna's Future.mp3");
            le.Export();
        }

        [TestMethod]
        public void LrssImporter()
        {
            /*LrssReader lr = new LrssReader("exp.lrss", Encoding.ASCII.GetBytes("PrincessLuna"));
            LrssIndex[] lis = lr.GetIndex();
            foreach (LrssIndex li in lis)
            {
                PrintClass(typeof(LrssIndex), li);
            }
            LrssResource ls = lr.ReadResource(lis[0]).Result;
            Assert.IsNotNull(ls);
            Assert.IsTrue(ls.ToFile("."));
            lr.Dispose();*/
        }

        private void PrintClass(Type t, object instance)
        {
            foreach (FieldInfo fi in t.GetFields())
            {
                Console.WriteLine("{0}   =>   {1}", fi.Name, fi.GetValue(instance));
            }
            Console.WriteLine();
        }
    }
}
