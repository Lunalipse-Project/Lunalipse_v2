using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lunalipse.Core.Lyric;
using Lunalipse.Common.Data;
using System.Collections.Generic;
using Lunalipse.Core.BehaviorScript;
using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Core;
using Lunalipse.Core.Theme;
using Lunalipse.Common;

namespace LunalipseCoreTest
{
    [TestClass]
    public class ParserTest
    {
        LyricTokenizer lt;
        Interpreter intp;
        [TestInitialize]
        public void Initialize()
        {
            //lt = LyricTokenizer.INSTANCE;
            //intp = Interpreter.INSTANCE(@"F:\Lunalipse\TestUnit\bin\Debug");
            //ErrorDelegation.OnErrorRaisedBSI += (x, y, z) =>
            //{
            //    Console.WriteLine("ERROR:{0}   |   {2} => {1}", x, y, z);
            //};
            //Assert.IsTrue(intp.Load("prg2"));
        }
        [TestMethod]
        public void LyricTokenizerTest()
        {
            List<LyricToken> outp = lt.CreateTokensFromFile(@"F:\M2\Lyrics\亚历山大红旗歌舞团 - 万岁,我们强大的祖国.lrc");
            CollectionAssert.AllItemsAreUnique(outp);
            outp.ForEach((ltk) =>
            {
                Console.WriteLine("{0} => {1} => {2}", ltk.TimeStamp, ltk.Statement, ltk.SubStatement);
            });
            //Assert.IsNotNull(lt.ParseStatement("[00:14.79]Got my guitar shreddin' up the latest tune", ref offset));
        }

        [TestMethod]
        public void ScriptToenizerTest()
        {
            List<ActionToken> ts = intp.Actions;
            Assert.IsTrue(ts.Count > 0);
            foreach(ActionToken st in ts)
            {
                Console.WriteLine("Command: {0} | Args:[ {1} ]", st.CommandType, string.Join(",", st.ct_args));
                Console.WriteLine("Command: {0} | Args:[ {1} ]", st.SuffixType, string.Join(",", st.st_args));
                Console.WriteLine();
            }
        }

        [TestMethod]
        public void ThemeJsonParserTest()
        {
            LThemeParser jtp = new LThemeParser();
            jtp.ErrorRaised += Jtp_ErrorRaised;
            jtp.LoadAllTheme();
            Assert.AreEqual(1,jtp.Tuples.Count);
        }

        private void Jtp_ErrorRaised(string arg1, string[] arg2)
        {
            Console.WriteLine(arg1);
        }

        [TestMethod]
        public void ScriptSerializationTest()
        {
            Assert.IsTrue(intp.SaveAs("prg2.ld"));
        }
    }
}
