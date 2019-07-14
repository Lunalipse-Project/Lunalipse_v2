using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestInspect
{
    class MarkdownParserTest
    {
        Stack<char> charStack = new Stack<char>();
        string testString = "This is a ***Test*** String, with *Italic* and **Bold**";
        public void FindBoldOrItalic(string Markdown)
        {
            List<TextFormate> FormatedString = new List<TextFormate>();
            int index = 0;
            int starsBegin = 0, starsEnd = 0;
            string textBetween = "";
            while ((index = Markdown.IndexOf('*')) != -1)
            {
                starsBegin = 0;
                starsEnd = 0;
                textBetween = "";
                for (int i = index; i < Markdown.Length && Markdown[i] == '*'; i++, starsBegin++) ;
                for(int i=index+starsBegin;i< Markdown.Length && Markdown[i] != '*'; i++)
                {
                    textBetween += Markdown[i];
                }
                for (int i = index + starsBegin + textBetween.Length; i < Markdown.Length && Markdown[i] == '*'; i++, starsEnd++) ;
                //Markdown.Replace()
                int reference = Math.Min(starsBegin, starsEnd);
                
                Markdown = Markdown.Remove(index, starsBegin + textBetween.Length + starsEnd);
                string strBefore = Markdown.Substring(0, index);
                Markdown = Markdown.Remove(0, index);
                FormatedString.Add(new TextFormate()
                {
                    content = strBefore
                });
                FormatedString.Add(new TextFormate()
                {
                    isBold = reference >= 2,
                    isItalic = reference == 3 || reference == 1,
                    content = textBetween
                });
            }
        }
    }
    struct TextFormate
    {
        public bool isBold, isItalic;
        public string content;
    }
}
