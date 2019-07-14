using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Lunalipse.Core.Markdown
{
    public class ParseParagraph
    {
        const double baseFontSizePx = 14d;
        readonly double[] HeaderTagFontSize = { 2, 1.5, 1.17, 1, .83, .67 };

        ParseLines parseLines;
        public ParseParagraph()
        {
            parseLines = new ParseLines();
        }
        public Paragraph Parse(string para)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.fontSize = baseFontSizePx;
            paragraph.thickness = new Thickness(0, 0, 0, 25);

            if (string.IsNullOrEmpty(para)) return paragraph;
            int headerSize = 0;
            if (para.StartsWith("<c>"))
            {
                paragraph.needCenter = true;
                para = para.Remove(0, 3);
            }
            else if (para.StartsWith("<r>"))
            {
                paragraph.needRight = true;
                para = para.Remove(0, 3);
            }
            for (headerSize = 0; headerSize < 6 && para[headerSize] == '#'; headerSize++) ;
            paragraph.isHeader = headerSize > 0;
            if (headerSize > 0)
            {
                para = para.Remove(0, headerSize);
                paragraph.fontSize = HeaderTagFontSize[headerSize - 1] * baseFontSizePx;
                paragraph.thickness.Bottom = 16;
            }
            else if (headerSize == 0)
            {
                paragraph.isReferrence = para[0] == '>';
                if (paragraph.isReferrence) para = para.Remove(0, 1);
            }
            paragraph.isList = para[0] == '+'|| para[0] == '-';
            if (paragraph.isList) para = para.Remove(0, 1);
            para = para.Trim();
            paragraph.texts = parseLines.ParseSignleLine(para);
            return paragraph;
        }
    }
    public struct Paragraph
    {
        public bool isHeader;
        public bool isReferrence;
        public bool isList;
        public bool needCenter,needRight;
        public double fontSize;
        public Thickness thickness;
        public List<TextFormate> texts;
    }
}
