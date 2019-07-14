using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.Markdown
{
    public class ParseLines
    {
        public List<TextFormate> ParseSignleLine(string Line)
        {
            List<TextFormate> FormatedString = new List<TextFormate>();
            int index = 0;
            int starsBegin = 0, starsEnd = 0;
            string textBetween = "";
            while ((index = Line.IndexOf('*')) != -1)
            {
                starsBegin = 0;
                starsEnd = 0;
                textBetween = "";
                for (int i = index; i < Line.Length && Line[i] == '*'; i++, starsBegin++) ;
                for (int i = index + starsBegin; i < Line.Length && Line[i] != '*'; i++)
                {
                    textBetween += Line[i];
                }
                for (int i = index + starsBegin + textBetween.Length; i < Line.Length && Line[i] == '*'; i++, starsEnd++) ;
                //Line.Replace()
                int reference = Math.Min(starsBegin, starsEnd);
                
                Line = Line.Remove(index, starsBegin + textBetween.Length + starsEnd);
                string strBefore = Line.Substring(0, index);
                Line = Line.Remove(0, index);
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
            if (FormatedString.Count == 0 || !string.IsNullOrEmpty(Line)) 
            {
                FormatedString.Add(new TextFormate()
                {
                    content = Line
                });
            }
            return FormatedString;
        }
    }
    public struct TextFormate
    {
        public bool isBold, isItalic;
        public string content;
    }

}
