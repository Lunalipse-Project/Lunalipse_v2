using System;
using System.Collections.Generic;
using System.Reflection;
using Lunalipse.Common.Data;
using Lunalipse.Core.Cache;
using Lunalipse.Core.I18N;
using Lunalipse.Core.Metadata;
using Lunalipse.Core.PlayList;
using LunalipseCoreTest.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace LunalipseCoreTest
{
    [TestClass]
    public class SerializationTest
    {
        MusicListPool mlp;
        CataloguePool cpl;
        MediaMetaDataReader mmdr;
        [TestInitialize]
        public void Initialize()
        {
            mlp = MusicListPool.INSATNCE;
            //cpl = CataloguePool.INSATNCE;
            mmdr = new MediaMetaDataReader(new I18NConvertor());
            //mlp.AddToPool(@"F:\M2\", mmdr);
        }

        

        [TestMethod]
        public void I18NTest()
        {
            I18NTokenizer it = new I18NTokenizer();
            it.LoadFromFile(@"C:\Users\Lunaixsky\Desktop\Lunalipse I18N\i18n.lang");
            it.GetPages(SupportLanguages.CHINESE_SIM);
            //string s = I18NPage.INSTANCE.GetPage("CORE_FUNC").getContext("CORE_LBS_InfiniteCall");
            //Assert.AreEqual("检测到循环调用", s);
        }

        [TestMethod]
        public void TestCache()
        {
            string str = Compressed.readCompressed("json.txt", false);
            CacheSerializor ch = new CacheSerializor();
            Catalogue c = ch.RestoreTo<Catalogue>(str);
            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void DictionaryInspect()
        {
            JSONExportTestSet jets = new JSONExportTestSet();
            CacheSerializor ch = new CacheSerializor();
            WinterWrapUp wwu = new WinterWrapUp()
            {
                createDate = "00-00-00",
                deletable = true,
                markName = "TestSet",
                uid = "No"
            };
            string res;
            Console.Write(res=ch.CacheTo(jets, wwu));
            JSONExportTestSet jets_res = ch.RestoreTo<JSONExportTestSet>(JObject.Parse(res)["ctx"]);
            Assert.IsNotNull(jets_res);
        }

        private void PrintClass(Type t, object instance)
        {
            foreach(FieldInfo fi in t.GetFields())
            {
                Console.WriteLine("{0}   =>   {1}", fi.Name, fi.GetValue(instance));
            }
            Console.WriteLine();
        }
    }
}
