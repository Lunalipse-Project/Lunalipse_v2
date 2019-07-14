using Lunalipse.Resource;
using Lunalipse.Resource.Generic.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestInspect
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex regex = new Regex(@"(<?)[https?://]?.*(>?)");
            GroupCollection gc = regex.Match("cds<http://www.baid.com>abc").Groups;
            //Console.WriteLine(regex.IsMatch());
            Console.ReadKey();
            //List<string> Bold = new List<string>();
            //List<string> Italic = new List<string>();
            //string testString = "**Test** This is a ***Test*** String, with *Italic* and **Bold**";
            //string[] spt = testString.Split(new[] { "**" }, StringSplitOptions.None);
            //for (int i = 0; i < spt.Length; i += 1)
            //{
            //    if (i != 0 && i % 2 != 0)
            //    {
            //        Bold.Add(spt[i]);
            //    }
            //    if (spt[i].Contains('*'))
            //    {
            //        string[] spt2 = spt[i].Split(new[] { "*" }, StringSplitOptions.None);
            //        for (int j = 0; j < spt2.Length; j++)
            //        {
            //            if (j != 0 && j % 2 != 0)
            //            {
            //                Italic.Add(spt2[j]);
            //            }
            //        }
            //    }
            //}
            //MarkdownParserTest markdownParserTest = new MarkdownParserTest();
            //markdownParserTest.FindBoldOrItalic();
        }
    }
}
