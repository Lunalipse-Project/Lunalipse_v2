using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using Lunalipse.Common;
using Lunalipse.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunalipseCoreTest
{
    [TestClass]
    public class UtilsTest
    {
        [TestMethod]
        public void CommandParser()
        {
            string command = "abc -s \"sc ss,abc+s\" -b 34 -c 2.d";
            string[] args = Utils.ParseCommand(command);
            Assert.AreEqual(7, args.Length);
            foreach(string s in args)
            {
                Console.WriteLine(s);
            }
        }

        [TestMethod]
        public void Misc()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>()
            {
                {2,"ab" }
            };
            List<int> l = new List<int>();
            Type type1 = typeof(Dictionary<,>.Enumerator);
            Console.WriteLine(dic.GetType().GetProperty("Keys").GetValue(dic, new object[] { }).ToString());
            //foreach (PropertyInfo mi in dic.GetType().GetProperties())
            //{
            //    Console.WriteLine("Properties: {0}", mi.Name);
            //    Console.WriteLine("{1} {0}(", mi.GetMethod.Name, mi.GetMethod.ReturnType);

            //    foreach (ParameterInfo t in mi.GetMethod.GetParameters())
            //    {
            //        Console.Write("{0} {1}, ", t.ParameterType, t.Name);
            //    }
            //    Console.WriteLine(")");
            //}
        }
        [TestMethod]
        public void ForegroundPredict()
        {
            Color c = Color.FromRgb(255,255,255);
            Console.WriteLine(c.GetForeground());
        }
    }
}
