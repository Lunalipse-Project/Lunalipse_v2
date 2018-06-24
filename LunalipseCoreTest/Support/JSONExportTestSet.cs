using Lunalipse.Common.Data.Attribute;
using Lunalipse.Common.Interfaces.ICache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunalipseCoreTest.Support
{
    public class JSONExportTestSet : ICachable
    {
        [Cachable]
        Dictionary<string, TestClass> dic = new Dictionary<string, TestClass>();
        public JSONExportTestSet()
        {
            dic.Add("No1", new TestClass(1));
            dic.Add("No2", new TestClass(2));
        }

    }

    public class TestClass : ICachable
    {
        [Cachable]
        int i = 0;
        [Cachable]
        string str = "Hello";
        [Cachable]
        double pi = 3.14;
        [Cachable]
        List<string> strs = new List<string>()
        {
            "1aa","2ss","32ss"
        };
        public TestClass(int a)
        {
            i = a;
        }
        public TestClass()
        {

        }
    }
}
