using System;
using System.Collections.Generic;
using Lunalipse.Utilities.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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

        [TestMethod]
        public void TestJsonSerialization()
        {
            Console.WriteLine(JsonConvert.SerializeObject(new exampleClass()));
        }
    }

    class exampleClass
    {
        public string test1 = "abc";
        public int test2 = 321;
        public TimeSpan TimeSpan = new TimeSpan(1, 2, 3);
        public List<SubClass> classes = new List<SubClass>();
        [NonSerialized]
        public double[] test3 = new double[] { 0.3, 0.5, 32.1 };
        private int test4 = 222;

        public exampleClass()
        {
            classes.Add(new SubClass(1));
            classes.Add(new SubClass(2));
            classes.Add(new SubClass(3));
        }
    }
    class SubClass
    {
        public int i = 0;
        public SubClass(int i)
        {
            this.i = i;
        }
    }
}
