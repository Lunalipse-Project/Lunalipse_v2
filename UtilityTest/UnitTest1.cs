using System;
using Lunalipse.Utilities.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UtilityTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CommandParserTest()
        {
            string[] s = LpsCommandParser.ParseCommand("cmd -v \"some text and some \\\"text\\\"\" -s -j");
            Assert.IsTrue(s.Length != 0);
        }

        [TestMethod]
        public void CommandQuoteCheck()
        {
            Assert.IsFalse(LpsCommandParser.CheckQuote("cmd -v \"some text and some \\\"text\\\" -s -j"));
        }
    }
}
